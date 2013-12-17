using HalconDotNet;
using Hanbo.Helper;
using MeasureModule;
using MeasureModule.Resolver;
using MeasureModule.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ViewROI;


namespace Hanbo.WindowControlWrapper
{
	public enum GeoDataGridViewNotifyType { DeleteRow, ShowGeoImage, TreeView_AfterCheck, TreeView_AfterSelect, ReloadData, UpdateData, ErrorMessage }
	public delegate void GeoDataGridViewRecordChangeNotify(GeoDataGridViewNotifyType notifyType, object data);
	public class GeoDataGridViewManager
	{
		private DataGridView _GridViewContainer;
		private BindingSource _BindingSource;
		private BindingList<GeoDataGridViewModel> _DataList;
		private string[] _InvisiableColumnNames;
		private Dictionary<string, Bitmap> _ImageList;
		private string _ExportUnit { get; set; }
		private double _Resolution;
		private int _RoundDigit;
		private MeasureAssistant mAssistant;
		private TreeView _TreeViewContainer;

		//
		public event GeoDataGridViewRecordChangeNotify On_RecordChanged;

		//
		public CalcuteType DoCalculate = CalcuteType.None;

		//
		public GeoDataGridViewManager(DataGridView container, BindingList<GeoDataGridViewModel> bindingList, string[] invisiableColumnNames, Dictionary<string, Bitmap> iconImageList, double resolution, int roundDigit, MeasureAssistant assistant)
		{
			_GridViewContainer = container;
			_DataList = bindingList;
			_InvisiableColumnNames = invisiableColumnNames;
			_ImageList = iconImageList;
			_Resolution = resolution;
			_RoundDigit = roundDigit;
			mAssistant = assistant;
			initialize();
		}

		#region Public APIs

		/// <summary>
		/// 載入資料
		/// </summary>
		/// <param name="bindingList"></param>
		public void LoadRecord(BindingList<GeoDataGridViewModel> bindingList)
		{
			this.Clear();
			foreach (var geoModel in bindingList)
			{
				if (geoModel.ROIModel != null)
				{
					this._DataList.Add(geoModel);
					var reloadROI = geoModel.ROIModel.MakeROI();
					reloadROI.ROIMeasureType = geoModel.GeoType;

					addTreeNode(reloadROI, geoModel);
					notifyRecordChanged(GeoDataGridViewNotifyType.ReloadData, reloadROI);
				}
				else
				{
					addMeasuredViewModel(geoModel);
					notifyRecordChanged(GeoDataGridViewNotifyType.ReloadData, "");
				}
			}
			this.Refresh();
		}

		public void SetTreeViewControl(TreeView treeView, ImageList treeViewImageList)
		{
			initializeTreeView(treeView, treeViewImageList);
		}

		/// <summary>
		/// 刪除記錄
		/// </summary>
		/// <param name="id"></param>
		public void DeleteViewModel(string id)
		{
			deleteRecord(id);
		}

		/// <summary>
		/// 取得 GeoDataGridViewModel
		/// </summary>
		/// <param name="id">RecordID or ROIID</param>
		/// <returns></returns>
		public GeoDataGridViewModel GetViewModel(string id)
		{
			return getModel(id);
		}

		/// <summary>
		/// 新增 ViewModel or 更新 ViewModel
		/// </summary>
		/// <param name="model"></param>
		/// <param name="roi"></param>
		public void UpdateViewModel(MeasureViewModel model, ROI roi)
		{
			GeoDataGridViewModel tmpGeoDataViewModel = measureMapToGeoDataViewModel(model, roi);
			if (tmpGeoDataViewModel == null) return;

			var roiGeoOwner = GetViewModel(roi.ID);
			var isAddNew = (roiGeoOwner == null);
			var updateTreeNodeID = isAddNew ? tmpGeoDataViewModel.RecordID : roiGeoOwner.RecordID;
			if (isAddNew)
			{
				_DataList.Add(tmpGeoDataViewModel);
				addTreeNode(roi, tmpGeoDataViewModel);
			}
			else
			{
				updateRecord(roiGeoOwner.RecordID, tmpGeoDataViewModel);
			}
			this.Refresh();
			setTreeNodeFocus(updateTreeNodeID);
			notifyRecordChanged(GeoDataGridViewNotifyType.UpdateData, tmpGeoDataViewModel);
		}

		/// <summary>
		/// 更新 GridView
		/// </summary>
		public void Refresh()
		{
			//檢查資料, 標記顏色
			foreach (DataGridViewRow row in _GridViewContainer.Rows)
			{
				var cell = row.Cells["GeoType"];
				if (cell != null)
				{
					string cellValue = cell.Value.ToString();
					MeasureType geotype;
					if (Enum.TryParse<MeasureType>(cellValue, out geotype))
					{
						var valid = false;
						var rowBegin = row.Cells["Row1"].Value.ToString();
						var distance = row.Cells["Distance"].Value.ToString();
						valid = (!DistanceHelper.IsResultType(geotype)) ? rowBegin != "-1" : distance != "-1";
						Color cellColor = (!valid) ? Color.OrangeRed : System.Drawing.SystemColors.Control;
						foreach (DataGridViewCell colorCell in row.Cells)
						{
							colorCell.Style.BackColor = cellColor;
						}
					}
				}
			}
			_GridViewContainer.Refresh();
		}

		public void SetCalcuteType(CalcuteType cType)
		{
			DoCalculate = cType;
			_GridViewContainer.Columns[0].Visible = true;
		}

