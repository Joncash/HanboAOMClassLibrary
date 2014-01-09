using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI
{
	/// <summary>
	/// 描述工程圖形距離的 ROI
	/// </summary>
	public class ROIProgDistance : ROI
	{
		class LineDTO
		{
			public double RowBegin;
			public double ColBegin;
			public double RowEnd;
			public double ColEnd;
		}

		#region private variables
		private double _rowBegin;//起點 y 的座標
		private double _colBegin;//起點 x 的座標
		private double _rowEnd;	//終點 y 的座標
		private double _colEnd;	//終點 x 的座標
		private double _centerRow;	//中點 y 的座標
		private double _centerCol;	//中點 x 的座標
		private double _distance;
		private double _phi;

		private List<LineDTO> _lines;	//
		#endregion
		#region Public variables
		public string Name;

		#endregion

		#region 建構子
		public ROIProgDistance()
		{
			init();
		}
		public ROIProgDistance(ProgGraphicModel model)
		{
			init();
			_lines = new List<LineDTO>();
			if (model != null)
			{
				_centerRow = (model.RowBegin + model.RowEnd) / 2.0;
				_centerCol = (model.ColBegin + model.ColEnd) / 2.0;
				_distance = model.Distance;
				_rowBegin = model.RowBegin;
				_colBegin = model.ColBegin;
				_rowEnd = model.RowEnd;
				_colEnd = model.ColEnd;
				base.ID = model.ID;
				Name = model.Name;
				_phi = HMisc.AngleLx(_rowBegin, _colBegin, _rowEnd, _colEnd);

				if (model.ROIs != null)
				{
					for (var i = 0; i < model.ROIs.Length; i++)
					{
						var roiModel = model.ROIs[i];
						if (roiModel.RowBegin < 0
							&& roiModel.ColBegin < 0
							&& roiModel.RowEnd < 0
							&& roiModel.ColEnd < 0) continue;

						var dto = new LineDTO()
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

		#endregion

		#region private methods
		private void init()
		{
			base.NumHandles = 1;//移動用的 Handle, 1 middle point
		}
		private bool isLine(LineDTO line)
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
			double arrowSize = 2;
			double crossSize = 12;
			double crossAngle = 0.785398;

			//寫字
			if (!String.IsNullOrEmpty(Name))
			{
				HOperatorSet.SetTposition(window, _centerRow, _centerCol);
				window.WriteString(Name);
			}

			//畫線段
			window.SetLineWidth(2);
			for (var i = 0; i < _lines.Count; i++)
			{
				var line = _lines[i];
				if (isLine(line))
				{
					//draw line
					window.DispLine(line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
				}
				else
				{
					//draw point
					window.DispCross(line.RowBegin, line.ColBegin, crossSize, crossAngle);
				}
			}
			window.SetLineWidth(1);
			//畫箭頭，中心點為 (_centerCol, _centerRow)			
			_rowBegin = _centerRow + (Math.Sin(_phi) * _distance / 2.0);
			_colBegin = _centerCol + (Math.Cos(_phi) * _distance / 2.0);

			_rowEnd = _centerRow + (Math.Sin(_phi - Math.PI) * _distance / 2.0);
			_colEnd = _centerCol + (Math.Cos(_phi - Math.PI) * _distance / 2.0);
			/*
			if (_phi == 0.0)
			{
				_rowEnd = _centerRow - (Math.Sin(-_phi) * _distance / 2.0);
				_colEnd = _centerCol - (Math.Cos(-_phi) * _distance / 2.0);
			}
			else
			{
				_rowEnd = _centerRow + (Math.Sin(-_phi) * _distance / 2.0);
				_colEnd = _centerCol + (Math.Cos(-_phi) * _distance / 2.0);
			}
			 */
			window.DispArrow(_centerRow, _centerCol, _rowBegin, _colBegin, arrowSize);
			window.DispArrow(_centerRow, _centerCol, _rowEnd, _colEnd, arrowSize);

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

			val[0] = HMisc.DistancePp(y, x, _centerRow, _centerCol); // midpoint 

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
			var rectangleSize = 10;
			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(_centerRow, _centerCol, 0, rectangleSize, rectangleSize);
					break;
			}
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
			switch (activeHandleIdx)
			{
				case 0: // center Point 	
					_centerCol = newX;
					_centerRow = newY;
					break;
			}
		}
		#endregion
	}
}
