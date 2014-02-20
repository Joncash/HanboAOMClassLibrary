using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Model;

namespace ViewROI
{
	/// <summary>
	/// 工程圖 - 點, 交點
	/// </summary>
	public class ROIProgPoint : ROI
	{
		#region private variables
		private ProgGraphicModel _model;
		private double _rawPointRow;	//點 y 的座標
		private double _rawPointCol;	//點 x 的座標
		private List<PositionModel> _lines;	//相依的線段元素
		#endregion

		#region 建構子
		public ROIProgPoint(ProgGraphicModel model)
		{
			_model = model;
			init();
		}
		#endregion

		#region private methods
		private void init()
		{
			this.NumHandles = 1;
			_lines = new List<PositionModel>();
			if (_model != null)
			{
				//原始值
				this.Name = _model.Name;
				this.ID = _model.ID;

				//點
				_rawPointRow = (_model.RowBegin);
				_rawPointCol = (_model.ColBegin);

				//如果此點有相依的線段元素
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


			//畫交點
			if (!this.IsActive)
				HOperatorSet.SetColor(window, "magenta");

			//HOperatorSet.SetDraw(window, "fill");
			HOperatorSet.SetLineWidth(window, 2);
			HOperatorSet.DispCross(window, _model.RowBegin, _model.ColBegin, crossSize, crossAngle);

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

			val[0] = HMisc.DistancePp(y, x, this._rawPointRow, this._rawPointCol); // midpoint 

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

		}
		/// <summary>
		/// 移動 ROI
		/// </summary>
		/// <param name="newX">x position of mouse</param>
		/// <param name="newY">y position of mouse</param>
		public override void moveByHandle(double newX, double newY)
		{

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
