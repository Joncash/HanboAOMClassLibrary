using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//===
using ViewROI;
using Hanbo.WindowControlWrapper;
using MeasureModule;
namespace Hanbo.WindowsFormsControlLibrary.UserControls
{

	/*
	 預期
	 * 1。可依功能設定檔來顯示功能按鈕及TabPage (進階, Option)
	 * 2。常用功能按鈕 (進階, Option)
	 * 3。使用者設定檔 (進階, Option)
	 */

	//[Designer(typeof(GeometryMeasureControlDesigner))]
	public partial class GeometryMeasureControl : UserControl
	{
		private ROIController _roiController;
		private HWndCtrl _mView;
		private GeoDataGridViewManager _geoManager;
		public GeometryMeasureControl()
		{
			InitializeComponent();
		}
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="roiController"></param>
		public void Init(ROIController roiController, HWndCtrl mView, GeoDataGridViewManager geoManager)
		{
			_roiController = roiController;
			_mView = mView;
			_geoManager = geoManager;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//幾何元素 - 點測量
			_roiController.setROIShape(new ROIRectangle2());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//幾何元素 - 線測量
			_roiController.setROIShape(new ROIRectangle2()
			{
				ROIMeasureType = MeasureType.Line
			});
		}

		private void button3_Click(object sender, EventArgs e)
		{
			//幾何元素 - 圓測量
			_roiController.setROIShape(new ROICircle());
		}

		private void button4_Click(object sender, EventArgs e)
		{
			//幾何元素 (點-smart)
			this._mView.EnableZoomContinue();
			_roiController.setROIShape(new ViewROI.SmartROIs.SmartPoint());
		}

		private void button5_Click(object sender, EventArgs e)
		{
			//幾何元素 (線-smart)
			this._mView.EnableZoomContinue();
			_roiController.setROIShape(new ViewROI.SmartROIs.PointsLine());
		}

		private void button6_Click(object sender, EventArgs e)
		{
			//Distance
			_geoManager.SetCalcuteType(CalcuteType.Distance);
		}

		private void button7_Click(object sender, EventArgs e)
		{
			//Circle
			_geoManager.SetCalcuteType(CalcuteType.Point3ToCircle);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			//Symetry
			_geoManager.SetCalcuteType(CalcuteType.SymmetryLine);
		}

		private void button9_Click(object sender, EventArgs e)
		{
			//angle
			_geoManager.SetCalcuteType(CalcuteType.Angle);
		}

		private void button10_Click(object sender, EventArgs e)
		{
			//Crosspoint
			_geoManager.SetCalcuteType(CalcuteType.CrossPoint);
		}

		private void button11_Click(object sender, EventArgs e)
		{
			//Distance X
			_geoManager.SetCalcuteType(CalcuteType.DistanceX);
		}

		private void button12_Click(object sender, EventArgs e)
		{
			//Distance Y
			_geoManager.SetCalcuteType(CalcuteType.DistanceY);
		}
		/*
		#region Designer===========================================================
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabControl MeasureTagControl { get { return this.MeasureFuncTabControl; } }
		#endregion
		*/
	}
}