		public void Clear()
		{
			_DataList.Clear();
			if (_TreeViewContainer != null)
			{
				_TreeViewContainer.Nodes.Clear();
			}
		}

		public int GetRecordCount()
		{
			return _DataList.Count;
		}

		/// <summary>
		/// 取得所有的量測資料
		/// </summary>
		/// <returns></returns>
		public BindingList<GeoDataGridViewModel> GetAllRecord()
		{
			return _DataList;
		}

		public void SetUnit(string unit)
		{
			_ExportUnit = unit;
			//更新Data
			foreach (var item in _DataList.Where(p => p.Distance > 0.0))
			{
				var isDistanceUnit = (!String.IsNullOrEmpty(item.Unit) && item.Unit != "Angle");
				if (isDistanceUnit)
				{
					item.Unit = _ExportUnit;
					var pixelValue = item.Distance;

					item.WorldDistance = pixelToRealWorldValue(pixelValue);
				}
			}
			Refresh();
		}
		public string GetUnit()
		{
			return _ExportUnit;
		}
		#endregion

		/// <summary>
		/// 更新 ViewModel
		/// </summary>
		/// <param name="id">RecordID</param>
		/// <param name="tmpGeoDataViewModel"></param>
		private void updateRecord(string id, GeoDataGridViewModel tmpGeoDataViewModel)
		{
			var roiGeoOwner = _DataList.SingleOrDefault(p => p.RecordID == id);
			if (roiGeoOwner != null)
			{
				roiGeoOwner.Col1 = tmpGeoDataViewModel.Col1;
				roiGeoOwner.Row1 = tmpGeoDataViewModel.Row1;
				roiGeoOwner.Distance = tmpGeoDataViewModel.Distance;
				roiGeoOwner.WorldDistance = pixelToRealWorldValue(tmpGeoDataViewModel.Distance);
				roiGeoOwner.Col2 = tmpGeoDataViewModel.Col2;
				roiGeoOwner.Row2 = tmpGeoDataViewModel.Row2;
				roiGeoOwner.ROIModel = tmpGeoDataViewModel.ROIModel;
				//更新與此Geo 相關的 Geo Objects
				updateDependGeoObject(roiGeoOwner);
			}
		}

		private void setTreeNodeFocus(string id, bool searchChild)
		{
			if (_TreeViewContainer == null) return;
			var nodes = _TreeViewContainer.Nodes.Find(id, searchChild);
			if (nodes.Length > 0)
			{
				_TreeViewContainer.Focus();
				_TreeViewContainer.SelectedNode = nodes[0];
			}
		}
		private void setTreeNodeFocus(string id)
		{
			setTreeNodeFocus(id, false);
		}

		/// <summary>
		/// 新增量測記錄 (distance, symmetrylin, angle, 3pointCircle)
		/// </summary>
		/// <param name="model"></param>
		private void addMeasuredViewModel(GeoDataGridViewModel model)
		{
			_DataList.Add(model);
			addMeasuredTreeNode(model);
		}

		private void addMeasuredTreeNode(GeoDataGridViewModel geoModel)
		{
			if (_TreeViewContainer != null)
			{
				//目前 Focus 的節點
				var selectedNode = _TreeViewContainer.SelectedNode;

				//相依的 ROI 資料列
				var dependsRecordIDs = _DataList.Where(p => geoModel.DependGeoRowNames.Contains(p.RecordID) && p.ROIID != null).Select(p => p.RecordID);

				//找到樹狀結構中相對應的 ROI 節點
				var nodes = _TreeViewContainer.Nodes.Cast<TreeNode>().Where(p => dependsRecordIDs.Contains(p.Name));
				foreach (TreeNode pNode in nodes)
				{
					//add subNode
					var geoNode = getGeoTreeNode(geoModel);
					pNode.Nodes.Add(geoNode);
					HideCheckBox(_TreeViewContainer, geoNode);
				}
				_TreeViewContainer.Focus();
				_TreeViewContainer.SelectedNode = selectedNode;
			}
		}

		private TreeNode getGeoTreeNode(GeoDataGridViewModel geoModel)
		{
			var number = _DataList.Count;
			var geoNodeDisplayName = (String.IsNullOrEmpty(geoModel.Name)) ? String.Format("{0} {1}", number.ToString("d2"), geoModel.GeoType) : geoModel.Name;
			var geoImageKey = geoModel.GeoType.ToString();
			TreeNode geoNode = new TreeNode() { Name = geoModel.RecordID, Text = geoNodeDisplayName, ImageKey = geoImageKey, SelectedImageKey = geoImageKey };
			return geoNode;
		}

		private void addTreeNode(ROI activeROI, GeoDataGridViewModel geoModel)
		{
			if (_TreeViewContainer != null)
			{
				var number = _DataList.Count;
				var roiNodeName = String.IsNullOrEmpty(geoModel.Name) ? String.Format("{0} {1}", number.ToString("d2"), activeROI.ROIMeasureType) : geoModel.Name;
				var roiImageKey = activeROI.ROIMeasureType.ToString();
				var index = _TreeViewContainer.Nodes.Count;
				TreeNode roiNode = new TreeNode() { Name = geoModel.RecordID, Text = roiNodeName, ImageKey = roiImageKey, SelectedImageKey = roiImageKey, Checked = activeROI.Visiable };
				roiNode.Tag = activeROI;

				_TreeViewContainer.Nodes.Add(roiNode);
				_TreeViewContainer.Focus();
				_TreeViewContainer.SelectedNode = roiNode;
			}

		}

