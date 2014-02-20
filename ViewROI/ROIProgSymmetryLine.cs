using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Model;

namespace ViewROI
{
	/// <summary>
	/// 工程圖 - 對稱中線
	/// </summary>
	public class ROIProgSymmetryLine : ROI
	{
		#region 建構子
		public ROIProgSymmetryLine(ProgGraphicModel model)
		{
			_model = model;
			init();
		}
		#endregion

		#region private variables
		private ProgGraphicModel _model;
		private double _rawCenterRow;	//中點 y 的座標
		private double _rawCenterCol;	//中點 x 的座標

		private List<PositionModel> _lines;	//
		#endregion

		#region private methods
		private void init()
		{
			_lines = new List<PositionModel>();
			if (_model != null)
			{
				this.NumHandles = 1; //整個線段
				this.ID = _model.ID;
				this.Name = _model.Name;

				//原始值
				_rawCenterRow = (_model.RowBegin + _model.RowEnd) / 2.0;
				_rawCenterCol = (_model.ColBegin + _model.ColEnd) / 2.0;

				if (_model.ROIs != null)
				{
					for (var i = 0; i < _model.ROIs.Length; i++)
					{
						var roiModel = _model.ROIs[i];
						if (roiModel.RowBegin < 0
							&& roiModel.ColBegin < 0
							&& roiModel.RowEnd < 0
							&& roiModel.ColEnd < 0) continue;

						var dto = new PositionModel()
						{
							RowBegin = roiModel.RowBegin,
							ColBegin = roiModel.ColBegin,
							RowEnd = roiModel.RowEnd,
							ColEnd = roiModel.ColEnd,
						};
						_lines.Add(dto);
					}
				}
			}
		}
		private bool isLine(PositionModel line)
		{
			return (line.RowBegin > 0 && line.ColBegin > 0 && line.RowEnd > 0 && line.ColEnd > 0);
		}
		#endregion

		#region Override methods
		/// <summary>
		/// 建立 roi
		/// </summary>
		/// <param name="midX">position x of mouse</param>
		/// <param name="midY">position y of mouse</param>
		public override void createROI(double midX, double midY)
		{

		}

		/// <summary>
		/// draw, 決定要畫什麼在 window 上
		/// </summary>
		/// <param name="window">Halcon Window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			//虛線
			HTuple dotLineStyle = new HTuple(new int[4] { 20, 7, 3, 7 });

			//寫字
			if (!String.IsNullOrEmpty(Name))
			{
				if (!this.IsActive)
					window.SetColor("red");

				HOperatorSet.SetTposition(window, _rawCenterRow, _rawCenterCol);
				window.WriteString(Name);
			}

			//Draw 中線
			window.SetLineStyle(dotLineStyle);
			window.DispLine(_model.RowBegin, _model.ColBegin, _model.RowEnd, _model.ColEnd);

			//Reset line Style
			HOperatorSet.SetLineStyle(window, null);

			//畫相依的線元素 (ROI)
			window.SetLineWidth(2);
			if (!this.IsActive)
				HOperatorSet.SetColor(window, "magenta");
			for (var i = 0; i < _lines.Count; i++)
			{
				var line = _lines[i];
				window.DispLine(line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
			}
		}

		/// <summary>
		/// 計算滑鼠位置接近控制點 (Handle) 的距離
		/// </summary>
		/// <param name="x">x position of mouse</param>
		/// <param name="y">y position of mouse</param>
		/// <returns></returns>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			double[] val = new double[NumHandles];

			val[0] = HMisc.DistancePl(y, x, _model.RowBegin, _model.ColBegin, _model.RowEnd, _model.ColEnd);

			for (int i = 0; i < NumHandles; i++)
			{
				if (val[i] < max)
				{
					max = val[i];
					activeHandleIdx = i;
				}
			}// end of for 
			return val[activeHandleIdx];
		}

		/// <summary>
		/// 顯示控制點
		/// </summary>
		/// <param name="window"></param>
		public override void displayActive(HalconDotNet.HWindow window)
		{
			//var rectangleSize = 10;
			//switch (activeHandleIdx)
			//{
			//	case 0:
			//		window.SetLineWidth(2);
			//		window.SetColor("green");
			//		for (var i = 0; i < _lines.Count; i++)
			//		{
			//			var line = _lines[i];
			//			if (isLine(line))
			//			{
			//				//draw line，ROI 的線				
			//				window.DispLine(line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
			//			}
			//		}
			//		//reset
			//		window.SetLineWidth(1);
			//		break;
			//}
		}
		public override HRegion getRegion()
		{
			return null;
		}
		public override HTuple getModelData()
		{
			return null;
		}

		/// <summary>
		/// 移動 ROI
		/// </summary>
		/// <param name="newX">x position of mouse</param>
		/// <param name="newY">y position of mouse</param>
		public override void moveByHandle(double newX, double newY)
		{
			//switch (activeHandleIdx)
			//{
			//	case 0: // center Point 	
			//		_model.UserDefineCenterCol = NewCenterCol = newX;
			//		_model.UserDefineCenterRow = NewCenterRow = newY;
			//		break;
			//}
		}
		#endregion
	}
}
