using HalconDotNet;
using Hanbo.Geometry;
using Hanbo.Geometry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Interface;
using ViewROI.Model;

namespace ViewROI.SmartROIs
{
	public class SmartArc : ROI, IContinueZoom, IROIModelUpdateable
	{
		/// <summary>
		/// 半徑
		/// </summary>
		private double radius;

		/// <summary>
		/// 圓心 Y 座標
		/// </summary>
		private double midR;

		/// <summary>
		/// 圓心 X 座標
		/// </summary>
		private double midC;

		//點座標
		private GeoPoint _FirstPoint;
		private GeoPoint _SecondPoint;
		private GeoPoint _EndPoint;

		private double startPhi, extentPhi; // -2*PI <= x <= 2*PI

		//display attributes
		private HXLDCont contour;
		private HXLDCont _ArcRegion;
		private HXLDCont arrowHandleXLD;
		private string circDir;
		private double TwoPI;
		private double PI;

		private double sizeR, sizeC;     // 1. handle        
		private double startR, startC;   // 2. handle
		private double extentR, extentC; // 3. handle

		// 畫弧的邊界
		private int _ROIWidth = 10;

		public SmartArc()
		{
			NumHandles = 4; // one at corner of circle + midpoint
			activeHandleIdx = 0;
			this.ROIMeasureType = MeasureType.Circle;

			_clickedPointsPositionList = new List<PositionModel>();

			contour = new HXLDCont();
			circDir = "";
			_ArcRegion = new HXLDCont();

			TwoPI = 2 * Math.PI;
			PI = Math.PI;

			arrowHandleXLD = new HXLDCont();
			arrowHandleXLD.GenEmptyObj();
		}


		/// <summary>Creates a new ROI instance at the mouse position</summary>
		public override void createROI(double midX, double midY)
		{
			/*
			midR = midY;
			midC = midX;

			radius = 100;

			sizeR = midR;
			sizeC = midC - radius;

			startPhi = PI * 0.25;
			extentPhi = PI * 1.5;
			circDir = "positive";

			determineArcHandles();
			updateArrowHandle();
			 */
		}

		/// <summary>Paints the ROI into the supplied window</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			//畫個 x
			double crossSize = 12;
			double crossAngle = 0.785398;

			//Reset line Style
			HOperatorSet.SetLineStyle(window, null);
			if (!_initPointsDone)
			{
				foreach (var dot in _clickedPointsPositionList)
				{
					HOperatorSet.DispCross(window, dot.RowBegin, dot.ColBegin, crossSize, crossAngle);
				}
			}
			else
			{
				//畫弧線
				contour.Dispose();
				contour.GenCircleContourXld(midR, midC, radius, startPhi,
											(startPhi + extentPhi), circDir, 1.0);
				window.DispObj(contour);
				drawOuterArc(window);

				//畫控制點
				window.DispRectangle2(sizeR, sizeC, 0, 5, 5);
				window.DispRectangle2(midR, midC, 0, 5, 5);
				window.DispRectangle2(startR, startC, startPhi, 10, 2);
				window.DispObj(arrowHandleXLD);
			}
		}

		private void drawOuterArc(HalconDotNet.HWindow window)
		{
			double sPhi, extent, innerRad, outerRad;
			HTuple innerR, outerR, innerC, outerC;
			HXLDCont outCont, innerCont, contour;

			outCont = new HXLDCont();
			innerCont = new HXLDCont();

			var roiModel = this.getModelData();
			var mMeasROI = genArcRegionModel(roiModel);

			sPhi = mMeasROI[2];
			extent = mMeasROI[3];
			outerRad = mMeasROI[4];
			innerRad = mMeasROI[5];


			innerCont.GenCircleContourXld(mMeasROI[0], mMeasROI[1], innerRad, sPhi, (sPhi + extent), (extent > 0) ? "positive" : "negative", 1.0);
			outCont.GenCircleContourXld(mMeasROI[0], mMeasROI[1], outerRad, (sPhi + extent), sPhi, (extent > 0) ? "negative" : "positive", 1.0);

			innerCont.GetContourXld(out innerR, out innerC);
			outCont.GetContourXld(out outerR, out outerC);
			innerR = innerR.TupleConcat(outerR);
			innerC = innerC.TupleConcat(outerC);

			contour = new HXLDCont(innerR, innerC);
			contour = contour.CloseContoursXld();

			_ArcRegion.Dispose();
			_ArcRegion = contour.GenRegionContourXld("margin");
			window.DispObj(_ArcRegion);

			contour.Dispose();
			innerCont.Dispose();
			outCont.Dispose();
		}

