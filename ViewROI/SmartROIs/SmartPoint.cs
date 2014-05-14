using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Interface;
using ViewROI.Model;

namespace ViewROI.SmartROIs
{
	public class SmartPoint : ROI, IContinueZoom, IROIModelUpdateable
	{
		#region private variables =========================================================
		/// <summary>Half length of the rectangle side, perpendicular to phi</summary>
		private double length1;

		/// <summary>Half length of the rectangle side, in direction of phi</summary>
		private double length2;

		/// <summary>Row coordinate of midpoint of the rectangle</summary>
		private double midR;

		/// <summary>Column coordinate of midpoint of the rectangle</summary>
		private double midC;

		/// <summary>Orientation of rectangle defined in radians.</summary>
		private double phi;

		private double creakPointPhi;

		//auxiliary variables
		private HTuple rowsInit;
		private HTuple colsInit;
		private HTuple rows;
		private HTuple cols;

		private HHomMat2D hom2D, tmp;

		/// <summary>
		/// ROI 寬度, 預設為 10 pixels
		/// <para>以點選位置向外延伸的 pixel 大小</para>
		/// </summary>
		private int _smartWidth = 10;
		#endregion =========================================================================

		/// <summary>
		/// 以 ROIRectangle2 為範本, 實作 IContinueZoom, 達成在影像上點選 1 點 形成 ROI 的操作
		/// <para>預設 ROI 大小為以點選位置向外延伸 10 pixel</para>
		/// </summary>
		public SmartPoint()
		{
			NumHandles = 6; // 4 corners +  1 midpoint + 1 rotationpoint			
			activeHandleIdx = 4;
			this.ROIMeasureType = MeasureType.Point; // 預設量測類型			
			_clickedPointsPositionList = new List<PositionModel>();
		}
		/// <summary>Creates a new ROI instance at the mouse position</summary>
		/// <param name="midX">
		/// x (=column) coordinate for interactive ROI
		/// </param>
		/// <param name="midY">
		/// y (=row) coordinate for interactive ROI
		/// </param>
		public override void createROI(double midX, double midY)
		{

		}

		/// <summary>
		/// 重設 ROI 資訊
		/// </summary>
		/// <param name="roiRow">中心點 Row</param>
		/// <param name="roiCol">中心點 Col</param>
		/// <param name="roiPhi">角度</param>
		/// <param name="roiLength">half 長</param>
		/// <param name="roiWidth">half 寬</param>
		public void MakeROI(double roiRow, double roiCol, double roiPhi, double roiLength, double roiWidth)
		{
			midR = roiRow;
			midC = roiCol;

			length1 = roiLength;
			length2 = roiWidth;

			phi = roiPhi;

			rowsInit = new HTuple(new double[] {-1.0, -1.0, 1.0, 
												   1.0,  0.0, 0.0 });
			colsInit = new HTuple(new double[] {-1.0, 1.0,  1.0, 
												  -1.0, 0.0, 0.6 });
			//order        ul ,  ur,   lr,  ll,   mp, arrowMidpoint
			hom2D = new HHomMat2D();
			tmp = new HHomMat2D();

			updateHandlePos();
		}
		public override ROIViewModel ToROIViewModel()
		{
			return new ROIViewModel()
			{
				CenterRow = midR,
				CenterCol = midC,
				Phi = phi,
				Length = length1,
				Width = length2,
				ROIType = ROI.ROI_TYPE_RECTANGLE2,
				ID = ID,
			};
		}

		/// <summary>
		/// 重設 ROI 資訊
		/// </summary>
		/// <param name="model"></param>
		public override void MakeROI(ROIViewModel model)
		{
			MakeROI(model.CenterRow, model.CenterCol, model.Phi, model.Length, model.Width);
		}

		/// <summary>Paints the ROI into the supplied window</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			if (_initPointsDone)
			{
				//畫中心點
				window.DispRectangle2(midR, midC, -phi, length1, length2);

				//畫其它的控制點
				//for (int i = 0; i < NumHandles; i++)
				//	window.DispRectangle2(rows[i].D, cols[i].D, -phi, 5, 5);

			}
		}

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		/// <returns> 
		/// Distance of the closest ROI handle.
		/// </returns>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			if (!_initPointsDone) return max;

			double[] val = new double[NumHandles];
			for (int i = 0; i < NumHandles; i++)
				val[i] = HMisc.DistancePp(y, x, rows[i].D, cols[i].D);