		private GeoDataGridViewModel getModel(string id)
		{
			var model = _DataList.SingleOrDefault(p => p.RecordID == id);
			if (model == null)
				model = _DataList.SingleOrDefault(p => p.ROIID == id);
			return model;
		}

		/// <summary>
		/// 組成 GeoDataViewModel
		/// </summary>
		/// <param name="roiIndex">active ROI index</param>
		/// <param name="viewModel">MeasureViewModel</param>
		/// <param name="roi">ROI</param>
		/// <returns></returns>
		private GeoDataGridViewModel measureMapToGeoDataViewModel(MeasureViewModel viewModel, ROI roi)
		{
			if (roi == null) return null;

			var number = _DataList.Count + 1;
			var measureName = number.ToString("d2") + " " + roi.ROIMeasureType;
			var exportUnit = roi.ROIMeasureType == MeasureType.Angle ? "Angle" :
							roi.ROIMeasureType == MeasureType.Point ? "" : _ExportUnit;
			GeoDataGridViewModel geoModel = new GeoDataGridViewModel()
			{
				Icon = getGeoViewModelIcon(roi),
				Name = measureName,
				ROIID = roi.ID,
				ROIModel = roi.ToROIViewModel(),
				Col1 = (viewModel.Col1 != null && viewModel.Col1.TupleLength() > 0) ?
						viewModel.Col1.D : -1.0,

				Row1 = (viewModel.Row1 != null && viewModel.Row1.TupleLength() > 0) ?
						viewModel.Row1.D : -1.0,

				Distance = (viewModel.Distance != null && viewModel.Distance.TupleLength() > 0) ?
						viewModel.Distance.D : 0.0,

				WorldDistance = (viewModel.Distance != null && viewModel.Distance.TupleLength() > 0) ?
						pixelToRealWorldValue(viewModel.Distance.D) : 0.0,

				Col2 = (viewModel.Col2 != null && viewModel.Col2.TupleLength() > 0) ?
						viewModel.Col2.D : -1.0,

				Row2 = (viewModel.Row2 != null && viewModel.Row2.TupleLength() > 0) ?
						viewModel.Row2.D : -1.0,
				Selected = false,
				Unit = exportUnit,
				GeoType = roi.ROIMeasureType,
			};
			return geoModel;
		}
		/// <summary>
		/// 取得 幾何物件 Image
		/// </summary>
		/// <param name="roi"></param>
		/// <returns></returns>
		private Bitmap getGeoViewModelIcon(ROI roi)
		{
			if (roi == null) return null;
			Bitmap iconImage = null;
			switch (roi.ROIMeasureType)
			{
				case MeasureType.Point:
					iconImage = _ImageList[MeasureType.Point.ToString()];
					break;
				case MeasureType.Line:
					iconImage = _ImageList[MeasureType.Line.ToString()];
					break;
				case MeasureType.Circle:
					iconImage = _ImageList[MeasureType.Circle.ToString()];
					break;
			}
			return iconImage;
		}

		private void updateDependGeoObject(GeoDataGridViewModel roiGeoOwner)
		{
			//去除自已以外，其他有相依的資料列們
			var hasDependRecords = _DataList.Where(p => p.DependGeoRowNames != null && p.RecordID != roiGeoOwner.RecordID);

			//與本列相關的資料列
			var oneToManyGeoList = hasDependRecords.Where(q => q.DependGeoRowNames.Contains(roiGeoOwner.RecordID));

			foreach (var item in oneToManyGeoList)
			{
				var firstID = item.DependGeoRowNames.Length > 0 ? item.DependGeoRowNames[0] : "";
				var secondID = item.DependGeoRowNames.Length > 1 ? item.DependGeoRowNames[1] : "";
				var thirdID = item.DependGeoRowNames.Length > 2 ? item.DependGeoRowNames[2] : "";
				var firstModel = _DataList.SingleOrDefault(p => p.RecordID == firstID);
				var secondModel = _DataList.SingleOrDefault(p => p.RecordID == secondID);
				var thirdModel = _DataList.SingleOrDefault(p => p.RecordID == thirdID);
				GeoDataGridViewModel newModel = null;
				switch (item.GeoType)
				{
					case MeasureType.Angle:
						newModel = makeAngleGeoDataViewModel(firstModel, secondModel);
						break;
					case MeasureType.CrossPoint:
						newModel = makeCrossPointGeoDataViewModel(firstModel, secondModel);
						break;
					case MeasureType.Distance:
						newModel = makeDistanceGeoDataViewModel(firstModel, secondModel);
						break;
					case MeasureType.DistanceX:
						newModel = makeDistanceXGeoDataViewModel(firstModel, secondModel);
						break;
					case MeasureType.DistanceY:
						newModel = makeDistanceYGeoDataViewModel(firstModel, secondModel);
						break;
					case MeasureType.PointCircle:
						newModel = make3PointToCircleGeoDataViewModel(firstModel, secondModel, thirdModel);
						break;
					case MeasureType.SymmetryLine:
						newModel = makeSymmetryLineGeoDataViewModel(firstModel, secondModel);
						break;
				}
				reAssignModelValue(item, newModel);
			}
		}

