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
using ViewROI.SmartROIs;
using Hanbo.Extensions;


namespace Hanbo.WindowControlWrapper
{
	public enum GeoDataGridViewNotifyType { DeleteRow, ShowGeoImage, TreeView_AfterCheck, TreeView_AfterSelect, ReloadData, UpdateData, ErrorMessage, Coordinate_Removed, Coordinate_NameChanged, Clear, Calculated, Skew_NameChanged }
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

		//參考座標
		private BindingList<RefCoordinate> _refCoordinate;
		private string _currentCoordinateID;

		//參考軸擺正
		private BindingList<RefSkew> _refSkew;
		private string _currentSkewID;
		private RefSkew _defaultRefSkew;

		//
		public event GeoDataGridViewRecordChangeNotify On_RecordChanged;

		//
		public CalcuteType DoCalculate = CalcuteType.None;

		private ContextMenuStrip _geoContextMenuStrip;

		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="container">DataGrid</param>
		/// <param name="menuStrip">右鍵選單</param>
		/// <param name="refCoordinate">參考座標</param>
		/// <param name="refSkew">參考軸擺正</param>
		/// <param name="invisiableColumnNames">不顯示的欄位名稱們</param>
		/// <param name="iconImageList">圖示s</param>
		/// <param name="resolution">解析度</param>
		/// <param name="roundDigit">進位</param>
		/// <param name="assistant"></param>
		public GeoDataGridViewManager(DataGridView container,
										ContextMenuStrip menuStrip,
										BindingList<RefCoordinate> refCoordinate,
										BindingList<RefSkew> refSkew,
										string[] invisiableColumnNames,
										Dictionary<string, Bitmap> iconImageList,
										double resolution,
										int roundDigit, MeasureAssistant assistant)
		{
			_GridViewContainer = container;
			_geoContextMenuStrip = menuStrip;
			_refCoordinate = refCoordinate;
			_refSkew = refSkew;

			_defaultRefSkew = new RefSkew();
			//初始化 skewID
			if (_refSkew == null)
			{
				_refSkew = new BindingList<RefSkew>();
				_refSkew.Add(_defaultRefSkew);
			}
			_currentSkewID = (_refSkew.Count > 0) ? _refSkew[0].ID : "";

			_DataList = new BindingList<GeoDataGridViewModel>();
			_InvisiableColumnNames = invisiableColumnNames;
			_ImageList = iconImageList;
			_Resolution = resolution;
			_RoundDigit = roundDigit;
			mAssistant = assistant;
			initialize();
		}

		//
		[Obsolete("Use another constructor")]
		public GeoDataGridViewManager(DataGridView container,
										BindingList<GeoDataGridViewModel> bindingList,
										string[] invisiableColumnNames,
										Dictionary<string, Bitmap> iconImageList,
										double resolution,
										int roundDigit, MeasureAssistant assistant)
		{
			_GridViewContainer = container;
			_DataList = bindingList;
			_InvisiableColumnNames = invisiableColumnNames;
			_ImageList = iconImageList;
			_Resolution = resolution;
			_RoundDigit = roundDigit;
			mAssistant = assistant;

			var dfCoordinateModel = new List<RefCoordinate>() { new RefCoordinate(), };
			_refCoordinate = new BindingList<RefCoordinate>(dfCoordinateModel);

			//初始化 軸擺正
			_defaultRefSkew = new RefSkew();
			_currentSkewID = _defaultRefSkew.ID;
			var dfSkewModel = new List<RefSkew>() { _defaultRefSkew };
			_refSkew = new BindingList<RefSkew>(dfSkewModel);

			initialize();
		}

		#region 參考座標 ******************

		/// <summary>
		/// <para>**********************</para>
		/// 移除參考座標，並重設相依於此參考座標的資料列
		/// <para>**********************</para>
		/// </summary>
		/// <param name="coordinateID">CoordinateID as RecordID</param>
		/// <returns></returns>
		public bool RemoveRefCoordinate(string coordinateID)
		{
			bool removed = false;
			var coordinate = _refCoordinate.SingleOrDefault(p => p.ID == coordinateID);
			if (coordinate != null)
			{
				removed = _refCoordinate.Remove(coordinate);
				if (removed)
				{
					//重設相依於此參考座標的資料列
					foreach (var refModel in _DataList.Where(item => item.CoordinateID == coordinate.ID))
					{
						resetModelCoordinate(refModel);
					}

					//如果現在設定的 ID 是移除的 ID, 則現在的 ID 設為預設值
					if (_currentCoordinateID == coordinate.ID)
						_currentCoordinateID = "";
				}
			}
			return removed;
		}