			for (int i = 0; i < NumHandles; i++)
			{
				if (val[i] < max)
				{
					max = val[i];
					activeHandleIdx = i;
				}
			}
			return val[activeHandleIdx];
		}

		/// <summary> 
		/// Paints the active handle of the ROI object into the supplied window
		/// </summary>
		/// <param name="window">HALCON window</param>
		public override void displayActive(HalconDotNet.HWindow window)
		{
			if (!_initPointsDone) return;
			if (activeHandleIdx == 4)
			{
				window.DispRectangle2(rows[activeHandleIdx].D,
									  cols[activeHandleIdx].D,
									  -phi, 5, 5);
			}
			/*
			if (activeHandleIdx == 5)
				window.DispArrow(midR, midC,
								 midR + (Math.Sin(phi) * length1 * 1.2),
								 midC + (Math.Cos(phi) * length1 * 1.2),
								 2.0);
			 */
		}


		/// <summary>Gets the HALCON region described by the ROI</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRectangle2(midR, midC, -phi, length1, length2);
			return region;
		}

		/// <summary>
		/// Gets the model information described by 
		/// the interactive ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { midR, midC, phi, length1, length2 });
		}

		/// <summary> 
		/// Recalculates the shape of the ROI instance. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y)
		/// </summary>
		/// <param name="newX">x mouse coordinate</param>
		/// <param name="newY">y mouse coordinate</param>
		public override void moveByHandle(double newX, double newY)
		{
			switch (activeHandleIdx)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					/*
					double x = 0, y = 0;
					tmp = hom2D.HomMat2dInvert();
					x = tmp.AffineTransPoint2d(newX, newY, out y);

					length2 = Math.Abs(y);
					length1 = Math.Abs(x);

					checkForRange(x, y);
					 */
					break;
				case 4:
					midC = newX;
					midR = newY;
					break;
				case 5:
					/*
					var vY = newY - rows[4].D;
					var vX = newX - cols[4].D;
					phi = Math.Atan2(vY, vX);
					 */
					break;
			}
			updateHandlePos();
		}//end of method

		/// <summary>
		/// Auxiliary method to recalculate the contour points of 
		/// the rectangle by transforming the initial row and 
		/// column coordinates (rowsInit, colsInit) by the updated
		/// homography hom2D
		/// </summary>
		private void updateHandlePos()
		{
			hom2D.HomMat2dIdentity();
			hom2D = hom2D.HomMat2dTranslate(midC, midR);
			hom2D = hom2D.HomMat2dRotateLocal(phi);
			tmp = hom2D.HomMat2dScaleLocal(length1, length2);
			cols = tmp.AffineTransPoint2d(colsInit, rowsInit, out rows);
		}


		/* This auxiliary method checks the half lengths 
		 * (length1, length2) using the coordinates (x,y) of the four 
		 * rectangle corners (handles 0 to 3) to avoid 'bending' of 
		 * the rectangular ROI at its midpoint, when it comes to a
		 * 'collapse' of the rectangle for length1=length2=0.
		 * */
		private void checkForRange(double x, double y)
		{
			switch (activeHandleIdx)
			{
				case 0:
					if ((x < 0) && (y < 0))
						return;
					if (x >= 0) length1 = 0.01;
					if (y >= 0) length2 = 0.01;
					break;
				case 1:
					if ((x > 0) && (y < 0))
						return;
					if (x <= 0) length1 = 0.01;
					if (y >= 0) length2 = 0.01;
					break;
				case 2:
					if ((x > 0) && (y > 0))
						return;
					if (x <= 0) length1 = 0.01;
					if (y <= 0) length2 = 0.01;
					break;
				case 3:
					if ((x < 0) && (y > 0))
						return;
					if (x >= 0) length1 = 0.01;
					if (y <= 0) length2 = 0.01;
					break;
				default:
					break;
			}
		}

		#region 實作 IContinueZoom  ===============================================================================

		private int _clickPoints = 1; //應點擊的次數
		private List<PositionModel> _clickedPointsPositionList;
		private bool _initPointsDone = false;
		public bool WaitForClickPoints(double x, double y)
		{
			if (_clickedPointsPositionList.Count < _clickPoints)
			{
				_clickedPointsPositionList.Add(new PositionModel() { ColBegin = x, RowBegin = y });
			}
			_initPointsDone = _clickedPointsPositionList.Count == _clickPoints;
			if (_initPointsDone)
			{
				//產生 Rectangle 資訊
				midR = _clickedPointsPositionList.Average(p => p.RowBegin);
				midC = _clickedPointsPositionList.Average(p => p.ColBegin);

				length1 = _smartWidth;
				length2 = _smartWidth;

				//決定角度
				phi = 0.0;

				rowsInit = new HTuple(new double[] {-1.0, -1.0, 1.0, 
												   1.0,  0.0, 0.0 });
				colsInit = new HTuple(new double[] {-1.0, 1.0,  1.0, 
												  -1.0, 0.0, 0.6 });
				//order        ul ,  ur,   lr,  ll,   mp, arrowMidpoint
				hom2D = new HHomMat2D();
				tmp = new HHomMat2D();

				updateHandlePos();
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

		public void UpdateROIModel(ROIViewModel updateModel)
		{
			if (updateModel != null)
			{
				this.length1 = updateModel.Length;
				this.length2 = updateModel.Width;
				this.phi = updateModel.Phi;
				this.midR = updateModel.CenterRow;
				this.midC = updateModel.CenterCol;
			}
		}
	}
}
