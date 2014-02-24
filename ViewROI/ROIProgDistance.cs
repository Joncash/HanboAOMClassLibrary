using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI.Model;
using System.Data.Linq;
using System.Linq;
using ViewROI.Interface;
namespace ViewROI
{
	/// <summary>
	/// 描述工程圖形距離的 ROI
	/// </summary>
	public class ROIProgDistance : ROI, IProg
	{
		#region private variables
		private ProgGraphicModel _model;
		private double _rawCenterRow;	//中點 y 的座標
		private double _rawCenterCol;	//中點 x 的座標

		private double _halfDistance;
		private double _phiStart;
		private double _phiEnd;
		private List<PositionModel> _dependROIModels;	//
		private List<PositionModel> _ROIModelPoints = new List<PositionModel>();

		private double _arrowLineRowBegin;
		private double _arrowLineColBegin;
		private double _arrowLineRowEnd;
		private double _arrowLineColEnd;

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

		#region 建構子
		public ROIProgDistance(ProgGraphicModel model)
		{
			_model = model;
			init();
		}
		#endregion

		#region private methods
		private void init()
		{
			this.NumHandles = 2;//移動用的 Handle, 1 middle point, 整條線
			_dependROIModels = new List<PositionModel>();
			if (_model != null)
			{
				//原始值
				_rawCenterRow = (_model.RowBegin + _model.RowEnd) / 2.0;
				_rawCenterCol = (_model.ColBegin + _model.ColEnd) / 2.0;
				_halfDistance = _model.Distance / 2.0;

				var radStart = HMisc.AngleLx(_rawCenterRow, _rawCenterCol, _model.RowBegin, _model.ColBegin);
				_phiStart = halconRadToNormalRad(radStart);

				var radEnd = HMisc.AngleLx(_rawCenterRow, _rawCenterCol, _model.RowEnd, _model.ColEnd);
				_phiEnd = halconRadToNormalRad(radEnd);

				this.NewCenterRow = (!_model.UserDefineCenterRow.HasValue) ? _rawCenterRow : _model.UserDefineCenterRow.Value;
				this.NewCenterCol = (!_model.UserDefineCenterCol.HasValue) ? _rawCenterCol : _model.UserDefineCenterCol.Value;

				this.ID = _model.ID;
				Name = _model.Name;

				//_modelPoints = new List<PositionModel>() { 
				//	new PositionModel(){ RowBegin =_model.RowBegin, ColBegin = _model.ColBegin },
				//	new PositionModel(){ RowBegin =_model.RowEnd, ColBegin = _model.ColEnd },
				//};

				if (_model.ROIs != null)
				{
					var basicGeoType = new List<MeasureType>() { 
											{MeasureType.FitLine},
											{MeasureType.Line},
											{MeasureType.Point},
											{MeasureType.Circle}
										};
					for (var i = 0; i < _model.ROIs.Length; i++)
					{
						var roiModel = _model.ROIs[i];

						PositionModel dto;
						if (roiModel.RowBegin > 0
							&& roiModel.ColBegin > 0
							&& roiModel.RowEnd > 0
							&& roiModel.ColEnd > 0)
						{
							dto = new PositionModel()
							{
								RowBegin = roiModel.RowBegin,
								ColBegin = roiModel.ColBegin,
								RowEnd = roiModel.RowEnd,
								ColEnd = roiModel.ColEnd,
							};
							_ROIModelPoints.Add(new PositionModel()
							{
								RowBegin = (roiModel.RowBegin + roiModel.RowEnd) / 2.0,
								ColBegin = (roiModel.ColBegin + roiModel.ColEnd) / 2.0
							});
						}
						else
						{
							dto = new PositionModel()
							{
								RowBegin = roiModel.RowBegin,
								ColBegin = roiModel.ColBegin,
							};
							_ROIModelPoints.Add(new PositionModel()
							{
								RowBegin = (roiModel.RowBegin),
								ColBegin = (roiModel.ColBegin)
							});
						}
						if (basicGeoType.Contains(roiModel.GeoType))
							_dependROIModels.Add(dto);
					}
					_ROIModelPoints = _ROIModelPoints.OrderBy(x => x.ColBegin).OrderBy(y => y.RowBegin).ToList();

				}
			}
		}
		private double halconRadToNormalRad(double rad)
		{
			var oneRadDegree = (180.0 / Math.PI);
			var oneDegreeRad = (Math.PI / 180.0);
			var degree = 0.0;
			if (rad < 0)
				degree = Math.Abs(rad) * oneRadDegree;
			else
				degree = 360 - (rad * oneRadDegree);
			return degree * oneDegreeRad;
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
				HOperatorSet.SetTposition(window, this.NewCenterRow, this.NewCenterCol);
				if (!this.IsActive)
					HOperatorSet.SetColor(window, "red");
				window.WriteString(Name);
			}

