using System;
using HalconDotNet;

namespace ViewROI
{
	public enum AuxLineType { Horizontal, Vertical };
	/// <summary>
	/// 垂直或水平的輔助線
	/// </summary>
	public class ROIAuxLine : ROI
	{

		private double _StartRow, _StartColumn;   // first end point of line
		private double _EndRow, _EndCol;   // second end point of line
		private double _MidRow, _MidCol;   // midPoint of line
		private double _WinWidth, _WinHeight;	//HWindow Width, Height
		private AuxLineType _LineType { get; set; }

		private HXLDCont _AuxLineHandleXLD;

		public ROIAuxLine(double hWinImagePartWidth, double hWinImagePartHeight, AuxLineType lineType)
		{
			NumHandles = 1;        // one Control points of line
			activeHandleIdx = 2;
			_AuxLineHandleXLD = new HXLDCont();
			_AuxLineHandleXLD.GenEmptyObj();
			_WinWidth = hWinImagePartWidth;
			_WinHeight = hWinImagePartHeight;
			_LineType = lineType;
		}

		public string GetLineType()
		{
			return _LineType.ToString();
		}

		/// <summary>
		/// 取得中點位置 double[0] = MidRow, double[1] = MidCol
		/// </summary>
		/// <returns></returns>
		public double[] GetMidPoint()
		{
			return new double[] { _MidRow, _MidCol };
		}

		/// <summary>
		/// 設定中點位置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetMidPoint(double x, double y)
		{
			_MidRow = y;
			_MidCol = x;
		}

		/// <summary>Creates a new ROI instance at the mouse position.</summary>
		public override void createROI(double midX, double midY)
		{
			_MidRow = midX;
			_MidCol = midY;
			if (_LineType == AuxLineType.Horizontal)
			{
				_StartRow = _MidRow;
				_StartColumn = 0;
				_EndRow = _MidRow;
				_EndCol = _WinWidth;
			}
			else if (_LineType == AuxLineType.Vertical)
			{
				_StartRow = 0;
				_StartColumn = _MidCol;

				_EndRow = _WinHeight;
				_EndCol = _MidCol;
			}
			updateArrowHandle();
		}

		/// <summary>Paints the ROI into the supplied window.</summary>
		public override void draw(HalconDotNet.HWindow window)
		{

			window.DispLine(_StartRow, _StartColumn, _EndRow, _EndCol);

			//window.DispRectangle2(_StartRow, _StartColumn, 0, 5, 5);//開始
			//window.DispObj(_AuxLineHandleXLD);  //window.DispRectangle2( row2, col2, 0, 5, 5); //箭頭

			//window.DispRectangle2(_MidRow, _MidCol, 0, 5, 5);//中間
		}

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y).
		/// </summary>
		public override double distToClosestHandle(double x, double y)
		{
			var dist = HMisc.DistancePl(y, x, _StartRow, _StartColumn, _EndRow, _EndCol);
			return dist;
		}

		/// <summary> 
		/// Paints the active handle of the ROI object into the supplied window. 
		/// </summary>
		public override void displayActive(HalconDotNet.HWindow window)
		{

			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(_StartRow, _StartColumn, 0, 5, 5);
					break;
				case 1:
					window.DispObj(_AuxLineHandleXLD); //window.DispRectangle2(row2, col2, 0, 5, 5);
					break;
				case 2:
					//window.DispRectangle2(_MidRow, _MidCol, 0, 5, 5);
					break;
			}
		}

		/// <summary>Gets the HALCON region described by the ROI.</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRegionLine(_StartRow, _StartColumn, _EndRow, _EndCol);
			return region;
		}

		public override double getDistanceFromStartPoint(double row, double col)
		{
			double distance = HMisc.DistancePp(row, col, _StartRow, _StartColumn);
			return distance;
		}
		/// <summary>
		/// Gets the model information described by 
		/// the ROI.
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { _StartRow, _StartColumn, _EndRow, _EndCol });
		}

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y).
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			switch (activeHandleIdx)
			{
				case 0: // first end point
					_StartRow = newY;
					_StartColumn = newX;

					_MidRow = (_StartRow + _EndRow) / 2;
					_MidCol = (_StartColumn + _EndCol) / 2;
					break;
				case 1: // last end point
					_EndRow = newY;
					_EndCol = newX;

					_MidRow = (_StartRow + _EndRow) / 2;
					_MidCol = (_StartColumn + _EndCol) / 2;
					break;
				case 2: // midpoint 
					
					if (_LineType == AuxLineType.Horizontal)
					{
						//水平線移動, X 不變, Y 可變
						_StartRow = newY;
						_MidRow = newY;
						_EndRow = newY;
					}
					else if (_LineType == AuxLineType.Vertical)
					{
						//垂直線移動, Y 不變, X 可變
						_StartColumn = newX;
						_MidCol = newX;
						_EndCol = newX;
					}
					

					break;
			}
			updateArrowHandle();
		}


		/// <summary> Auxiliary method </summary>
		private void updateArrowHandle()
		{
			double length, dr, dc, halfHW;
			double rrow1, ccol1, rowP1, colP1, rowP2, colP2;

			double headLength = 15;
			double headWidth = 15;


			_AuxLineHandleXLD.Dispose();
			_AuxLineHandleXLD.GenEmptyObj();

			rrow1 = _StartRow + (_EndRow - _StartRow) * 0.8;
			ccol1 = _StartColumn + (_EndCol - _StartColumn) * 0.8;

			length = HMisc.DistancePp(rrow1, ccol1, _EndRow, _EndCol);
			if (length == 0)
				length = -1;

			dr = (_EndRow - rrow1) / length;
			dc = (_EndCol - ccol1) / length;

			halfHW = headWidth / 2.0;
			rowP1 = rrow1 + (length - headLength) * dr + halfHW * dc;
			rowP2 = rrow1 + (length - headLength) * dr - halfHW * dc;
			colP1 = ccol1 + (length - headLength) * dc - halfHW * dr;
			colP2 = ccol1 + (length - headLength) * dc + halfHW * dr;

			if (length == -1)
				_AuxLineHandleXLD.GenContourPolygonXld(rrow1, ccol1);
			else
				_AuxLineHandleXLD.GenContourPolygonXld(new HTuple(new double[] { rrow1, _EndRow, rowP1, _EndRow, rowP2, _EndRow }),
													new HTuple(new double[] { ccol1, _EndCol, colP1, _EndCol, colP2, _EndCol }));
		}

	}//end of class
}//end of namespace