		/// <summary>
		/// <para>**********************</para>
		/// 設定目前參考座標
		/// <para>**********************</para>
		/// </summary>
		/// <param name="modelRecordID"></param>
		public void SetRefCoordinate(string modelRecordID)
		{
			var exists = _refCoordinate.Any(p => p.ID == modelRecordID);
			if (exists)
				_currentCoordinateID = modelRecordID;
		}
		#endregion 參考座標 ***************

		#region ===== 軸擺正 ==========================================================
		/// <summary>
		/// <para>***********</para>
		/// 設定目前軸擺正參考
		/// <para>***********</para>
		/// </summary>
		/// <param name="modelRecordID"></param>
		public void SetSkew(string modelRecordID)
		{
			var skewModel = _refSkew.SingleOrDefault(p => p.ID == modelRecordID);
			if (skewModel != null)
			{
				_currentSkewID = skewModel.ID;
				//計算 skew
				var skew = getSkew(skewModel.ID);

				var selfModel = _DataList.SingleOrDefault(p => p.RecordID == modelRecordID);
				selfModel.Skew = 0;
				selfModel.SkewID = _defaultRefSkew.ID;
				selfModel.SkewName = _defaultRefSkew.Name;


				//設定目前所有的參考軸擺正為目前的軸擺正，除了自已
				var updateModels = _DataList.Where(p => p.RecordID != modelRecordID);
				foreach (var model in updateModels)
				{
					model.SkewID = skewModel.ID;
					model.SkewName = skewModel.Name;
					model.Skew = skew;
				}
				_GridViewContainer.Refresh();
			}
		}

		/// <summary>
		/// 計算軸擺正角度 (Degree)
		/// </summary>
		/// <param name="skewID">skewID as recordID</param>
		private double getSkew(string skewID)
		{
			var skewRadian = 0.0;

			//取得 model
			var model = _DataList.SingleOrDefault(p => p.RecordID == skewID);
			if (model != null)
			{
				//判斷 model 類型
				if (DistanceHelper.IsPointType(model.GeoType))
				{
					skewRadian = HMisc.AngleLx(0, 0, model.Row1, model.Col1);
				}
				else if (DistanceHelper.isLineType(model.GeoType))
				{
					if (model.Col1 <= model.Col2)
						skewRadian = HMisc.AngleLx(model.Row1, model.Col1, model.Row2, model.Col2);
					else
						skewRadian = HMisc.AngleLx(model.Row2, model.Col2, model.Row1, model.Col1);
				}
			}
			return skewRadian.HalconPhiToDegree();
		}
		/// <summary>
		/// 移除參考的軸擺正
		/// </summary>
		/// <param name="skewID"></param>
		/// <returns></returns>
		public bool RemoveRefSkew(string skewID)
		{
			bool removed = false;
			var removeSkew = _refSkew.SingleOrDefault(p => p.ID == skewID);
			if (removeSkew != null)
			{
				removed = _refSkew.Remove(removeSkew);
				if (removed)
				{
					//重設相依於此軸擺正的資料列
					foreach (var refModel in _DataList.Where(item => item.SkewID == removeSkew.ID))
					{
						resetModelSkew(refModel);
					}
					if (_currentSkewID == removeSkew.ID)
						_currentSkewID = "";
				}
			}
			return removed;
		}

		private void resetModelSkew(GeoDataGridViewModel refModel)
		{
			refModel.Skew = 0;
			refModel.SkewID = "";
			refModel.SkewName = "";
		}

		#endregion

		#region public APIs
		/// <summary>
		/// 設定欄位顯示名稱
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="headerText"></param>
		public void SetColumnHeaderText(string columnName, string headerText)
		{
			var column = _GridViewContainer.Columns[columnName];
			if (column != null)
			{
				column.HeaderText = headerText;
			}
		}