		private void reAssignModelValue(GeoDataGridViewModel item, GeoDataGridViewModel newModel)
		{
			var lineTypes = new MeasureType[] { MeasureType.Distance, MeasureType.DistanceX, MeasureType.DistanceY, MeasureType.SymmetryLine };
			var isCrossPoint = (item.GeoType == MeasureType.CrossPoint);
			if (newModel == null)
			{
				item.Row1 = -1;
				item.Col1 = -1;
				item.Row2 = -1;
				item.Col2 = -1;
				item.Distance = -1;
				item.WorldDistance = -1;
			}
			else
			{
				item.Row1 = newModel.Row1;
				item.Col1 = newModel.Col1;
				if (!isCrossPoint)
				{
					if (lineTypes.Contains(item.GeoType))
					{
						item.Row2 = newModel.Row2;
						item.Col2 = newModel.Col2;
					}
					item.Distance = newModel.Distance;
					item.WorldDistance = newModel.WorldDistance;
				}
			}
			if (isCrossPoint)
			{
				updateDependGeoObject(item);
			}
		}

		private void initialize()
		{
			//Data
			_BindingSource = new BindingSource() { DataSource = _DataList };

			//Data Binding
			_GridViewContainer.DataSource = _BindingSource;

			//Column initialize
			initColumn();

			//Event
			_GridViewContainer.CellClick += _GridViewContainer_CellClick;
			_GridViewContainer.CellEndEdit += _GridViewContainer_CellEndEdit;
			_GridViewContainer.CellContentClick += DataGridView_CellContentClick;
			_GridViewContainer.CellDoubleClick += DataGridView_CellDoubleClick;
			_GridViewContainer.UserDeletingRow += DataGridView_UserDeletingRow;

			//Default value
			_ExportUnit = "mm";

		}

		void _GridViewContainer_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// GridViewCell 與 TreeView 互動
			if (DoCalculate == CalcuteType.None && _TreeViewContainer != null)
			{
				var gridView = sender as DataGridView;
				DataGridViewRow row = gridView.Rows[e.RowIndex];
				var rowCell = row.Cells["RecordID"];//.Value as ROIViewModel;
				if (rowCell != null)
				{
					var recordID = rowCell.Value.ToString();
					setTreeNodeFocus(recordID, true);
				}
			}
		}