			// 畫箭頭, 由線段的中心點向兩端畫箭頭			
			window.SetLineWidth(1);

			//計算兩箭頭端點位置
			var arrowPointRowBegin = _arrowLineRowBegin = this.NewCenterRow + (Math.Sin(_phiStart) * _halfDistance);
			var arrowPointColBegin = _arrowLineColBegin = this.NewCenterCol + (Math.Cos(_phiStart) * _halfDistance);

			var arrowPointRowEnd = _arrowLineRowEnd = this.NewCenterRow + (Math.Sin(_phiEnd) * _halfDistance);
			var arrowPointColEnd = _arrowLineColEnd = this.NewCenterCol + (Math.Cos(_phiEnd) * _halfDistance);

			window.DispArrow(this.NewCenterRow, this.NewCenterCol, arrowPointRowBegin, arrowPointColBegin, arrowSize);
			window.DispArrow(this.NewCenterRow, this.NewCenterCol, arrowPointRowEnd, arrowPointColEnd, arrowSize);

			//畫虛線，如果使用者改變了原始中心點的位置
			//由兩箭頭端點連結原始的起始兩端點
			if (_rawCenterRow != this.NewCenterRow || _rawCenterCol != this.NewCenterCol)
			{
				HOperatorSet.SetLineStyle(window, dotLineStyle);
				//決定兩個端點的連結，將座標點 以左上排序
				var arrowPoints = new List<PositionModel>() { 
					new PositionModel(){ RowBegin =arrowPointRowBegin, ColBegin = arrowPointColBegin },
					new PositionModel(){ RowBegin =arrowPointRowEnd, ColBegin = arrowPointColEnd },
				};

				arrowPoints = arrowPoints.OrderBy(x => x.ColBegin).OrderBy(y => y.RowBegin).ToList();

				for (int i = 0; i < _ROIModelPoints.Count; i++)
					window.DispLine(arrowPoints[i].RowBegin, arrowPoints[i].ColBegin, _ROIModelPoints[i].RowBegin, _ROIModelPoints[i].ColBegin);
			}

			//reset Style
			HOperatorSet.SetLineStyle(window, null);

			//畫ROI
			window.SetLineWidth(1);
			if (!this.IsActive)
				HOperatorSet.SetColor(window, "magenta");
			for (var i = 0; i < _dependROIModels.Count; i++)
			{
				var line = _dependROIModels[i];
				if (isLine(line))
				{
					//draw line，ROI 的線				
					window.DispLine(line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
				}
				else
				{
					//draw point，ROI 的點
					HXLDCont crossPoint = new HXLDCont();
					crossPoint.GenEmptyObj();
					crossPoint.GenCrossContourXld(line.RowBegin, line.ColBegin, crossSize, crossAngle);
					crossPoint.DispObj(window);
					//window.DispCross(line.RowBegin, line.ColBegin, crossSize, crossAngle);
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
			val[1] = HMisc.DistancePl(y, x, this._arrowLineRowBegin, this._arrowLineColBegin, this._arrowLineRowEnd, this._arrowLineColEnd);
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
				case 1:
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
				case 1: // whole line
					_model.UserDefineCenterCol = NewCenterCol = newX;
					_model.UserDefineCenterRow = NewCenterRow = newY;
					break;
			}
		}
		#endregion

		#region IProg Implement
		public ProgGraphicModel GetProgGraphicModel()
		{
			return _model;
		}
		public void SetCustomPos(double userDefineX, double userDefineY)
		{
			_model.UserDefineCenterCol = this.NewCenterCol = userDefineX;
			_model.UserDefineCenterRow = this.NewCenterRow = userDefineY;
		}
		#endregion



	}
}