		/// <summary>
		/// 載入資料
		/// </summary>
		/// <param name="bindingList"></param>
		public void LoadRecord(BindingList<GeoDataGridViewModel> bindingList)
		{
			this.Clear();

			//重建擺正資訊
			reInitSkew(bindingList);

			//把資料全部加入
			foreach (var geoModel in bindingList)
			{
				if (geoModel.ROIModel != null)
				{
					//重建且重計算所有的 ROI
					this._DataList.Add(geoModel);
					var reloadROI = geoModel.ROIModel.MakeROI();
					reloadROI.ROIMeasureType = geoModel.GeoType;

					addTreeNode(reloadROI, geoModel);
					notifyRecordChanged(GeoDataGridViewNotifyType.ReloadData, reloadROI);
				}
				else
				{
					//加入舊的非 ROI 的資料列
					addMeasuredViewModel(geoModel);

					//更新skew
					var skewValue = getSkew(_currentSkewID);
					geoModel.SkewID = _currentSkewID;
					geoModel.Skew = skewValue;
				}
			}
			//取得所有的 Record, 並更新其相依的資料列
			foreach (var item in _DataList)
			{
				updateDependGeoObject(item);
			}

			//Restore 參考座標
			var coordinateIDs = bindingList.Where(p => !String.IsNullOrEmpty(p.CoordinateID))
				.Select(p => new RefCoordinate()
				{
					ID = p.CoordinateID,
					Name = p.CoordinateName,
					Desc = "",
				}).Distinct().ToList();

			//Clear	Coordinate, 保留 Default
			while (_refCoordinate.Count > 1)
			{
				_refCoordinate.RemoveAt(1);
			}
			foreach (var item in coordinateIDs)
			{
				_refCoordinate.Add(item);
			}
			this.Refresh();
		}

		/// <summary>
		/// <para>****************</para>
		/// 重建擺正資訊, 只有一個
		/// <para>****************</para>
		/// </summary>
		/// <param name="bindingList"></param>
		private void reInitSkew(BindingList<GeoDataGridViewModel> bindingList)
		{
			//Restore 參考擺正, 只留一個
			var skewIDs = bindingList.Where(p => !String.IsNullOrEmpty(p.SkewID))
				.Select(p => new RefSkew()
				{
					ID = p.SkewID,
					Name = p.SkewName,
					Desc = "",
				}).FirstOrDefault();
			if (skewIDs != null)
			{
				_refSkew.Clear();
				_refSkew.Add(skewIDs);
				_currentSkewID = _refSkew[0].ID;
			}
		}

		/// <summary>
		/// 設定 TreeView
		/// </summary>
		/// <param name="treeView"></param>
		/// <param name="treeViewImageList"></param>
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
			if (model == null || roi == null) return;

