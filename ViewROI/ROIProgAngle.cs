using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Interface;
using ViewROI.Model;

namespace ViewROI
{
	/// <summary>
	/// Angle 工程圖
	/// </summary>
	public class ROIProgAngle : ROI, IProg
	{
		#region 建構子
		public ROIProgAngle(ProgGraphicModel model)
		{
			_model = model;
			init();
		}
		#endregion

		#region private variables
		private ProgGraphicModel _model;
		private double _rawCenterRow;	//中點 y 的座標，畫角度的起始端點
		private double _rawCenterCol;	//中點 x 的座標

		private double _lineACenterRow;
		private double _lineACenterCol;
		private double _lineBCenterRow;
		private double _lineBCenterCol;

		private List<PositionModel> _lines;	//
		#endregion

		#region Public variables
		/// <summary>
		/// 修改 or 移動後的標示位置 y 座標
		/// </summary>
		public double NewCenterRow;

		/// <summary>
		/// 修改 or 移動後的標示位置 x 座標
		/// </summary>
		public double NewCenterCol;
		#endregion

		#region private methods
		private void init()
		{
			this.NumHandles = 1;//移動用的 Handle, 1 middle point
			_lines = new List<PositionModel>();
			_rawCenterRow = _rawCenterCol = -1;
			_lineACenterRow = _lineACenterRow = _lineBCenterRow = _lineBCenterRow = -1;
			if (_model != null)
			{
				this.ID = _model.ID;
				this.Name = _model.Name;

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

					if (_lines.Count > 1)
					{
						_lineACenterRow = _rawCenterRow = (_lines[0].RowBegin + _lines[0].RowEnd) / 2.0;
						_lineACenterCol = _rawCenterCol = (_lines[0].ColBegin + _lines[0].ColEnd) / 2.0;

						_lineBCenterRow = (_lines[1].RowBegin + _lines[1].RowEnd) / 2.0;
						_lineBCenterCol = (_lines[1].ColBegin + _lines[1].ColEnd) / 2.0;
					}
				}
				this.NewCenterRow = (!_model.UserDefineCenterRow.HasValue) ? _rawCenterRow : _model.UserDefineCenterRow.Value;
				this.NewCenterCol = (!_model.UserDefineCenterCol.HasValue) ? _rawCenterCol : _model.UserDefineCenterCol.Value;
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
			double arrowSize = 2;
			double crossSize = 12;
			double crossAngle = 0.785398;

			HTuple dotLineStyle = new HTuple(new int[4] { 7, 7, 7, 7 });
			//Reset line Style
			HOperatorSet.SetLineStyle(window, null);

			//寫字
			if (!String.IsNullOrEmpty(Name))
			{
				if (!this.IsActive)
					HOperatorSet.SetColor(window, "red");

				HOperatorSet.SetTposition(window, this.NewCenterRow, this.NewCenterCol);
				window.WriteString(Name);
			}


			//畫線段，水平 + 30 度角
			var lineLength = 50;
			var angle = Math.PI / 6.0;
			var hLineRow = this.NewCenterRow + Math.Sin(Math.PI) * lineLength;
			var hLineCol = this.NewCenterCol - Math.Cos(Math.PI) * lineLength;
			var degLineRow = this.NewCenterRow - Math.Sin(angle) * lineLength;
			var degLineCol = this.NewCenterCol + Math.Cos(angle) * lineLength;
			HOperatorSet.DispLine(window, this.NewCenterRow, this.NewCenterCol, hLineRow, hLineCol);
			HOperatorSet.DispLine(window, this.NewCenterRow, this.NewCenterCol, degLineRow, degLineCol);

			//畫 arc
			// angle 為正時，會順時針畫，
			// angle 為負時，會逆時針畫，
			// 因此要注意兩線段夾角的開口方向來選擇
			var arcBeginRow = this.NewCenterRow - Math.Sin(angle) * (lineLength / 2.0);
			var arcBeginCol = this.NewCenterCol + Math.Cos(angle) * (lineLength / 2.0);

			window.DispArc(this.NewCenterRow, this.NewCenterCol, angle, arcBeginRow, arcBeginCol);

			// 畫箭頭, 
			window.SetLineWidth(1);
			HOperatorSet.SetLineStyle(window, dotLineStyle);

			window.DispArrow(this.NewCenterRow, this.NewCenterCol, _lineACenterRow, _lineACenterCol, arrowSize);
			window.DispArrow(this.NewCenterRow, this.NewCenterCol, _lineBCenterRow, _lineBCenterCol, arrowSize);

			//Reset line Style
			HOperatorSet.SetLineStyle(window, null);

			//畫ROI
			window.SetLineWidth(2);
			if (!this.IsActive)
				HOperatorSet.SetColor(window, "magenta");

			for (var i = 0; i < _lines.Count; i++)
			{
				var line = _lines[i];
				if (isLine(line))
				{
					//draw line，ROI 的線
					window.DispLine(line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
				}
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
			var rectangleSize = 10;
			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(this.NewCenterRow, this.NewCenterCol, 0, rectangleSize, rectangleSize);
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
					_model.UserDefineCenterCol = NewCenterCol = newX;
					_model.UserDefineCenterRow = NewCenterRow = newY;
					break;
			}
		}
		#endregion

		public ProgGraphicModel GetProgGraphicModel()
		{
			return _model;
		}
		public void SetCustomPos(double userDefineX, double userDefineY)
		{
			this.NewCenterCol = userDefineX;
			this.NewCenterRow = userDefineY;
		}
	}
}
