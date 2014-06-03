using HalconDotNet;
using Hanbo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Interface;
using ViewROI.Model;

namespace ViewROI.SmartROIs
{
	public class SmartCircle : ROI, IContinueZoom
	{
		private double radius;
		private double row1, col1;  // first handle
		private double midR, midC;  // second handle		

		/// <summary>
		/// 預設量測類型為 "圓"
		/// </summary>
		public SmartCircle()
		{
			NumHandles = 2; // one at corner of circle + midpoint
			activeHandleIdx = 1;
			this.ROIMeasureType = MeasureType.Circle;

			_clickedPointsPositionList = new List<PositionModel>();
		}

		/// <summary>Creates a new ROI instance at the mouse position</summary>
		public override void createROI(double midX, double midY)
		{
			//midR = midY;
			//midC = midX;

			//radius = 100;

			//row1 = midR;
			//col1 = midC + radius;
		}

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

			row1 = midR;
			col1 = midC + radius;
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

		//畫 4個角及中間的 Handle Rectangle 大小
		private double _HandleRectangleWidth = 10.0;
		private double _HandleRectangleHeight = 10.0;

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
				window.DispCircle(midR, midC, radius);

				var w = _HandleRectangleWidth * _ZoomRatio;
				var h = _HandleRectangleHeight * _ZoomRatio;
				window.DispRectangle2(row1, col1, 0, w, h);
				window.DispRectangle2(midR, midC, 0, w, h);
			}
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

			val[0] = HMisc.DistancePp(y, x, row1, col1); // border handle 
			val[1] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

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
					window.DispRectangle2(row1, col1, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>Gets the HALCON region described by the ROI</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenCircle(midR, midC, radius);
			return region;
		}

		public override double getDistanceFromStartPoint(double row, double col)
		{
			double sRow = midR; // assumption: we have an angle starting at 0.0
			double sCol = midC + 1 * radius;

			double angle = HMisc.AngleLl(midR, midC, sRow, sCol, midR, midC, row, col);

			if (angle < 0)
				angle += 2 * Math.PI;

			return (radius * angle);
		}

		/// <summary>
		/// Gets the model information described by 
		/// the  ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { midR, midC, radius });
		}

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y)
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			HTuple distance;
			double shiftX, shiftY;

			switch (activeHandleIdx)
			{
				case 0: // handle at circle border

					row1 = newY;
					col1 = newX;
					HOperatorSet.DistancePp(new HTuple(row1), new HTuple(col1),
											new HTuple(midR), new HTuple(midC),
											out distance);

					radius = distance[0].D;
					break;
				case 1: // midpoint 

					shiftY = midR - newY;
					shiftX = midC - newX;

					midR = newY;
					midC = newX;

					row1 -= shiftY;
					col1 -= shiftX;
					break;
			}
		}
		#region Implement
		private int _clickPoints = 3; //應點擊的次數
		private List<PositionModel> _clickedPointsPositionList;
		private bool _initPointsDone = false;

		public bool WaitForClickPoints(double x, double y)
		{
			if (_clickedPointsPositionList.Count < _clickPoints)
			{
				var isNotTheSamePoint = _clickedPointsPositionList.Count == 0;
				var prevIdx = _clickedPointsPositionList.Count - 1;
				var prevPoint = prevIdx > -1 ? _clickedPointsPositionList[prevIdx] : null;
				if (prevPoint != null)
				{
					isNotTheSamePoint = Math.Abs(prevPoint.ColBegin - x) > 0 || Math.Abs(prevPoint.RowBegin - y) > 0;
				}
				if (isNotTheSamePoint)
				{
					_clickedPointsPositionList.Add(new PositionModel() { ColBegin = x, RowBegin = y });
				}
			}
			_initPointsDone = _clickedPointsPositionList.Count == _clickPoints;
			if (_initPointsDone)
			{
				//3 點求圓
				var rows = _clickedPointsPositionList.Select(p => p.RowBegin).ToArray();
				var cols = _clickedPointsPositionList.Select(p => p.ColBegin).ToArray();
				GeoOperatorSet.CalculateCircleCenter(rows, cols, out midR, out midC, out radius);
				row1 = midR;
				col1 = midC + radius;
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
		#endregion
	}
}