		/// <summary>
		/// 產生弧的 Region
		/// </summary>
		/// <param name="circleModel"></param>
		/// <returns>[0] = row, [1] = col, [2] = startPhi, [3] = extent, [4] = RadiusOuter, [5] = RadiusInner</returns>
		private double[] genArcRegionModel(double[] circleModel)
		{
			double row1 = circleModel[0];
			double col1 = circleModel[1];
			double radius = circleModel[2];
			double startPhi = circleModel[3];
			double extent = circleModel[4];

			if (radius <= _ROIWidth)
				return new HTuple(new double[] { row1, col1, startPhi, extent, (radius + _ROIWidth), 0.0 });
			else
				return new HTuple(new double[] { row1, col1, startPhi, extent, (radius + _ROIWidth), (radius - _ROIWidth) });
		}

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			if (!_initPointsDone) return max;
			double[] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, midR, midC);       // midpoint 
			val[1] = HMisc.DistancePp(y, x, sizeR, sizeC);     // border handle 
			val[2] = HMisc.DistancePp(y, x, startR, startC);   // border handle 
			val[3] = HMisc.DistancePp(y, x, extentR, extentC); // border handle 

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
		/// Paints the active handle of the ROI object into the supplied window 
		/// </summary>
		public override void displayActive(HalconDotNet.HWindow window)
		{
			if (!_initPointsDone) return;
			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(sizeR, sizeC, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(startR, startC, startPhi, 10, 2);
					break;
				case 3:
					window.DispObj(arrowHandleXLD);
					break;
			}
		}

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y)
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			HTuple distance;
			double dirX, dirY, prior, next, valMax, valMin;

			switch (activeHandleIdx)
			{
				case 0: // midpoint 
					dirY = midR - newY;
					dirX = midC - newX;

					midR = newY;
					midC = newX;

					sizeR -= dirY;
					sizeC -= dirX;

					determineArcHandles();
					break;

				case 1: // handle at circle border                  
					sizeR = newY;
					sizeC = newX;

					HOperatorSet.DistancePp(new HTuple(sizeR), new HTuple(sizeC),
											new HTuple(midR), new HTuple(midC), out distance);
					radius = distance[0].D;
					determineArcHandles();
					break;

				case 2: // start handle for arc                
					dirY = newY - midR;
					dirX = newX - midC;

					startPhi = Math.Atan2(-dirY, dirX);

					if (startPhi < 0)
						startPhi = PI + (startPhi + PI);

					setStartHandle();
					prior = extentPhi;
					extentPhi = HMisc.AngleLl(midR, midC, startR, startC, midR, midC, extentR, extentC);

					if (extentPhi < 0 && prior > PI * 0.8)
						extentPhi = (PI + extentPhi) + PI;
					else if (extentPhi > 0 && prior < -PI * 0.7)
						extentPhi = -PI - (PI - extentPhi);

					break;

				case 3: // end handle for arc
					dirY = newY - midR;
					dirX = newX - midC;

					prior = extentPhi;
					next = Math.Atan2(-dirY, dirX);

					if (next < 0)
						next = PI + (next + PI);

					if (circDir == "positive" && startPhi >= next)
						extentPhi = (next + TwoPI) - startPhi;
					else if (circDir == "positive" && next > startPhi)
						extentPhi = next - startPhi;
					else if (circDir == "negative" && startPhi >= next)
						extentPhi = -1.0 * (startPhi - next);
					else if (circDir == "negative" && next > startPhi)
						extentPhi = -1.0 * (startPhi + TwoPI - next);

					valMax = Math.Max(Math.Abs(prior), Math.Abs(extentPhi));
					valMin = Math.Min(Math.Abs(prior), Math.Abs(extentPhi));

					if ((valMax - valMin) >= PI)
						extentPhi = (circDir == "positive") ? -1.0 * valMin : valMin;

					setExtentHandle();
					break;
			}

			circDir = (extentPhi < 0) ? "negative" : "positive";
			updateArrowHandle();
		}

		/// <summary>Gets the HALCON region described by the ROI</summary>
		public override HRegion getRegion()
		{
			HRegion region;
			contour.Dispose();
			contour.GenCircleContourXld(midR, midC, radius, startPhi, (startPhi + extentPhi), circDir, 1.0);
			region = new HRegion(contour);
			return region;
		}

		/// <summary>
		/// Gets the model information described by the ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { midR, midC, radius, startPhi, extentPhi });
		}

		/// <summary>
		/// Auxiliary method to determine the positions of the second and
		/// third handle.
		/// </summary>
		private void determineArcHandles()
		{
			setStartHandle();
			setExtentHandle();
		}

		/// <summary> 
		/// Auxiliary method to recalculate the start handle for the arc 
		/// </summary>
		private void setStartHandle()
		{
			startR = midR - radius * Math.Sin(startPhi);
			startC = midC + radius * Math.Cos(startPhi);
		}

		/// <summary>
		/// Auxiliary method to recalculate the extent handle for the arc
		/// </summary>
		private void setExtentHandle()
		{
			extentR = midR - radius * Math.Sin(startPhi + extentPhi);
			extentC = midC + radius * Math.Cos(startPhi + extentPhi);
		}

		/// <summary>
		/// Auxiliary method to display an arrow at the extent arc position
		/// </summary>
		private void updateArrowHandle()
		{
			double row1, col1, row2, col2;
			double rowP1, colP1, rowP2, colP2;
			double length, dr, dc, halfHW, sign, angleRad;
			double headLength = 15;
			double headWidth = 15;

			arrowHandleXLD.Dispose();
			arrowHandleXLD.GenEmptyObj();

			row2 = extentR;
			col2 = extentC;
			angleRad = (startPhi + extentPhi) + Math.PI * 0.5;

			sign = (circDir == "negative") ? -1.0 : 1.0;
			row1 = row2 + sign * Math.Sin(angleRad) * 20;
			col1 = col2 - sign * Math.Cos(angleRad) * 20;

			length = HMisc.DistancePp(row1, col1, row2, col2);
			if (length == 0)
				length = -1;

			dr = (row2 - row1) / length;
			dc = (col2 - col1) / length;

			halfHW = headWidth / 2.0;
			rowP1 = row1 + (length - headLength) * dr + halfHW * dc;
			rowP2 = row1 + (length - headLength) * dr - halfHW * dc;
			colP1 = col1 + (length - headLength) * dc - halfHW * dr;
			colP2 = col1 + (length - headLength) * dc + halfHW * dr;

			if (length == -1)
				arrowHandleXLD.GenContourPolygonXld(row1, col1);
			else
				arrowHandleXLD.GenContourPolygonXld(new HTuple(new double[] { row1, row2, rowP1, row2, rowP2, row2 }),
					new HTuple(new double[] { col1, col2, colP1, col2, colP2, col2 }));
		}

		#region =================================================================
		/// <summary>
		/// 重設 ROI 資訊
		/// </summary>
		/// <param name="roiRow">ROI 圓心 Row</param>
		/// <param name="roiCol">ROI 圓心 Col</param>
		/// <param name="roiRadius">ROI 半徑</param>
		public void MakeROI(double roiRow, double roiCol, double roiRadius)
		{
			midR = roiRow;
			midC = roiCol;
			radius = roiRadius;


		}
		public override ROIViewModel ToROIViewModel()
		{
			return new ROIViewModel()
			{
				CenterRow = midR,
				CenterCol = midC,
				Radius = radius,
				ROIType = ROI.ROI_TYPE_CIRCLE,
				ID = ID,
			};
		}
		public override void MakeROI(ROIViewModel model)
		{
			MakeROI(model.CenterRow, model.CenterCol, model.Radius);
		}
		#endregion

		#region Implements
		private int _clickPoints = 3; //應點擊的次數
		private List<PositionModel> _clickedPointsPositionList;
		private bool _initPointsDone = false;
		private int _smartWidth = 10;
		private bool _success = false;
		public bool WaitForClickPoints(double x, double y)
		{
			if (_clickedPointsPositionList.Count < _clickPoints)
			{
				_clickedPointsPositionList.Add(new PositionModel() { ColBegin = x, RowBegin = y });
			}
			_initPointsDone = _clickedPointsPositionList.Count == _clickPoints;
			if (_initPointsDone)
			{
				//3 點求圓
				var rows = _clickedPointsPositionList.Select(p => p.RowBegin).ToArray();
				var cols = _clickedPointsPositionList.Select(p => p.ColBegin).ToArray();
				GeoOperatorSet.CalculateCircleCenter(rows, cols, out midR, out midC, out radius);

				//檢查 and notify
				_success = (midR > 0 && midC > 0 && radius > 0);
				if (_success)
				{
					_FirstPoint = new GeoPoint() { Row = rows[0], Col = cols[0] };
					_SecondPoint = new GeoPoint() { Row = rows[1], Col = cols[1] };
					_EndPoint = new GeoPoint() { Row = rows[2], Col = cols[2] };

					//
					sizeR = midR;
					sizeC = midC - radius;

					//toDo, 計算startPhi, extentPhi
					// 角度為正，表示在圓心的上方，反之在下方
					// 角度越大，表示越接近左上角，視為起始點
					// 角度越小，視為終點
					var agOne = HMisc.AngleLx(midR, midC, _FirstPoint.Row, _FirstPoint.Col);
					var agTwo = HMisc.AngleLx(midR, midC, _SecondPoint.Row, _SecondPoint.Col);
					var agThree = HMisc.AngleLx(midR, midC, _EndPoint.Row, _EndPoint.Col);
					Dictionary<string, double> agDict = new Dictionary<string, double>() { };
					agDict.Add("1", agOne);
					agDict.Add("2", agTwo);
					agDict.Add("3", agThree);

					//逆排序，再取奇數, 即為 StartPhi, EndPhi
					var angles = agDict.OrderByDescending(p => p.Value)
										.Where((p, idx) => idx % 2 == 0)
										.Select(p => p.Value).ToArray();
					startPhi = angles[0];
					var endPhi = angles[1];

					//計算延伸長度
					extentPhi = (startPhi < 0) ? Math.Abs(startPhi) - Math.Abs(endPhi)
												: endPhi - startPhi;
					circDir = "negative";//clockwise 畫弧

					determineArcHandles();
					updateArrowHandle();
				}
			}
			return _initPointsDone;
		}

		public int ClickedPoints
		{
			get
			{
				return _clickedPointsPositionList.Count;
			}
			set
			{
				return;
			}
		}

		public void UpdateROIModel(ROIViewModel updateModel)
		{
			//throw new NotImplementedException();
		}
		#endregion
	}
}