		void _GridViewContainer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (_TreeViewContainer != null)
			{
				var gridview = sender as DataGridView;
				if (gridview != null)
				{
					var columnName = gridview.Columns[e.ColumnIndex].Name;

					try
					{
						if (columnName == "Name")
						{
							DataGridViewRow row = gridview.Rows[e.RowIndex];
							var recordID = (string)row.Cells["RecordID"].Value;

							var newCellValue = (string)gridview[e.ColumnIndex, e.RowIndex].Value;
							//相依的 ROI 資料列
							var dependsRecordIDs = _DataList.Where(p => p.RecordID == recordID)
													.Select(p => p.DependGeoRowNames).FirstOrDefault();

							var parentID = _DataList.Where(p => p.RecordID == recordID).SingleOrDefault();
							if (parentID != null)
							{
								var nodes = _TreeViewContainer.Nodes.Cast<TreeNode>().Where(p => p.Name == parentID.RecordID);
								foreach (TreeNode parentNode in nodes)
								{
									parentNode.ToolTipText = parentNode.Text = newCellValue;
								}

							}

							//找到樹狀結構中相對應的 ROI 節點
							if (dependsRecordIDs != null)
							{
								var nodes = _TreeViewContainer.Nodes.Cast<TreeNode>().Where(p => dependsRecordIDs.Contains(p.Name));
								foreach (TreeNode parentNode in nodes)
								{
									var node = parentNode.Nodes.Cast<TreeNode>().SingleOrDefault(p => p.Name == recordID);
									if (node != null)
									{
										node.ToolTipText = node.Text = newCellValue;
									}
								}
							}
							if (dependsRecordIDs != null || parentID != null)
								notifyRecordChanged(GeoDataGridViewNotifyType.UpdateData, null);
						}
					}
					catch (Exception ex)
					{
						Hanbo.Log.LogManager.Error(ex);
					}

				}
			}
		}

		private void DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			var rowIndex = e.Row.Index;
			var model = _DataList[rowIndex];
			deleteRecord(model.RecordID);
			//deleteGridViewRecord(model);
			e.Cancel = true;
			return;
		}

		/// <summary>
		/// 刪除記錄 (樹狀&GridView)
		/// </summary>
		/// <param name="id">RecordID or ROIID</param>
		private void deleteRecord(string id)
		{
			var model = getModel(id);

			//Delete GridView Record
			if (model != null)
			{
				var confirmDelete = deleteGridViewRecord(model);

				//Delete TreeView Record
				if (confirmDelete)
					deleteTreeViewNode(id);
			}
		}

		private void deleteTreeViewNode(string id)
		{
			if (_TreeViewContainer != null)
			{
				var deleteNodes = _TreeViewContainer.Nodes.Find(id, true);
				foreach (TreeNode deleteNode in deleteNodes)
				{
					var isLeaf = deleteNode.Nodes.Count == 0;
					if (isLeaf)
					{
						deleteNode.Remove();
					}
					else
					{
						//取得目前要刪除節點的子節點IDs
						var subNodeNames = deleteNode.Nodes.Cast<TreeNode>().Select(p => p.Name).ToArray();

						//刪除子節點包含目前刪除節點的子節點
						foreach (var name in subNodeNames)
						{
							_TreeViewContainer.Nodes.Find(name, true)[0].Remove();
						}

						//刪除目前選擇的節點
						_TreeViewContainer.Nodes.Remove(deleteNode);
					}
				}
			}

		}

		private bool deleteGridViewRecord(GeoDataGridViewModel model)
		{
			var hasROIDependencyModels = _DataList.Where(p => p.RecordID != model.RecordID && p.DependGeoRowNames != null);
			var relatedModelIDs = hasROIDependencyModels.Where(q => q.DependGeoRowNames.Contains(model.RecordID)).Select(p => p.RecordID);
			string message = (relatedModelIDs.Count() > 0) ? Hanbo.Resources.Resource.Message_DependencyWarning
															: Hanbo.Resources.Resource.Message_DeleteNotice;
			var confirmDelete = MessageBox.Show(message, Hanbo.Resources.Resource.Message_Confirm, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
			if (confirmDelete)
			{
				//刪除相依的 Model
				var modelROIID = model.ROIID;
				var deletedRows = _DataList.Where(p => relatedModelIDs.Contains(p.RecordID)).ToList();
				for (int i = 0; i < deletedRows.Count; i++)
				{
					var row = deletedRows[i];
					var recordID = row.RecordID;
					_DataList.Remove(row);
				}
				_DataList.Remove(model);
				notifyRecordChanged(GeoDataGridViewNotifyType.DeleteRow, modelROIID);
			}
			return confirmDelete;
		}

		private void notifyRecordChanged(GeoDataGridViewNotifyType notifyType, object notifyData)
		{
			if (On_RecordChanged != null)
			{
				On_RecordChanged(notifyType, notifyData);
			}
		}

		private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			//顯示幾何量測圖形
			if (e.RowIndex < 0) return;
			GeoDataGridViewModel geoModel = _DataList[e.RowIndex];
			HObject displayObject = displayObject = DistanceHelper.GenContour(geoModel);
			var isShow = (displayObject != null) ? true : false;
			if (isShow)
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ShowGeoImage, displayObject);
			}
		}

		/// <summary>
		/// 點擊 Cell CheckBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var gridView = sender as DataGridView;

			if (gridView.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
			{
				gridView.EndEdit();
				var checkRows = gridView.Rows.Cast<DataGridViewRow>().Where(p => Convert.ToBoolean(p.Cells[0].Value) == true).ToArray();
				var doDistance = (DoCalculate == CalcuteType.Distance && checkRows.Count() == 2);
				var doPoint3ToCircle = (DoCalculate == CalcuteType.Point3ToCircle && checkRows.Count() == 3);
				var doAngle = (DoCalculate == CalcuteType.Angle && checkRows.Count() == 2);
				var doSymetryLine = (DoCalculate == CalcuteType.SymmetryLine && checkRows.Count() == 2);
				var doCrossPoint = (DoCalculate == CalcuteType.CrossPoint && checkRows.Count() == 2);
				var doDistanceX = (DoCalculate == CalcuteType.DistanceX && checkRows.Count() == 2);
				var doDistanceY = (DoCalculate == CalcuteType.DistanceY && checkRows.Count() == 2);

				if (doDistance)
				{
					measureDistance(checkRows);
				}
				else if (doSymetryLine)
				{
					measureSymmeryLine(checkRows);
				}
				else if (doAngle)
				{
					measureAngle(checkRows);
				}
				else if (doPoint3ToCircle)
				{
					measure3PointCircle(checkRows);
				}
				else if (doCrossPoint)
				{
					measureCrossPoint(checkRows);
				}
				else if (doDistanceX)
				{
					measureDistanceX(checkRows);
				}
				else if (doDistanceY)
				{
					measureDistanceY(checkRows);
				}

				//打完收工
				if (doDistance || doPoint3ToCircle || doAngle || doSymetryLine || doCrossPoint || doDistanceX || doDistanceY)
				{
					var columnIndex = e.ColumnIndex;
					restoreGridviewState(columnIndex, gridView, checkRows);
					notifyRecordChanged(GeoDataGridViewNotifyType.UpdateData, null);
				}
			}
		}

		private void measureDistanceY(DataGridViewRow[] checkRows)
		{
			var pA = _DataList[checkRows[0].Index];
			var pB = _DataList[checkRows[1].Index];
			var distanceYModel = makeDistanceYGeoDataViewModel(pA, pB);
			if (distanceYModel != null) this.addMeasuredViewModel(distanceYModel);
		}

		private GeoDataGridViewModel makeDistanceYGeoDataViewModel(GeoDataGridViewModel pA, GeoDataGridViewModel pB)
		{
			GeoDataGridViewModel result = null;
			var measure = new MeasurementDistanceY(MeasureViewModelResolver.Resolve(pA), MeasureViewModelResolver.Resolve(pB), mAssistant);
			var mModel = measure.GetViewModel();
			if (mModel != null)
			{
				var number = _DataList.Count + 1;
				var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_YwayDistance;
				result = new GeoDataGridViewModel()
				{
					Name = measureName,
					Distance = mModel.Distance,
					WorldDistance = pixelToRealWorldValue(mModel.Distance),
					Icon = _ImageList[MeasureType.DistanceY.ToString()],
					GeoType = MeasureType.DistanceY,
					Row1 = mModel.Row1,
					Col1 = mModel.Col1,
					Row2 = mModel.Row2,
					Col2 = mModel.Col2,
					DependGeoRowNames = new string[] { pA.RecordID, pB.RecordID },
					Selected = false,
					Unit = _ExportUnit,
				};
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage, Hanbo.Resources.Resource.Message_DistanceYWarning);
			}
			return result;
		}

		private void measureDistanceX(DataGridViewRow[] checkRows)
		{
			var pA = _DataList[checkRows[0].Index];
			var pB = _DataList[checkRows[1].Index];
			var distanceXModel = makeDistanceXGeoDataViewModel(pA, pB);
			if (distanceXModel != null) this.addMeasuredViewModel(distanceXModel);
		}

		private GeoDataGridViewModel makeDistanceXGeoDataViewModel(GeoDataGridViewModel pA, GeoDataGridViewModel pB)
		{
			GeoDataGridViewModel result = null;
			var measure = new MeasurementDistanceX(MeasureViewModelResolver.Resolve(pA), MeasureViewModelResolver.Resolve(pB), mAssistant);
			var mModel = measure.GetViewModel();
			if (mModel != null)
			{
				var number = _DataList.Count + 1;
				var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_XwayDistance; ;
				result = new GeoDataGridViewModel()
				{
					Name = measureName,
					Distance = mModel.Distance,
					WorldDistance = pixelToRealWorldValue(mModel.Distance),
					Icon = _ImageList[MeasureType.DistanceX.ToString()],
					GeoType = MeasureType.DistanceX,
					Row1 = mModel.Row1,
					Col1 = mModel.Col1,
					Row2 = mModel.Row2,
					Col2 = mModel.Col2,
					DependGeoRowNames = new string[] { pA.RecordID, pB.RecordID },
					Selected = false,
					Unit = _ExportUnit,
				};
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage, Hanbo.Resources.Resource.Message_DistanceXWarning);
			}
			return result;

		}

		private void measureCrossPoint(DataGridViewRow[] checkRows)
		{
			var pA = _DataList[checkRows[0].Index];
			var pB = _DataList[checkRows[1].Index];
			var crossPointModel = makeCrossPointGeoDataViewModel(pA, pB);
			if (crossPointModel != null) this.addMeasuredViewModel(crossPointModel);
		}

		private GeoDataGridViewModel makeCrossPointGeoDataViewModel(GeoDataGridViewModel lineOne, GeoDataGridViewModel lineTwo)
		{
			GeoDataGridViewModel result = null;
			var measure = new MeasurementTwoLineCrossPoint(MeasureViewModelResolver.Resolve(lineOne), MeasureViewModelResolver.Resolve(lineTwo), mAssistant);
			var mModel = measure.GetViewModel();
			if (mModel != null)
			{
				var resultData = measure.getMeasureResultData();
				var isParallel = (resultData as PointResult).IsParallel;
				if (isParallel)
				{
					notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
						, Hanbo.Resources.Resource.Message_CrossPointWarning);
				}
				else
				{
					var number = _DataList.Count + 1;
					var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_CrossPoint;
					result = new GeoDataGridViewModel()
					{
						Name = measureName,
						Icon = _ImageList[MeasureType.CrossPoint.ToString()],
						GeoType = MeasureType.CrossPoint,
						Row1 = mModel.Row1,
						Col1 = mModel.Col1,
						DependGeoRowNames = new string[] { lineOne.RecordID, lineTwo.RecordID },
						Selected = false,
						Unit = "",
					};
				}
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
					, "運算模型資料不正確！");

			}
			return result;
		}

		private void measure3PointCircle(DataGridViewRow[] checkRows)
		{
			var pA = _DataList[checkRows[0].Index];
			var pB = _DataList[checkRows[1].Index];
			var pC = _DataList[checkRows[2].Index];
			var circlPointViewModel = make3PointToCircleGeoDataViewModel(pA, pB, pC);
			if (circlPointViewModel != null) this.addMeasuredViewModel(circlPointViewModel);
		}

		private void measureAngle(DataGridViewRow[] checkRows)
		{
			var lineOne = _DataList[checkRows[0].Index];
			var lineTwo = _DataList[checkRows[1].Index];
			if (lineOne.Row2 > 0.0 && lineTwo.Row2 > 0.0)
			{
				var angleViewModel = makeAngleGeoDataViewModel(lineOne, lineTwo);
				if (angleViewModel != null) this.addMeasuredViewModel(angleViewModel);
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
					, Hanbo.Resources.Resource.Message_AngleWarning);
			}
		}

		private GeoDataGridViewModel makeAngleGeoDataViewModel(GeoDataGridViewModel lineOne, GeoDataGridViewModel lineTwo)
		{
			GeoDataGridViewModel result = null;
			var measure = new MeasurementTwoLineAngle(MeasureViewModelResolver.Resolve(lineOne), MeasureViewModelResolver.Resolve(lineTwo), mAssistant);
			var mModel = measure.GetViewModel();
			if (mModel != null)
			{
				var number = _DataList.Count + 1;
				var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_Angle;
				result = new GeoDataGridViewModel()
				{
					Name = measureName,
					Distance = mModel.Distance,
					WorldDistance = mModel.Distance,
					Icon = _ImageList[MeasureType.Angle.ToString()],
					GeoType = MeasureType.Angle,
					Row1 = mModel.Row1,
					Col1 = mModel.Col1,
					DependGeoRowNames = new string[] { lineOne.RecordID, lineTwo.RecordID },
					Selected = false,
					Unit = "Angle",
				};
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage,
									Hanbo.Resources.Resource.Message_AngleError);
			}
			return result;
		}

		private void measureSymmeryLine(DataGridViewRow[] checkRows)
		{
			var lineOne = _DataList[checkRows[0].Index];
			var lineTwo = _DataList[checkRows[1].Index];
			if (lineOne.Row2 > 0.0 && lineTwo.Row2 > 0.0)
			{
				var symmetryLineViewModel = makeSymmetryLineGeoDataViewModel(lineOne, lineTwo);
				if (symmetryLineViewModel != null) this.addMeasuredViewModel(symmetryLineViewModel);
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
					, Hanbo.Resources.Resource.Message_SymmetryLineWarning);
			}
		}

		private GeoDataGridViewModel makeSymmetryLineGeoDataViewModel(GeoDataGridViewModel lineOne, GeoDataGridViewModel lineTwo)
		{
			GeoDataGridViewModel result = null;
			var measure = new MeasurementSymmetryLine(MeasureViewModelResolver.Resolve(lineOne), MeasureViewModelResolver.Resolve(lineTwo), mAssistant);
			var mModel = measure.GetViewModel();
			if (mModel != null)
			{
				var number = _DataList.Count + 1;
				var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_SymmetryLine;
				result = new GeoDataGridViewModel()
				{
					Name = measureName,
					Distance = mModel.Distance,
					WorldDistance = pixelToRealWorldValue(mModel.Distance),
					Icon = _ImageList[MeasureType.SymmetryLine.ToString()],
					GeoType = MeasureType.SymmetryLine,
					Row1 = mModel.Row1,
					Col1 = mModel.Col1,
					Row2 = mModel.Row2,
					Col2 = mModel.Col2,
					DependGeoRowNames = new string[] { lineOne.RecordID, lineTwo.RecordID },
					Selected = false,
					Unit = _ExportUnit,
				};
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage,
									Hanbo.Resources.Resource.Message_SymmetryLineError);
			}
			return result;
		}

		/// <summary>
		/// 回復 GridView 的狀態
		/// </summary>
		/// <param name="columnIndex"></param>
		/// <param name="gridView"></param>
		/// <param name="checkRows"></param>
		private void restoreGridviewState(int columnIndex, DataGridView gridView, DataGridViewRow[] checkRows)
		{
			DoCalculate = CalcuteType.None;
			foreach (var item in checkRows)
			{
				item.Cells[columnIndex].Value = false;
			}
			gridView.Columns[0].Visible = false;
		}

		private void measureDistance(DataGridViewRow[] checkRows)
		{
			try
			{
				var firstRowIndex = checkRows[0].Index;
				var secondRowIndex = checkRows[1].Index;
				var firstModel = _DataList[firstRowIndex];
				var secondModel = _DataList[secondRowIndex];
				var distanceViewMoel = makeDistanceGeoDataViewModel(firstModel, secondModel);
				if (distanceViewMoel != null)
				{
					this.addMeasuredViewModel(distanceViewMoel);
				}
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error("計算距離 Error:" + ex.Message);
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
					, Hanbo.Resources.Resource.Message_DistanceCalculationError);
			}
		}

		private GeoDataGridViewModel makeDistanceGeoDataViewModel(GeoDataGridViewModel firstModel, GeoDataGridViewModel secondModel)
		{
			GeoDataGridViewModel result = null;
			if (firstModel != null && secondModel != null)
			{
				var newModel = DistanceHelper.CaculateDistance(MeasureViewModelResolver.Resolve(firstModel), MeasureViewModelResolver.Resolve(secondModel));
				if (newModel != null)
				{
					var number = _DataList.Count + 1;
					var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_Distance;
					result = new GeoDataGridViewModel()
					{
						Name = measureName,
						Distance = newModel.Distance,
						WorldDistance = pixelToRealWorldValue(newModel.Distance),
						Icon = _ImageList[MeasureType.Distance.ToString()],
						GeoType = MeasureType.Distance,
						Row1 = newModel.Row1,
						Col1 = newModel.Col1,
						Row2 = newModel.Row2,
						Col2 = newModel.Col2,
						DependGeoRowNames = new string[] { firstModel.RecordID, secondModel.RecordID },
						Selected = false,
						Unit = _ExportUnit,
					};
				}
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage,
									Hanbo.Resources.Resource.Message_DependencyNotExists);
			}
			return result;
		}

		/// <summary>
		/// 3點成圓計算
		/// </summary>
		/// <param name="pA">幾何模型 點 A</param>
		/// <param name="pB">幾何模型 點 B</param>
		/// <param name="pC">幾何模型 點 C</param>
		/// <returns></returns>
		private GeoDataGridViewModel make3PointToCircleGeoDataViewModel(GeoDataGridViewModel pA, GeoDataGridViewModel pB, GeoDataGridViewModel pC)
		{
			GeoDataGridViewModel result = null;
			var pAModel = MeasureViewModelResolver.Resolve(pA);
			var pBModel = MeasureViewModelResolver.Resolve(pB);
			var pcModel = MeasureViewModelResolver.Resolve(pC);

			var isAllPointModel = DistanceHelper.IsPointType(pAModel) && DistanceHelper.IsPointType(pBModel) && DistanceHelper.IsPointType(pcModel);
			if (!isAllPointModel)
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage
					, Hanbo.Resources.Resource.Message_DependencyNotExists);
				return result;
			}
			MeasureViewModel newModel = DistanceHelper.Get3PointToCircleModel(pAModel, pBModel, pcModel);
			if (newModel != null)
			{
				var number = _DataList.Count + 1;
				var measureName = number.ToString("d2") + " " + Hanbo.Resources.Resource.Model_3PointCircle;
				result = new GeoDataGridViewModel()
				{
					Name = measureName,
					Distance = newModel.Distance,
					WorldDistance = pixelToRealWorldValue(newModel.Distance),
					Icon = _ImageList[MeasureType.PointCircle.ToString()],
					GeoType = MeasureType.PointCircle,
					Row1 = newModel.Row1,
					Col1 = newModel.Col1,
					DependGeoRowNames = new string[] { pA.RecordID, pB.RecordID, pC.RecordID },
					Selected = false,
					Unit = _ExportUnit,
				};
			}
			else
			{
				notifyRecordChanged(GeoDataGridViewNotifyType.ErrorMessage,
					Hanbo.Resources.Resource.Message_3PointCircleWarning);
			}
			return result;
		}

		private void initColumn()
		{
			for (int i = 0; i < _InvisiableColumnNames.Length; i++)
			{
				var columnName = _InvisiableColumnNames[i];
				var column = _GridViewContainer.Columns[columnName];
				if (column != null) column.Visible = false;
			}
		}

		/// <summary>
		/// Pixel 轉換為實際尺寸
		/// </summary>
		/// <param name="value">Pixel value</param>
		/// <returns>real world value</returns>
		private double pixelToRealWorldValue(double value)
		{
			return UnitConverter.PixelToRealWorldValue(value, _ExportUnit, _Resolution, _RoundDigit);
		}

		#region TreeView 初始化
		private void initializeTreeView(TreeView treeView, ImageList treeViewImageList)
		{
			_TreeViewContainer = treeView;

			//param
			_TreeViewContainer.CheckBoxes = true;
			_TreeViewContainer.ImageList = treeViewImageList;

			//event
			_TreeViewContainer.AfterSelect += _TreeViewContainer_AfterSelect;//按下節點			
			_TreeViewContainer.KeyDown += _TreeViewContainer_KeyDown; //;//偵測按下 Delete
			_TreeViewContainer.AfterCheck += _TreeViewContainer_AfterCheck; //按下CheckBox 後
		}

		void _TreeViewContainer_AfterCheck(object sender, TreeViewEventArgs e)
		{
			var roi = e.Node.Tag as ROI;
			roi.Visiable = e.Node.Checked;
			notifyRecordChanged(GeoDataGridViewNotifyType.TreeView_AfterCheck, e);
		}

		void _TreeViewContainer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				var tree = sender as TreeView;
				if (tree.SelectedNode != null)
				{
					this.deleteRecord(tree.SelectedNode.Name);
				}
				e.Handled = true;
			}
		}

		void _TreeViewContainer_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ROI roi = e.Node.Tag as ROI;
			setGridViewCellFocus(e.Node);
			notifyRecordChanged(GeoDataGridViewNotifyType.TreeView_AfterSelect, roi);
		}

		private void setGridViewCellFocus(TreeNode treeNode)
		{
			var rowModel = _GridViewContainer.Rows.Cast<DataGridViewRow>()
						.Where(p => p.Cells["RecordID"].Value.ToString() == treeNode.Name)
						.Select(p => new
						{
							RowIndex = p.Cells["Name"].RowIndex,
							ColumnIndex = p.Cells["Name"].ColumnIndex
						}).FirstOrDefault();
			if (rowModel != null)
				_GridViewContainer.CurrentCell = _GridViewContainer[rowModel.ColumnIndex, rowModel.RowIndex];
		}
		public void SetTreeViewNodeActivate(ROI activeROI)
		{
			if (_TreeViewContainer != null)
			{
				findTheNode(activeROI, _TreeViewContainer.Nodes);
			}
		}

		private void findTheNode(ROI activeROI, TreeNodeCollection treeNodeCollection)
		{
			try
			{
				foreach (TreeNode node in treeNodeCollection)
				{
					var nodeROI = node.Tag as ROI;
					var findIt = (nodeROI != null && nodeROI.ID == activeROI.ID);
					if (findIt)
					{
						node.TreeView.SelectedNode = node;
						node.Expand();
						node.TreeView.Focus();
						break;
					}
					if (!findIt)
						findTheNode(activeROI, node.Nodes);
				}
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
		}
		#region TreeView  原生方法
		private const int TVIF_STATE = 0x8;
		private const int TVIS_STATEIMAGEMASK = 0xF000;
		private const int TV_FIRST = 0x1100;
		private const int TVM_SETITEM = TV_FIRST + 63;

		[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
		private struct TVITEM
		{
			public int mask;
			public IntPtr hItem;
			public int state;
			public int stateMask;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszText;
			public int cchTextMax;
			public int iImage;
			public int iSelectedImage;
			public int cChildren;
			public IntPtr lParam;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
												 ref TVITEM lParam);

		/// <summary>
		/// Hides the checkbox for the specified node on a TreeView control.
		/// </summary>
		private void HideCheckBox(TreeView tvw, TreeNode node)
		{
			TVITEM tvi = new TVITEM();
			tvi.hItem = node.Handle;
			tvi.mask = TVIF_STATE;
			tvi.stateMask = TVIS_STATEIMAGEMASK;
			tvi.state = 0;
			SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
		}
		#endregion
		#endregion
	}
}
