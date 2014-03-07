using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI.Model;

namespace ViewROI
{
	/// <summary>
	/// 描述工程圖-圓形的 ROI
	/// </summary>
	public class ROIProgCircle : ROI
	{
		#region private variables
		public int CircleDistanceSetting = 1;
		private ProgGraphicModel _model;
		/// <summary>
		/// 圓心位置 y
		/// </summary>
		private double _rawCenterRow;

		/// <summary>
		/// 圓心位置 x
		/// </summary>
		private double _rawCenterCol;
		#endregion

		#region public variables
		/// <summary>
		/// 修改 or 移動後的標示位置 y 座標
		/// </summary>
		public double NewCenterRow;

		/// <summary>
		/// 修改 or 移動後的標示位置 x 座標
		/// </summary>
		public double NewCenterCol;
		#endregion

		#region 建構子
		public ROIProgCircle(ProgGraphicModel model)
		{
			_model = model;
			init();
		}
		#endregion

		#region private methods
		private void init()
		{
			this.NumHandles = 1;//1 middle point, 1 rotation point
			if (_model != null)
			{
				//原始值
				this.ID = _model.ID;
				this.Name = _model.Name;

				_rawCenterRow = (_model.RowBegin);
				_rawCenterCol = (_model.ColBegin);

				this.NewCenterRow = _rawCenterRow;
				this.NewCenterCol = _rawCenterCol;
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
			double crossSize = 12;
			double crossAngle = 0.785398;
			//寫字
			if (!String.IsNullOrEmpty(Name))
			{
				if (!this.IsActive)
					window.SetColor("red");
				HOperatorSet.SetTposition(window, _model.RowBegin, _model.ColBegin);
				window.WriteString(Name);
			}

			//畫圓
			if (!this.IsActive)
				window.SetColor("magenta");

			window.SetLineWidth(2);
			var radius = _model.Distance / CircleDistanceSetting;
			window.DispCross(_model.RowBegin, _model.ColBegin, crossSize, crossAngle);
			window.DispCircle(_model.RowBegin, _model.ColBegin, radius);

			//畫圓內虛線
			/**/
			window.SetLineWidth(1);
			HTuple dotLineStyle = new HTuple(new int[4] { 20, 7, 3, 7 });
			window.SetLineStyle(dotLineStyle);
			var hLineRowBegin = _model.RowBegin;
			var hLineColBegin = _model.ColBegin - radius;
			var hLineRowEnd = _model.RowBegin;
			var hLineColEnd = _model.ColBegin + radius;

			var vLineRowBegin = _model.RowBegin - radius;
			var vLineColBegin = _model.ColBegin;
			var vLineRowEnd = _model.RowBegin + radius;
			var vLineColEnd = _model.ColBegin;
			window.DispLine(hLineRowBegin, hLineColBegin, hLineRowEnd, hLineColEnd);
			window.DispLine(vLineRowBegin, vLineColBegin, vLineRowEnd, vLineColEnd);

			//Reset line Style
			HOperatorSet.SetLineStyle(window, null);
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

			val[0] = HMisc.DistancePp(y, x, this.NewCenterRow, this.NewCenterCol); // midpoint 

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
			//		//window.DispRectangle2(this.NewCenterRow, this.NewCenterCol, 0, rectangleSize, rectangleSize);
			//		break;
			//}
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

		public override HRegion getRegion()
		{
			return null;
		}
		public override HTuple getModelData()
		{
			return null;
		}
		#endregion
	}
}
