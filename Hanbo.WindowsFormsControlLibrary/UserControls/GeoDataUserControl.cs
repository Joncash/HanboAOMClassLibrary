using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//=================================
using Hanbo.WindowControlWrapper;
using ViewROI;
using Hanbo.AOM.Configuration;
using MeasureModule.ViewModel;
using MeasureModule;

namespace Hanbo.WindowsFormsControlLibrary.UserControls
{
	/// <summary>
	/// <para>**************</para>
	/// 量測資料 User Control
	/// <para>**************</para>
	/// </summary>
	public partial class GeoDataUserControl : UserControl
	{
		#region ***** 參考座標 *****
		private BindingList<RefCoordinate> _dfCoordinateBindingList;
		#endregion

		private GeoDataGridViewManager _geoManager;
		private ROIController _roiController;
		public GeoDataUserControl()
		{
			InitializeComponent();
		}
		public bool Init(ROIController roicontroller, MeasureAssistant mAssistant
						, double resolution, int roundDigit, string[] inVisibleFields)
		{
			var success = true;
			var model = new List<RefCoordinate>() { 
								new RefCoordinate() { ID = "", Name = "Default", Desc = "影像座標" },
						};
			_dfCoordinateBindingList = new BindingList<RefCoordinate>(model);

			_geoManager = new GeoDataGridViewManager(GeoDataGridView, GeoContextMenuStrip
											, _dfCoordinateBindingList, inVisibleFields
											, ConfigurationMM.GeoImageDictionary
											, resolution, roundDigit, mAssistant);
			_roiController = roicontroller;

			initGeoContextMenu();
			initCoordinateComboBox();
			initLengthUnitComboBox();
			initClearButton();
			initExportButton();
			initGeoTreeView();
			return success;
		}
		public GeoDataGridViewManager GetGeoDataGridViewManager()
		{
			return _geoManager;
		}

		private void initGeoContextMenu()
		{
			GeoContextMenuStrip.Opening += GeoContextMenuStrip_Opening;
		}
		private void SetCoordinateSystemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			var menuContext = menuItem.GetCurrentParent() as ContextMenuStrip;
			GeoDataGridViewModel currentModel = getContextMenuGeoDataGridViewModel(menuContext);
			if (currentModel != null)
			{
				var confirmText = String.Format("「{0}」 將設為參考座標？", currentModel.Name);
				var confirmed = MessageBox.Show(confirmText, "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
				if (confirmed)
				{
					var isExists = _dfCoordinateBindingList.Any(p => p.ID == currentModel.RecordID);
					if (!isExists)
					{
						var nwCoordinate = new RefCoordinate()
						{
							ID = currentModel.RecordID,
							Desc = "",
							Name = currentModel.Name,
						};
						_dfCoordinateBindingList.Add(nwCoordinate);
						CoordinateComboBox.SelectedItem = nwCoordinate;
					}
				}
			}
		}
		private void GeoContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			var pointTypes = new MeasureType[] { MeasureType.Point, MeasureType.Circle, MeasureType.PointCircle, MeasureType.CrossPoint };
			var menuContext = sender as ContextMenuStrip;
			var name = "SetCoordinateSystemToolStripMenuItem";
			GeoDataGridViewModel model = getContextMenuGeoDataGridViewModel(menuContext);
			if (model != null)
			{
				var menuItem = menuContext.Items[name];//.OfType<ToolStripMenuItem>().SingleOrDefault(p => p.Name == name);
				var isPointType = (pointTypes.Contains(model.GeoType));
				menuItem.Enabled = isPointType;
			}
		}
		private GeoDataGridViewModel getContextMenuGeoDataGridViewModel(ContextMenuStrip menuContext)
		{
			GeoDataGridViewModel model = null;
			var dgv = menuContext.SourceControl as DataGridView;
			if (dgv != null)
			{
				var index = dgv.CurrentRow.Index;
				model = _geoManager.GetAllRecord()[index];
			}

			var tv = menuContext.SourceControl as TreeView;
			if (tv != null)
			{
				var node = tv.SelectedNode;
				model = _geoManager.GetAllRecord().SingleOrDefault(p => p.RecordID == node.Name);
			}
			return model;
		}

		private void initGeoTreeView()
		{
			_geoManager.SetTreeViewControl(GeoTreeView, ConfigurationMM.GeoTreeViewImageList);
		}

