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

		//
		private bool _ckLock;
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
			if (_roiController != null)
			{
				_roiController.NotifyRCObserver += new IconicDelegate(resetFinalCheckedBox);
			}
		}

		/// <summary>
		/// 全部設為 Unchecked
		/// </summary>
		public void ResetAllCheckButton()
		{
			//ResetROI
			if (_roiController != null && _mView != null)
			{
				this._mView.DisableZoomContinue();
				_roiController.resetROI();
			}
			if (_geoManager != null)
			{
				_geoManager.ResetCalcuteType();
			}

			_ckLock = true;
			foreach (var cbox in MM_TabPage1_FlowPanel.Controls.OfType<CheckBox>())
			{
				cbox.Checked = false;
			}
			_ckLock = false;
		}

		/// <summary>
		/// 重置所有的幾何元素量測按鈕
		/// </summary>
		/// <param name="val"></param>
		private void resetFinalCheckedBox(int val)
		{
			if (val == ROIController.EVENT_CREATED_ROI)
			{
				checkBox1.Checked = false;
				checkBox2.Checked = false;
				checkBox3.Checked = false;
				checkBox4.Checked = false;
				checkBox5.Checked = false;
				checkBox13.Checked = false;
				checkBox14.Checked = false;
			}
		}
		/// <summary>
		/// <para>******</para>
		/// 檢查並執行 ResetROI, 若 Reset, 則傳為 true
		/// <para>******</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <returns></returns>
		private bool resetROIDoer(object sender)
		{
			bool doResetROI = !(sender as CheckBox).Checked;
			this._mView.DisableZoomContinue();
			try
			{
				if (doResetROI)
				{
					_roiController.resetROI();
				}
				else
				{
					exclusiveCheckbox(sender);
				}

				//如果按下 ROI, 則重置量測設定
				if (!doResetROI && _geoManager != null)
				{
					_geoManager.ResetCalcuteType();
				}
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error("GeometryMeasureControl Error");
				Hanbo.Log.LogManager.Error(ex);
			}
			return doResetROI;
		}
		private void exclusiveCheckbox(object sender)
		{
			_ckLock = true;
			foreach (var cbox in MM_TabPage1_FlowPanel.Controls.OfType<CheckBox>().Where(p => p != sender))
			{
				cbox.Checked = false;
			}
			_ckLock = false;
		}
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 - 點測量
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				_roiController.setROIShape(new ROIRectangle2());
			}
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 - 線測量
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				_roiController.setROIShape(new ROIRectangle2()
				{
					ROIMeasureType = MeasureType.Line
				});
			}
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 - 圓測量
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				_roiController.setROIShape(new ROICircle());
			}
		}

		private void checkBox4_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 (點-smart)
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				this._mView.EnableZoomContinue();
				_roiController.setROIShape(new ViewROI.SmartROIs.SmartPoint());
			}
		}

		private void checkBox5_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 (線-smart)
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				this._mView.EnableZoomContinue();
				_roiController.setROIShape(new ViewROI.SmartROIs.SmartLine());
			}
		}

		private void checkBox13_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 (圓-smart)
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				this._mView.EnableZoomContinue();
				_roiController.setROIShape(new ViewROI.SmartROIs.SmartCircle());
			}
		}

		private void checkBox14_CheckedChanged(object sender, EventArgs e)
		{
			//幾何元素 (弧-smart)
			if (_ckLock) return;
			if (!resetROIDoer(sender))
			{
				this._mView.EnableZoomContinue();
				_roiController.setROIShape(new ViewROI.SmartROIs.SmartArc());
			}
		}

		private bool resetCalculateTypeDoer(object sender)
		{
			var doResetCalculateType = !(sender as CheckBox).Checked;
			try
			{
				if (doResetCalculateType)
				{
					_geoManager.ResetCalcuteType();
				}
				else
				{
					exclusiveCheckbox(sender);
				}
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error("GeometryMeasureControl Error");
				Hanbo.Log.LogManager.Error(ex);
			}
			return doResetCalculateType;
		}
		private void checkBox6_CheckedChanged(object sender, EventArgs e)
		{
			//Distance
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.Distance);
			}
		}

		private void checkBox7_CheckedChanged(object sender, EventArgs e)
		{
			//Circle
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.Point3ToCircle);
			}
		}

		private void checkBox8_CheckedChanged(object sender, EventArgs e)
		{
			//Symetry
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.SymmetryLine);
			}
		}

		private void checkBox9_CheckedChanged(object sender, EventArgs e)
		{
			//angle
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.Angle);
			}
		}

		private void checkBox10_CheckedChanged(object sender, EventArgs e)
		{
			//Crosspoint
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.CrossPoint);
			}
		}

		private void checkBox11_CheckedChanged(object sender, EventArgs e)
		{
			//Distance X
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.DistanceX);
			}
		}

		private void checkBox12_CheckedChanged(object sender, EventArgs e)
		{
			//Distance Y
			if (_ckLock) return;
			if (!resetCalculateTypeDoer(sender))
			{
				_geoManager.SetCalcuteType(CalcuteType.DistanceY);
			}
		}
		/*
		#region Buttons

		#endregion
		*/
		/*
		#region Designer===========================================================
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabControl MeasureTagControl { get { return this.MeasureFuncTabControl; } }
		#endregion
		*/
	}
}