			var roiRecord = GetViewModel(roi.ID);
			var isAddNew = (roiRecord == null);
			GeoDataGridViewModel tmpROIRecord = transferROIToGeoDataViewModel(model, roi, isAddNew);
			var updateTreeNodeID = isAddNew ? tmpROIRecord.RecordID : roiRecord.RecordID;
			if (isAddNew)
			{
				addROIRecord(roi, tmpROIRecord);
			}
			else
			{
				updateROIRecord(roiRecord.RecordID, tmpROIRecord);
			}
			this.Refresh();
			setTreeNodeFocus(updateTreeNodeID);
			notifyRecordChanged(GeoDataGridViewNotifyType.UpdateData, tmpROIRecord);
		}

		/// <summary>
		/// 更新 GridView
		/// <para>檢查資料並標記顏色</para>
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
						Color cellColor = (!valid) ? Color.OrangeRed : Color.White;
						foreach (DataGridViewCell colorCell in row.Cells)
						{
							colorCell.Style.BackColor = cellColor;
						}
					}
				}
			}
			_GridViewContainer.Refresh();
		}

		/// <summary>
		/// 設定計算類型
		/// </summary>
		/// <param name="cType"></param>
		public void SetCalcuteType(CalcuteType cType)
		{
			DoCalculate = cType;
			_GridViewContainer.Columns[0].Visible = !(DoCalculate == CalcuteType.None);
		}
		/// <summary>
		/// 取消計算
		/// </summary>
		public void ResetCalcuteType()
		{
			DoCalculate = CalcuteType.None;
			var checkIdx = 0;
			foreach (var row in
				_GridViewContainer.Rows.Cast<DataGridViewRow>()
								.Where(p => Convert.ToBoolean(p.Cells[checkIdx].Value) == true)
								.ToArray())
			{
				row.Cells[checkIdx].Value = false;
			}
			_GridViewContainer.Columns[checkIdx].Visible = false;
		}

		/// <summary>
		/// 清除所有結果
		/// </summary>
		public void Clear()
		{
			_DataList.Clear();
			if (_TreeViewContainer != null)
			{
				_TreeViewContainer.Nodes.Clear();
			}
			notifyRecordChanged(GeoDataGridViewNotifyType.Clear, null);
		}

		/// <summary>
		/// 取得資料筆數
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// 設定量測單位 (um, mm, mil, inch)
		/// </summary>
		/// <param name="unit"></param>
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
			_GridViewContainer.Refresh();
		}

		/// <summary>
		/// 取得量測單位
		/// </summary>
		/// <returns></returns>
		public string GetUnit()
		{
			return _ExportUnit;
		}
		#endregion

		/// <summary>
		/// 新增有 ROI model 的資料列
		/// </summary>
		/// <param name="roi"></param>
		/// <param name="tmpGeoDataViewModel"></param>
		private void addROIRecord(ROI roi, GeoDataGridViewModel tmpGeoDataViewModel)
		{
			_DataList.Add(tmpGeoDataViewModel);
			addTreeNode(roi, tmpGeoDataViewModel);
		}

		/// <summary>
		/// 更新 ViewModel
		/// <para>有 ROI model 的資料列更新</para>
		/// </summary>
		/// <param name="recordID">RecordID</param>
		/// <param name="tmpGeoDataViewModel"></param>
		private void updateROIRecord(string recordID, GeoDataGridViewModel tmpGeoDataViewModel)
		{
			var rowModel = _DataList.SingleOrDefault(p => p.RecordID == recordID);
			if (rowModel != null)
			{
				//
				var newDistance = tmpGeoDataViewModel.Distance;
				double coordinateCol, coordinateRow;
				getRefCoordinate(rowModel, out coordinateCol, out coordinateRow);

				//參考座標異動
				rowModel.CoordinateRow = coordinateRow;
				rowModel.CoordinateCol = coordinateCol;

				rowModel.Col1 = tmpGeoDataViewModel.Col1;
				rowModel.Row1 = tmpGeoDataViewModel.Row1;
				rowModel.Distance = newDistance;
				rowModel.WorldDistance = pixelToRealWorldValue(newDistance);
				rowModel.Col2 = tmpGeoDataViewModel.Col2;
				rowModel.Row2 = tmpGeoDataViewModel.Row2;
				rowModel.ROIModel = tmpGeoDataViewModel.ROIModel;

				//更新參考我的座標的物件
				updateDependOnCoordinateObjects(rowModel);

				//更新參考我作為軸擺正的物件
				updateDependOnSkewObjects(rowModel);

				//更新與此Geo 相關的 Geo Objects
				updateDependGeoObject(rowModel);

			}
		}

		/// <summary>
		/// 更新參考我作為軸擺正的物件
		/// </summary>
		/// <param name="rowModel"></param>
		private void updateDependOnSkewObjects(GeoDataGridViewModel rowModel)
		{
			var isSkewModel = _refSkew.Any(p => p.ID == rowModel.RecordID);
			if (isSkewModel)
			{
				//參考我的物件們
				var models = _DataList.Where(p => p.SkewID == rowModel.RecordID);
				var skewValue = getSkew(rowModel.RecordID);
				foreach (var model in models)
				{
					model.Skew = skewValue;
				}
			}
		}
		/// <summary>
		/// 更新參考我的座標的物件
		/// </summary>
		/// <param name="coordinateModel"></param>
		private void updateDependOnCoordinateObjects(GeoDataGridViewModel coordinateModel)
		{
			foreach (var model in _DataList.Where(p => p.CoordinateID == coordinateModel.RecordID))
			{
				model.CoordinateRow = model.Row1 - coordinateModel.Row1;
				model.CoordinateCol = model.Col1 - coordinateModel.Col1;
			}
		}

		/// <summary>
		/// 設定 TreeNode Focus
		/// </summary>
		/// <param name="id"></param>
		/// <param name="searchChild">搜尋子節點</param>
		private void setTreeNodeFocus(string id, bool searchChild)
		{
			if (_TreeViewContainer != null)
			{
				var nodes = _TreeViewContainer.Nodes.Find(id, searchChild);
				if (nodes.Length > 0)
				{
					_TreeViewContainer.Focus();
					_TreeViewContainer.SelectedNode = nodes[0];
				}
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
			if (model.GeoType == MeasureType.PointCircle || model.GeoType == MeasureType.Circle)
			{
				model.Distance = model.Distance * _circleDistanceSetting;
				model.WorldDistance = model.WorldDistance * _circleDistanceSetting;
			}
			_DataList.Add(model);
			addMeasuredTreeNode(model);
		}

		/// <summary>
		/// 新增量測記錄 (distance, symmetrylin, angle, 3pointCircle) 
		/// <para>Tree View</para>
		/// </summary>
		/// <param name="geoModel"></param>
		private void addMeasuredTreeNode(GeoDataGridViewModel geoModel)
		{
			if (_TreeViewContainer != null)
			{
				//addSubNodeBaseOnROIMeasureElement(geoModel);
				var parentNode = getGeoTreeNode(geoModel);
				_TreeViewContainer.Nodes.Add(parentNode);
				//Depend models as child node
				foreach (var recordID in geoModel.DependGeoRowNames)
				{
					//depend model
					var model = _DataList.SingleOrDefault(p => p.RecordID == recordID);
					if (model != null)
					{
						var childNode = getGeoTreeNode(model);
						parentNode.Nodes.Add(childNode);
						//childNode.
						HideCheckBox(_TreeViewContainer, childNode);
					}
				}
				_TreeViewContainer.Focus();
				_TreeViewContainer.SelectedNode = parentNode;
			}
		}

		/// <summary>
		/// 以ROI 元素為主的呈現結構 (量測元素會加入在 ROI 元素下)
		/// </summary>
		/// <param name="geoModel"></param>
		private void addSubNodeBaseOnROIMeasureElement(GeoDataGridViewModel geoModel)
		{
			///* // 
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
			// */
		}

		private string _treeNodeDispNamePattern = "{0:d2} {1}";
		private TreeNode getGeoTreeNode(GeoDataGridViewModel geoModel)
		{
			var number = _DataList.Count;
			var geoNodeDisplayName = (String.IsNullOrEmpty(geoModel.Name)) ?
									String.Format(_treeNodeDispNamePattern, number, geoModel.GeoType) : geoModel.Name;
			var geoImageKey = geoModel.GeoType.ToString();
			TreeNode geoNode = new TreeNode()
			{
				Name = geoModel.RecordID,
				Text = geoNodeDisplayName,
				ImageKey = geoImageKey,
				SelectedImageKey = geoImageKey,
				Tag = geoModel.ROIID,
			};
			if (_geoContextMenuStrip != null)
			{
				geoNode.ContextMenuStrip = _geoContextMenuStrip;
			}
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
				TreeNode roiNode = new TreeNode()
				{
					Name = geoModel.RecordID,
					Text = roiNodeName,
					ImageKey = roiImageKey,
					SelectedImageKey = roiImageKey,
					Checked = activeROI.Visiable,
					Tag = activeROI,
				};
				if (_geoContextMenuStrip != null)
				{
					roiNode.ContextMenuStrip = _geoContextMenuStrip;
				}

				_TreeViewContainer.Nodes.Add(roiNode);
				_TreeViewContainer.Focus();
				_TreeViewContainer.SelectedNode = roiNode;
			}

		}

		/// <summary>
		/// 取得 Model
		/// <para>先比對 RecordID，若沒有再比對 ROIID</para>
		/// </summary>
		/// <param name="recordOrROIID"></param>
		/// <returns></returns>
		private GeoDataGridViewModel getModel(string recordOrROIID)
		{
			var model = _DataList.SingleOrDefault(p => p.RecordID == recordOrROIID);
			if (model == null)
				model = _DataList.SingleOrDefault(p => p.ROIID == recordOrROIID);
			return model;
		}

		/// <summary>
		/// 組成 GeoDataViewModel
		/// <para>把 ROI 轉成GeoDataViewModel</para>
		/// </summary>
		/// <param name="roiIndex">active ROI index</param>
		/// <param name="viewModel">MeasureViewModel</param>
		/// <param name="roi">ROI</param>
		/// <returns></returns>
		private GeoDataGridViewModel transferROIToGeoDataViewModel(MeasureViewModel viewModel, ROI roi, bool isAddNew)
		{
			var number = _DataList.Count + 1;
			var measureName = number.ToString("d2") + " " + roi.ROIMeasureType;
			var exportUnit = roi.ROIMeasureType == MeasureType.Angle ? "Angle" :
							roi.ROIMeasureType == MeasureType.Point ? "" : _ExportUnit;

			var distance = (viewModel.Distance != null && viewModel.Distance.TupleLength() > 0) ?
						viewModel.Distance.D : 0.0;

			//圓類型, 距離為半徑 or 直徑
			var isCircleType = (viewModel.GeoType == MeasureType.PointCircle || viewModel.GeoType == MeasureType.Circle);
			distance = (isCircleType) ? distance * _circleDistanceSetting : distance;

			var curCoordinate = _refCoordinate.SingleOrDefault(p => p.ID == _currentCoordinateID);
			var curSkew = _refSkew.SingleOrDefault(p => p.ID == _currentSkewID);
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
				Distance = distance,
				WorldDistance = pixelToRealWorldValue(distance),

				Col2 = (viewModel.Col2 != null && viewModel.Col2.TupleLength() > 0) ?
						viewModel.Col2.D : -1.0,

				Row2 = (viewModel.Row2 != null && viewModel.Row2.TupleLength() > 0) ?
						viewModel.Row2.D : -1.0,
				Selected = false,
				Unit = exportUnit,
				GeoType = roi.ROIMeasureType,
			};
			var arcRoi = roi as SmartArc;
			if (arcRoi != null)
			{
				var roiModelData = arcRoi.getModelData();
				geoModel.StartPhi = roiModelData[3];
				geoModel.EndPhi = roiModelData[4];
			}

			if (isAddNew)
			{
				if (curCoordinate != null)
				{
					geoModel.CoordinateID = curCoordinate.ID;
					geoModel.CoordinateName = curCoordinate.Name;
				}
			}
			//設定參考座標值
			double refCoordinateCol, refCoordinateRow;
			getRefCoordinate(geoModel, out refCoordinateCol, out refCoordinateRow);
			geoModel.CoordinateCol = refCoordinateCol;
			geoModel.CoordinateRow = refCoordinateRow;

			//計算軸擺正
			if (curSkew != null)
			{
				geoModel.SkewID = curSkew.ID;
				geoModel.SkewName = curSkew.Name;
				geoModel.Skew = getSkew(geoModel.SkewID);
			}
			return geoModel;
		}
		/// <summary>
		/// 我的位置 - 我參考的座標的位置
		/// <para>****取得參考座標位置****</para>
		/// </summary>
		/// <param name="geoModel">我</param>
		/// <param name="coordinateCol"></param>
		/// <param name="coordinateRow"></param>
		private void getRefCoordinate(GeoDataGridViewModel geoModel, out double coordinateCol, out double coordinateRow)
		{
			coordinateCol = geoModel.Col1;
			coordinateRow = geoModel.Row1;
			var refCoordinateModel = _DataList.SingleOrDefault(p => p.RecordID == geoModel.CoordinateID);
			if (refCoordinateModel != null)
			{
				//影像座標 - 參考座標
				coordinateCol = geoModel.Col1 - refCoordinateModel.Col1;
				coordinateRow = geoModel.Row1 - refCoordinateModel.Row1;
			}
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
			var key = roi.ROIMeasureType.ToString();
			if (_ImageList.ContainsKey(key))
			{
				iconImage = _ImageList[key];
			}
			return iconImage;
		}

		/// <summary>
		/// <para>*******************************</para>
		/// 更新與「我」相依的物件
		/// <para>Ex: 物件相依關系如下</para>
		/// <para> A </para>
		/// <para> B </para>
		/// <para> C 由 A, B 構成 </para>
		/// <para>--------------------------------------</para>
		/// <para> 則 A 或 B 異動時, 其相依物件 C 需被更新 </para>
		/// <para>*******************************</para>
		/// </summary>
		/// <param name="geoParent">傳入小孩的父親，即目前被異動的物件</param>
		private void updateDependGeoObject(GeoDataGridViewModel geoParent)
		{
			//去除自已以外，其他有相依的資料列們
			var hasDependObjectRecords = _DataList.Where(p => p.DependGeoRowNames != null
													&& p.RecordID != geoParent.RecordID);

			//與本列相關的資料列
			var oneToManyGeoList = hasDependObjectRecords.Where(q => q.DependGeoRowNames.Contains(geoParent.RecordID));

			foreach (var geoModel in oneToManyGeoList)
			{
				var firstID = geoModel.DependGeoRowNames.Length > 0 ? geoModel.DependGeoRowNames[0] : "";
				var secondID = geoModel.DependGeoRowNames.Length > 1 ? geoModel.DependGeoRowNames[1] : "";
				var thirdID = geoModel.DependGeoRowNames.Length > 2 ? geoModel.DependGeoRowNames[2] : "";
				var firstModel = _DataList.SingleOrDefault(p => p.RecordID == firstID);
				var secondModel = _DataList.SingleOrDefault(p => p.RecordID == secondID);
				var thirdModel = _DataList.SingleOrDefault(p => p.RecordID == thirdID);
				GeoDataGridViewModel newModel = null;
				switch (geoModel.GeoType)
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
				reAssignModelValue(geoModel, newModel);

				//更新參考"我"座標的物件
				updateDependOnCoordinateObjects(geoModel);

				//更新參考我作為軸擺正的物件
				updateDependOnSkewObjects(geoModel);
			}
		}

		private void reAssignModelValue(GeoDataGridViewModel oldModel, GeoDataGridViewModel newModel)
		{
			var lineTypes = new MeasureType[] { MeasureType.Distance, 
												MeasureType.DistanceX, 
												MeasureType.DistanceY, 
												MeasureType.SymmetryLine };

			var isCrossPoint = (oldModel.GeoType == MeasureType.CrossPoint);
			var isCircleType = (oldModel.GeoType == MeasureType.Circle || oldModel.GeoType == MeasureType.PointCircle);
			if (newModel == null)
			{
				modelSetToDefault(oldModel);
			}
			else
			{
				oldModel.Row1 = newModel.Row1;
				oldModel.Col1 = newModel.Col1;
				if (!isCrossPoint)
				{
					if (lineTypes.Contains(oldModel.GeoType))
					{
						oldModel.Row2 = newModel.Row2;
						oldModel.Col2 = newModel.Col2;
					}
					oldModel.Distance = (isCircleType) ? newModel.Distance * _circleDistanceSetting : newModel.Distance;
					oldModel.WorldDistance = (isCircleType) ? newModel.WorldDistance * _circleDistanceSetting : newModel.WorldDistance;
				}
			}
			if (isCrossPoint)
			{
				updateDependGeoObject(oldModel);
			}
		}

		/// <summary>
		/// <para>***************</para>
		/// 位置及距離數值設為 -1
		/// <para>***************</para>
		/// </summary>
		/// <param name="model"></param>
		private static void modelSetToDefault(GeoDataGridViewModel model)
		{
			model.Row1 = model.Col1 = -1;
			model.Row2 = model.Col2 = -1;
			model.Distance = model.WorldDistance = -1;
			model.CoordinateCol = model.CoordinateRow = -1;
		}

		private void initialize()
		{
			initGridView();

			//Default value
			_ExportUnit = "mm";


		}

		private void initGridView()
		{
			if (_GridViewContainer != null)
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
				_GridViewContainer.MouseDown += _GridViewContainer_MouseDown;

				//設定 Row 的 ContextMenu
				_GridViewContainer.RowsAdded += _GridViewContainer_RowsAdded;
			}
		}

		/// <summary>
		/// <para>************</para>
		/// 新增資料列時，設定該列的 ContextMenuStrip
		/// <para>************</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _GridViewContainer_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			var dgv = sender as DataGridView;
			var row = dgv.Rows[e.RowIndex];
			if (row != null)
			{
				row.ContextMenuStrip = _geoContextMenuStrip;
			}
		}

		/// <summary>
		/// <para>*******</para>
		/// 設定右鍵點擊, 選取整列
		/// <para>*******</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _GridViewContainer_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var dgv = sender as DataGridView;
				DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
				if (hit.Type == DataGridViewHitTestType.Cell)
				{
					if (!dgv.Rows[hit.RowIndex].Selected)
					{
						dgv.ClearSelection();
						dgv.CurrentCell = dgv[hit.ColumnIndex, hit.RowIndex];//.Rows[hit.RowIndex].Cells[hit.ColumnIndex];//配合滑鼠點下所選擇該列
						dgv.Rows[hit.RowIndex].Selected = true;
					}
				}
			}
		}

		/// <summary>
		/// <para>********</para>
		/// 點擊 DataGridView Cell, 同步選取 TreeView Node
		/// <para>********</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _GridViewContainer_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// GridViewCell 與 TreeView 互動
			if (DoCalculate == CalcuteType.None && _TreeViewContainer != null)
			{
				var gridView = sender as DataGridView;
				if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
				{
					DataGridViewRow row = gridView.Rows[e.RowIndex];
					var rowCell = row.Cells["RecordID"];//.Value as ROIViewModel;
					if (rowCell != null)
					{
						var recordID = rowCell.Value.ToString();
						setTreeNodeFocus(recordID, true);
					}
				}
			}
		}

		/// <summary>
		/// <para>*******</para>
		/// 編輯結束，變更 TreeNode 顯示名稱
		/// <para>*******</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _GridViewContainer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (_TreeViewContainer != null)
			{
				var gridview = sender as DataGridView;
				try
				{
					var columnName = gridview.Columns[e.ColumnIndex].Name;
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
						//變更座標系統名稱
						//recordID						
						var coordinate = _refCoordinate.SingleOrDefault(p => p.ID == recordID);
						var isCoordinateNameChanged = (coordinate != null);
						if (isCoordinateNameChanged)
						{
							coordinate.Name = newCellValue;
							foreach (var item in _DataList.Where(p => p.CoordinateID == coordinate.ID))
							{
								item.CoordinateName = coordinate.Name;
							}
							this.Refresh();
						}

						//變更軸擺正名稱
						var curSkew = _refSkew.SingleOrDefault(p => p.ID == recordID);
						var isCurSkewNameChanged = (curSkew != null);
						if (isCurSkewNameChanged)
						{
							curSkew.Name = newCellValue;
							foreach (var item in _DataList.Where(p => p.SkewID == curSkew.ID))
							{
								item.SkewName = curSkew.Name;
							}
							this.Refresh();
						}

						if (isCoordinateNameChanged)
							notifyRecordChanged(GeoDataGridViewNotifyType.Coordinate_NameChanged, isCoordinateNameChanged);
						if (isCurSkewNameChanged)
							notifyRecordChanged(GeoDataGridViewNotifyType.Skew_NameChanged, isCurSkewNameChanged);
					}
				}
				catch (Exception ex)
				{
					Hanbo.Log.LogManager.Error(ex);
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
				//參考我的座標的資料列
				RemoveRefCoordinate(model.RecordID);

				RemoveRefSkew(model.RecordID);

				//刪除相依的 Model
				var modelROIID = model.ROIID;
				var deletedRows = _DataList.Where(p => relatedModelIDs.Contains(p.RecordID)).ToList();
				for (int i = 0; i < deletedRows.Count; i++)
				{
					var row = deletedRows[i];
					var recordID = row.RecordID;
					RemoveRefCoordinate(row.RecordID);
					RemoveRefSkew(row.RecordID);
					_DataList.Remove(row);
				}
				_DataList.Remove(model);
				notifyRecordChanged(GeoDataGridViewNotifyType.DeleteRow, modelROIID);
			}
			return confirmDelete;
		}

		/// <summary>
		/// 將 Model 的參考座標清空, 設為 Default
		/// </summary>
		/// <param name="model"></param>
		private void resetModelCoordinate(GeoDataGridViewModel model)
		{
			model.CoordinateID = model.CoordinateName = "";
			model.CoordinateRow = model.Row1;
			model.CoordinateCol = model.Col1;
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
				var doEvaluateResolution = (DoCalculate == CalcuteType.EvaluateResolution);

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
				else if (doEvaluateResolution)
				{
					evaluateResolution(checkRows);
				}

				//打完收工
				if (doDistance || doPoint3ToCircle || doAngle || doSymetryLine || doCrossPoint || doDistanceX || doDistanceY)
				{
					var columnIndex = e.ColumnIndex;
					restoreGridviewState(columnIndex, gridView, checkRows);
					notifyRecordChanged(GeoDataGridViewNotifyType.Calculated, null);
				}
			}
		}

		private void evaluateResolution(DataGridViewRow[] checkRows)
		{
			double sum = 0.0;
			double count = 0.0;
			foreach (DataGridViewRow row in checkRows)
			{
				var pixelCell = row.Cells["Distance"];
				var realCell = row.Cells["Normal"];
				if (pixelCell != null && realCell != null && pixelCell.Value != null && realCell.Value != null)
				{
					Double pixelValue, realValue;

					var pixelValid = Double.TryParse(pixelCell.Value.ToString(), out pixelValue);
					var realValid = Double.TryParse(realCell.Value.ToString(), out realValue);
					if (pixelValid && realValid)
					{
						//計算實際值
						var resolution = realValue / pixelValue;
						sum += resolution;
						count++;
					}
				}
			}
			if (count > 0)
			{
				var avg = Math.Round(sum / count, _RoundDigit);
				notifyRecordChanged(GeoDataGridViewNotifyType.UpdateData, avg);
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
			//models
			var pA = _DataList[checkRows[0].Index];
			var pB = _DataList[checkRows[1].Index];

			//measure
			var distanceXModel = makeDistanceXGeoDataViewModel(pA, pB);

			//done
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
					var curSkew = _refSkew.SingleOrDefault(p => p.ID == _currentSkewID);
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
						SkewID = _currentSkewID,
					};
					if (curSkew != null)
					{
						result.SkewID = _currentSkewID;
						result.SkewName = curSkew.Name;
						result.Skew = getSkew(_currentSkewID);
					}
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
		/// <para>取消 Checkbox</para>
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

		private int _circleDistanceSetting = ConfigurationHelper.GetCircleDistanceSetting();
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
			_TreeViewContainer.NodeMouseClick += _TreeViewContainer_NodeMouseClick;
		}

		/// <summary>
		/// <para>****************</para>
		/// 設定按下滑鼠右鍵時，變換選取的節點
		/// <para>(點 Checkbox 不會觸發此事件)</para>
		/// <para>****************</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _TreeViewContainer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			var tv = sender as TreeView;
			tv.SelectedNode = e.Node;
		}

		void _TreeViewContainer_AfterCheck(object sender, TreeViewEventArgs e)
		{
			var roi = e.Node.Tag as ROI;
			if (roi != null)
			{
				roi.Visiable = e.Node.Checked;
				notifyRecordChanged(GeoDataGridViewNotifyType.TreeView_AfterCheck, e);
			}
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
			if (node.Handle != null)
			{
				try
				{
					tvi.hItem = node.Handle;
					tvi.mask = TVIF_STATE;
					tvi.stateMask = TVIS_STATEIMAGEMASK;
					tvi.state = 0;
					SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
				}
				catch (Exception ex)
				{
					Hanbo.Log.LogManager.Error(ex);
				}
			}
		}
		#endregion
		#endregion
	}
}