		private void initExportButton()
		{
			ExportButton.Click += (sender, e) =>
			{
				//保存報告
				if (_geoManager.GetRecordCount() == 0)
				{
					MessageBox.Show(Hanbo.Resources.Resource.Message_NoData);
					return;
				}

				//1。saveFileDialog
				var saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "csv (*.csv)|*.csv|all files (*.*)|*.*";
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.InitialDirectory = @"D:\";

				if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var fpath = saveFileDialog.FileName;
					var sb = new StringBuilder();
					var rowIndex = 0;
					//2. Save
					var exportRecords = _geoManager.GetAllRecord().Select(p => p).ToList();
					foreach (var model in exportRecords)
					{
						rowIndex++;
						var line = "";
						if (rowIndex == 1)
						{
							line = "流水號,元素,Start.X (pixel),Start.Y (pixel),End.X (pixel),End.Y(pixel),長度(pixel),長度 (" + _geoManager.GetUnit() + ")";
							sb.AppendLine(line);
						}
						line = String.Format(@"{0},{1},{2},{3},{4},{5},{6},{7}",
							rowIndex, model.GeoType, model.Col1, model.Row1, model.Col2, model.Row2, model.Distance, model.WorldDistance);
						sb.AppendLine(line);
					}
					File.WriteAllText(fpath, sb.ToString(), Encoding.UTF8);
				}
			};

		}

		private void initClearButton()
		{
			ClearListButton.Click += (sender, e) =>
			{
				if (MessageBox.Show(Hanbo.Resources.Resource.Message_DeleteNotice
								, Hanbo.Resources.Resource.Message_Warning
								, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
				{
					_geoManager.Clear();
					_roiController.reset();
					restoreToDefaultCoordinate();
				}
			};
		}

		private void initLengthUnitComboBox()
		{
			UnitComboBox.ComboBox.DataSource = Hanbo.AOM.Configuration.ConfigurationMM.LengthUnits;
			UnitComboBox.ComboBox.ValueMember = "Value";
			UnitComboBox.ComboBox.DisplayMember = "Name";

			UnitComboBox.SelectedIndex = 1;
			UnitComboBox.SelectedIndexChanged += (sender, e) =>
			{
				var cbox = sender as ToolStripComboBox;
				if (cbox != null)
				{
					var box = cbox.ComboBox;
					var value = (string)box.SelectedValue;
					if (_geoManager != null)
					{
						_geoManager.SetUnit(value);
					}
				}
			};
		}

		private void initCoordinateComboBox()
		{
			//arrange			
			BindingSource ds = new BindingSource() { DataSource = _dfCoordinateBindingList };

			//assign
			CoordinateComboBox.ComboBox.DataSource = ds;
			CoordinateComboBox.ComboBox.ValueMember = "ID";
			CoordinateComboBox.ComboBox.DisplayMember = "Name";
			CoordinateComboBox.ComboBox.SelectedIndex = 0;
			CoordinateComboBox.ComboBox.SelectedIndexChanged += (sender, e) =>
			{
				var cbox = sender as ComboBox;
				if (cbox != null)
				{
					//var box = cbox.ComboBox;
					var value = (string)cbox.SelectedValue;
					if (_geoManager != null)
						_geoManager.SetRefCoordinate(value);
				}
			};
		}

		private void restoreToDefaultCoordinate()
		{
			var model = new RefCoordinate() { ID = "", Name = "Default", Desc = "影像座標" };
			_dfCoordinateBindingList.Clear();
			_dfCoordinateBindingList.Add(model);
			BindingSource ds = new BindingSource() { DataSource = _dfCoordinateBindingList };
			CoordinateComboBox.ComboBox.DataSource = ds;
		}

		public void RefreshCoordinate()
		{
			var originalSelectedIndex = CoordinateComboBox.ComboBox.SelectedIndex;
			BindingSource ds = new BindingSource() { DataSource = _dfCoordinateBindingList };
			CoordinateComboBox.ComboBox.DataSource = ds;
			var selectedIdx = (originalSelectedIndex <= _dfCoordinateBindingList.Count) ?
								originalSelectedIndex : 0;
			CoordinateComboBox.ComboBox.SelectedIndex = selectedIdx;
		}
	}
}
