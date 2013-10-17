using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI;
using System.Data.Linq;
using System.Linq;
using NLog;

namespace MeasureModule
{
	/// <summary>
	/// 量測輔助計算類型
	/// </summary>
	public enum CalcuteType { None, Distance, Point3ToCircle, Angle, SymmetryLine, CrossPoint, DistanceX, DistanceY };

	public static class DistanceHelper
	{
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
		public static DistanceResult LineToLine(Measurement first, Measurement second, HTuple angle, LineDirection direction)
		{
			//get value
			var line1 = first.GetViewModel();
			var line2 = second.GetViewModel();

			//check
			if (line1 == null || line2 == null) return null;
			if (line1.Row1 == null && line1.Row1.TupleLength() == 0
				&& line2.Row1 == null && line2.Row1.TupleLength() == 0) return null;

			//act
			HTuple distanceMin, distanceMax;
			HOperatorSet.DistanceSs(line1.Row1, line1.Col1, line1.Row2, line1.Col2, line2.Row1, line2.Col1, line2.Row2, line2.Col2
									, out distanceMin, out distanceMax);
			return new DistanceResult()
			{
				FirstRowBegin = new HTuple(line1.Row1),
				FirstColBegin = new HTuple(line1.Col1),
				FirstRowEnd = new HTuple(line1.Row2),
				FirstColEnd = new HTuple(line1.Col2),
				SecondRowBegin = new HTuple(line2.Row1),
				SecondColBegin = new HTuple(line2.Col1),
				SecondRowEnd = new HTuple(line2.Row2),
				SecondColEnd = new HTuple(line2.Col2),
				Direction = direction,
				Distance = new HTuple(distanceMin),
				Angle = angle,
			};
		}

		public static DistanceResult PointToLine(Measurement first, Measurement second, HTuple angle)
		{
			DistanceResult result = null;
			var firstData = first.GetViewModel();
			var secData = second.GetViewModel();
			if (firstData.Row1 != null && firstData.Row1.TupleLength() > 0 && secData.Row1 != null && secData.Row1.TupleLength() > 0)
			{
				HTuple distanceMin, distanceMax;

				var point = (firstData.Row2 != null) ? secData : firstData;
				var line = (firstData.Row2 != null) ? firstData : secData;

				HOperatorSet.DistancePs(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out distanceMin, out distanceMax);

				result = new DistanceResult()
				{
					FirstRowBegin = new HTuple(line.Row1),
					FirstColBegin = new HTuple(line.Col1),
					FirstRowEnd = new HTuple(line.Row2),
					FirstColEnd = new HTuple(line.Col2),
					SecondRowBegin = new HTuple(point.Row1),
					SecondColBegin = new HTuple(point.Col1),
					//SecondRowEnd = new HTuple(line2.Row2),
					//SecondColEnd = new HTuple(line2.Col2),
					Direction = LineDirection.Vertical,
					Distance = new HTuple(distanceMin),
					Angle = angle,
				};
			}
			return result;
		}

		public static DistanceResult PointToPoint(Measurement first, Measurement second, HTuple angle, LineDirection direction)
		{
			DistanceResult result = null;
			var p1 = first.GetViewModel();
			var p2 = second.GetViewModel();
			if (p1 == null || p2 == null) return null;

			if (p1.Row1 != null && p1.Row1.TupleLength() > 0
				&& p2.Row1 != null && p2.Row1.TupleLength() > 0)
			{
				HTuple distanceMin;
				HOperatorSet.DistancePp(p1.Row1, p1.Col1, p2.Row1, p2.Col1, out distanceMin);
				result = new DistanceResult()
				{
					Distance = distanceMin,
					Direction = direction,
					Angle = angle,
					FirstRowBegin = p1.Row1,
					FirstColBegin = p1.Col1,
					FirstRowEnd = p2.Row1,
					FirstColEnd = p2.Col1,
				};
			}


			return result;
		}

		/// <summary>
		/// 計算線與線的角度
		/// </summary>
		/// <param name="LineA"></param>
		/// <param name="LineB"></param>
		/// <returns></returns>
		public static AngleResult AngleLineToLine(Measurement LineA, Measurement LineB)
		{
			//get value
			var line1 = LineA.GetViewModel();
			var line2 = LineB.GetViewModel();

			return AngleLineToLine(line1, line2);
		}

		/// <summary>
		/// 計算線與線的角度
		/// </summary>
		/// <param name="line1">IMeasureGeoModel 線段1</param>
		/// <param name="line2">IMeasureGeoModel 線段2</param>
		/// <returns></returns>
		public static AngleResult AngleLineToLine(IMeasureGeoModel line1, IMeasureGeoModel line2)
		{
			//check
			if (line1 == null || line2 == null) return null;
			if (line1.Row1 == null && line1.Row1.TupleLength() == 0
				&& line2.Row1 == null && line2.Row1.TupleLength() == 0) return null;

			//act
			HTuple angle, firstPhi, secondPhi, interRow, interCol, isParallel;
			HOperatorSet.AngleLx(line1.Row1, line1.Col1, line1.Row2, line1.Col2, out firstPhi);

			HOperatorSet.AngleLx(line2.Row1, line2.Col1, line2.Row2, line2.Col2, out secondPhi);

			HOperatorSet.IntersectionLl(line1.Row1, line1.Col1, line1.Row2, line1.Col2, line2.Row1, line2.Col1, line2.Row2, line2.Col2,
				out interRow, out interCol, out isParallel);
			HOperatorSet.AngleLl(line1.Row1, line1.Col1, line1.Row2, line1.Col2, line2.Row1, line2.Col1, line2.Row2, line2.Col2, out angle);
			return new AngleResult()
			{
				Angle = angle,
				StartPhi = firstPhi,
				EndPhi = secondPhi,
				Row = interRow,
				Col = interCol,
			};
		}

		/// <summary>
		/// 回傳 Distance
		/// </summary>
		/// <param name="pointA"></param>
		/// <param name="pointB"></param>
		/// <returns></returns>
		public static double PointToPoint(IMeasureGeoModel pointA, IMeasureGeoModel pointB)
		{
			//check
			if (pointA == null || pointB == null) return 0;
			if (pointA.Row1 == null && pointA.Row1.TupleLength() == 0
				&& pointB.Row1 == null && pointB.Row1.TupleLength() == 0) return 0;

			//act
			HTuple distanceMin;
			HOperatorSet.DistancePp(pointA.Row1, pointA.Col1, pointB.Row1, pointB.Col1, out distanceMin);
			return distanceMin.D;
		}

		/// <summary>
		/// 計算距離
		/// </summary>
		/// <param name="firstModel"></param>
		/// <param name="secondModel"></param>
		/// <returns></returns>
		public static MeasureViewModel CaculateDistance(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			var sum = (int)firstModel.GeoType + (int)secondModel.GeoType;
			MeasureViewModel newModel = null;
			double distance = 0.0;
			switch (sum)
			{
				case 2:
				case 5:
				case 8:
				case 17:
				case 20:
				case 129:
					//point, point
					//point, circle
					//circle, circle
					//point, pointCircle
					//circle, pointCircle
					//crosspoint, point
					distance = PointToPoint(firstModel, secondModel);
					newModel = new MeasureViewModel()
					{
						Distance = distance,
						Row1 = firstModel.Row1,
						Col1 = firstModel.Col1,
						Row2 = secondModel.Row1,
						Col2 = secondModel.Col1,
					};
					break;
				case 3:
				case 6:
				case 9:
				case 12:
				case 18:
				case 24:
				case 65:
				case 68:
				case 80:
					//point, line
					//line, circle
					//distance, point
					//circle, distance
					//pointCircle, Line
					//pointCircle, distance
					//point, symmetryLine
					//circle, symmetryline
					//pointcircle, symmetryline
					var isLineModel = (firstModel.Row2 != null && firstModel.Row2.D >= 0.0);
					var point = (isLineModel) ? secondModel : firstModel;
					var line = (isLineModel) ? firstModel : secondModel;
					HTuple projRow, projCol;
					HOperatorSet.ProjectionPl(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out projRow, out projCol);
					distance = PointToLine(firstModel, secondModel);
					newModel = new MeasureViewModel()
					{
						Distance = distance,
						Row1 = point.Row1,
						Col1 = point.Col1,
						Row2 = projRow,
						Col2 = projCol,
					};
					break;
				case 4:
				case 10:
				case 16:
				case 66:
				case 72:
				case 128:
					//line, line
					//distance, line
					//distance, distance
					//Symmetric line and line
					//Symmetric line, distance
					//Symetric line and Symetric line
					var model = GetTwoLineShortestPointModel(firstModel, secondModel);
					//distance = LineToLine(firstModel, secondModel);
					newModel = new MeasureViewModel()
					{
						Row1 = model.Row1,
						Row2 = model.Row2,
						Col1 = model.Col1,
						Col2 = model.Col2,
						Distance = model.Distance,
					};
					break;
			}
			return newModel;
		}

		public static double CircleToCircle(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			return PointToPoint(firstModel, secondModel);
		}

		public static double PointToCircle(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			return PointToPoint(firstModel, secondModel);
		}

		public static double LineToCircle(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			return PointToLine(firstModel, secondModel);
		}

		/// <summary>
		/// 取得兩線段之間最短的兩點模型
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <returns>line model</returns>
		public static IMeasureGeoModel GetTwoLineShortestPointModel(IMeasureGeoModel line1, IMeasureGeoModel line2)
		{
			MeasureViewModel shortestModel = null;

			try
			{
				var modelList = new List<IMeasureGeoModel>();
				//線段 1 的中點
				var line1CP = getMidPoint(line1);

				//線段 1 的中點在線段 2 上的投影模型
				var line1CPModel = getProjectionPlModel(line1CP, line2);
				modelList.Add(line1CPModel);


				//線段 1 起始點在線段 2 上的投影
				var line1StartPoint = new MeasureViewModel() { Row1 = line1.Row1, Col1 = line1.Col1 };
				var line1StartProjModel = getProjectionPlModel(line1StartPoint, line2);
				modelList.Add(line1StartProjModel);


				//線段 1 終點在線段 2 上的投影
				var line1EndPoint = new MeasureViewModel() { Row1 = line1.Row2, Col1 = line1.Col2 };
				var line1EndProjModel = getProjectionPlModel(line1EndPoint, line2);
				modelList.Add(line1EndProjModel);


				//線段 1 的中點
				var line2CP = getMidPoint(line2);

				//線段 1 的中點在線段 2 上的投影
				var line2CPProjModel = getProjectionPlModel(line2CP, line1);
				modelList.Add(line2CPProjModel);

				//線段 2 起始點在線段 1 上的投影
				var line2StartPoint = new MeasureViewModel() { Row1 = line2.Row1, Col1 = line2.Col1 };
				var line2StartProjModel = getProjectionPlModel(line2StartPoint, line1);
				modelList.Add(line2StartProjModel);

				//線段 2 終點在線段 1 上的投影
				var line2EndPoint = new MeasureViewModel() { Row1 = line2.Row2, Col1 = line2.Col2 };
				var line2EndProjModel = getProjectionPlModel(line2EndPoint, line1);
				modelList.Add(line2EndProjModel);
				shortestModel = modelList.OrderBy(p => p.Distance.D).Take(1).SingleOrDefault() as MeasureViewModel;

			}
			catch (Exception ex)
			{
				logger.Error("GetTwoLineShortestPointModel Exception: " + ex.Message);

			}
			return shortestModel;
		}
		/// <summary>
		/// 取得點到線的投影點模型
		/// </summary>
		/// <param name="point"></param>
		/// <param name="line"></param>
		/// <returns>point to Projection point Model ( 2 point, projectionPoint at (col2, row2))</returns>
		private static IMeasureGeoModel getProjectionPlModel(IMeasureGeoModel point, IMeasureGeoModel line)
		{
			MeasureViewModel model = null;
			HTuple rowProj, colProj, distance;
			if (point != null && line != null)
			{
				HOperatorSet.ProjectionPl(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out rowProj, out colProj);
				HOperatorSet.DistancePp(point.Row1, point.Col1, rowProj, colProj, out distance);
				model = new MeasureViewModel()
				{
					Row1 = new HTuple(point.Row1),
					Col1 = new HTuple(point.Col1),
					Row2 = new HTuple(rowProj),
					Col2 = new HTuple(colProj),
					Distance = new HTuple(distance),
				};
			}
			return model;
		}
		private static IMeasureGeoModel getMidPoint(IMeasureGeoModel line)
		{
			var midR = (line.Row1 + line.Row2) / 2.0;
			var midC = (line.Col1 + line.Col2) / 2.0;
			return new MeasureViewModel()
			{
				Row1 = new HTuple(midR),
				Col1 = new HTuple(midC),
			};
		}
		public static double LineToLine(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			var line1 = firstModel;
			var line2 = secondModel;


			var model = GetTwoLineShortestPointModel(line1, line2);


			//HTuple distanceMin, distanceMax;
			//HOperatorSet.DistanceSs(line1.Row1, line1.Col1, line1.Row2, line1.Col2, line2.Row1, line2.Col1, line2.Row2, line2.Col2, out distanceMin, out distanceMax);
			return model.Distance;
		}


		public static double PointToLine(IMeasureGeoModel firstModel, IMeasureGeoModel secondModel)
		{
			HTuple distanceMin, distanceMax;
			var isLineModel = (firstModel.Row2 != null && firstModel.Row2.D >= 0.0);
			var point = (isLineModel) ? secondModel : firstModel;
			var line = (isLineModel) ? firstModel : secondModel;

			/*
			 直線方程式 p1 (x1,y1), p2(x2,y2)
			 y-y1      y2-y1 (dy)
			 ------ = --------
			 x-x1      x2-x1 (dx)
			 
			 y-y1 = (dy/dx) * (x-x1)
			 y = (dy/dx) * (x-x1) + y1
			 * 
			 y = 
		
			var dy = line.Row2 - line.Row1;
			var dx = line.Col2 - line.Col1;
				 */

			HTuple projRow, projCol;
			HOperatorSet.ProjectionPl(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out projRow, out projCol);
			HOperatorSet.DistancePp(point.Row1, point.Col1, projRow, projCol, out distanceMin);
			//HOperatorSet.DistancePs(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out distanceMin, out distanceMax);
			return distanceMin;
		}

		/// <summary>
		/// 擬合圓
		/// </summary>
		/// <param name="f_radius">圓 ROI 半徑</param>
		/// <param name="f_ROI_Cur_Row">圓 ROI Row 座標</param>
		/// <param name="f_ROI_Cur_Col">圓 ROI Col 座標</param>
		/// <returns></returns>
		public static MeasureViewModel GetFitCircleModel(HObject ho_image, HTuple f_radius, HTuple f_ROI_Cur_Row, HTuple f_ROI_Cur_Col)
		{
			var cROIController = new ROIController();
			var cAssistant = new MeasureAssistant(cROIController);

			var hImage = ho_image as HImage;
			cAssistant.setImage(hImage);

			/*參數值*/
			cAssistant.mThresh = 40.0;
			cAssistant.mSigma = 1.0;
			cAssistant.mRoiWidth = 10;
			cAssistant.mInterpolation = "nearest_neighbor";
			cAssistant.mSelPair = false;
			cAssistant.mTransition = "all";
			cAssistant.mPosition = "all";
			cAssistant.mDispEdgeLength = 30;
			cAssistant.mDispROIWidth = true;
			cAssistant.setUnit("cm");

			cAssistant.mInitThresh = 40.0;
			cAssistant.mInitSigma = 1.0;
			cAssistant.mInitRoiWidth = 10;

			var roiF = new ROICircle() { ROIMeasureType = MeasureType.Circle };
			//roiF.MakeROI(416, 998, 0, 26.5, 71.2);
			roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_radius);


			var fitCircle = new MeasurementCircle(roiF, cAssistant);
			var model = fitCircle.GetViewModel();
			return model;
		}


		/// <summary>
		/// 有中心點及長度，制作幾何線段
		/// </summary>
		/// <param name="centerRow">中心點 row</param>
		/// <param name="centerCol">中心點 col</param>
		/// <param name="phi">角度</param>
		/// <param name="halfDistance">一半的長度</param>
		public static MeasureViewModel MakeLine(HTuple centerRow, HTuple centerCol, HTuple phi, HTuple halfDistance)
		{
			double row1, row2, col1, col2;

			row1 = centerRow - halfDistance * Math.Sin(phi + 0.5 * Math.PI);
			col1 = centerCol + halfDistance * Math.Cos(phi + 0.5 * Math.PI);
			row2 = centerRow - halfDistance * Math.Sin(phi + 1.5 * Math.PI);
			col2 = centerCol + halfDistance * Math.Cos(phi + 1.5 * Math.PI);
			MeasureViewModel result = new MeasureViewModel()
			{
				Row1 = row1,
				Row2 = row2,
				Col1 = col1,
				Col2 = col2,
				Distance = halfDistance * 2
			};
			return result;
		}

		/// <summary>
		/// 計算兩線段的對稱線
		/// </summary>
		/// <param name="modelOne"></param>
		/// <param name="modelTwo"></param>
		/// <returns></returns>
		public static LineResult CalculateSymmetryLine(IMeasureGeoModel modelOne, IMeasureGeoModel modelTwo)
		{
			LineResult result = null;

			//取得第一條線段的中點
			var pointModel = new MeasureViewModel()
			{
				Row1 = (modelOne.Row1 + modelOne.Row2) / 2.0,
				Col1 = (modelOne.Col1 + modelOne.Col2) / 2.0,
			};

			var lineModel = modelTwo;

			//計算第一線段中點到第二線段上的投影點(垂直距離)
			HTuple rowProj, colProj;
			HOperatorSet.ProjectionPl(pointModel.Row1, pointModel.Col1, lineModel.Row1, lineModel.Col1, lineModel.Row2, lineModel.Col2, out rowProj, out colProj);

			//計算第一線段中點與第二線段上的投影點距離 (point to point)
			HTuple distance;
			HOperatorSet.DistancePp(rowProj, colProj, pointModel.Row1, pointModel.Col1, out distance);

			//取兩線中點
			var cRow = (pointModel.Row1 + rowProj) / 2.0;
			var cCol = (pointModel.Col1 + colProj) / 2.0;

			//取角度
			HTuple angle;
			HOperatorSet.AngleLx(lineModel.Row1, lineModel.Col1, lineModel.Row2, lineModel.Col2, out angle);

			//線段
			double[] rows, cols;
			DetermineLine(cRow, cCol, angle, distance, out rows, out cols);
			result = new LineResult()
			{
				Row1 = rows[0],
				Row2 = rows[1],
				Col1 = cols[0],
				Col2 = cols[1],
				Distance = distance,
			};

			return result;
		}

		public static void DetermineLine(double row, double col, double phi, double width, out double[] rows, out double[] cols)
		{
			double row1, row2, col1, col2;

			row1 = row - width * Math.Sin(phi);
			col1 = col + width * Math.Cos(phi);
			row2 = row - width * Math.Sin(phi + Math.PI);
			col2 = col + width * Math.Cos(phi + Math.PI);
			rows = new double[] { row1, row2 };
			cols = new double[] { col1, col2 };
		}

		/// <summary>
		/// 計算兩線段的交點
		/// </summary>
		/// <param name="geoModelOne"></param>
		/// <param name="geoModelTwo"></param>
		/// <returns></returns>
		public static PointResult IntersetionLine(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo)
		{
			PointResult result = null;

			HTuple row, col, isParallel;
			HOperatorSet.IntersectionLl(geoModelOne.Row1, geoModelOne.Col1, geoModelOne.Row2, geoModelOne.Col2,
										geoModelTwo.Row1, geoModelTwo.Col1, geoModelTwo.Row2, geoModelTwo.Col2,
										out row, out col, out isParallel);
			result = new PointResult()
			{
				Row1 = row,
				Col1 = col,
				IsParallel = isParallel,
			};
			return result;
		}

		/// <summary>
		/// 計算兩點 X 方向距離
		/// </summary>
		/// <param name="_geoModelOne">點 1</param>
		/// <param name="_geoModelTwo">點 2</param>
		/// <returns></returns>
		public static LineResult DistanceX(IMeasureGeoModel _geoModelOne, IMeasureGeoModel _geoModelTwo)
		{
			LineResult result = null;
			var distanceX = Math.Abs(_geoModelOne.Col1.D - _geoModelTwo.Col1.D);


			result = new LineResult()
			{
				Col1 = _geoModelOne.Col1,
				Col2 = _geoModelTwo.Col1,
				Row1 = _geoModelOne.Row1,
				Row2 = _geoModelOne.Row1,
				Distance = distanceX,
			};
			return result;
		}

		/// <summary>
		/// 計算兩點 Y 方向距離
		/// </summary>
		/// <param name="_geoModelOne">點 1</param>
		/// <param name="_geoModelTwo">點 2</param>
		/// <returns></returns>
		public static LineResult DistanceY(IMeasureGeoModel _geoModelOne, IMeasureGeoModel _geoModelTwo)
		{
			LineResult result = null;
			var distanceY = Math.Abs(_geoModelOne.Row1.D - _geoModelTwo.Row1.D);
			result = new LineResult()
			{
				Col1 = _geoModelOne.Col1,
				Col2 = _geoModelOne.Col1,
				Row1 = _geoModelOne.Row1,
				Row2 = _geoModelTwo.Row1,
				Distance = distanceY,
			};
			return result;
		}

		public static HObject GenContour(ViewModel.GeoDataGridViewModel geoModel)
		{
			HObject displayObject = null;
			switch (geoModel.GeoType)
			{
				case MeasureType.Distance:
				case MeasureType.DistanceX:
				case MeasureType.DistanceY:
					double[] rows = new double[] { geoModel.Row1, geoModel.Row2 };
					double[] cols = new double[] { geoModel.Col1, geoModel.Col2 };
					HOperatorSet.GenContourPolygonXld(out displayObject, rows, cols);
					break;
				case MeasureType.PointCircle:
					HOperatorSet.GenCircle(out displayObject, geoModel.Row1, geoModel.Col1, geoModel.Distance);
					break;
				case MeasureType.CrossPoint:
					HOperatorSet.GenCrossContourXld(out displayObject, geoModel.Row1, geoModel.Col1, 25, 0.785398);
					break;
			}
			return displayObject;
		}

		/// <summary>
		/// 找到最接近 ROI 大小的 Radius
		/// </summary>
		/// <param name="radiusArr"></param>
		/// <param name="areaPixels"></param>
		/// <returns></returns>
		public static int GetApproximateRadiusIndex(double[] radiusArr, double areaPixels)
		{
			var index = -1;
			try
			{
				var orderedList = radiusArr.Select(p => new
					{
						Radius = p,
						Area = getAreaPixel(p)
					}).OrderByDescending(q => q.Area);

				var minDiff = Double.MaxValue;
				var prevDiff = minDiff;
				var result = 0.0;
				foreach (var item in orderedList)
				{
					var curDiff = (double)Math.Abs(item.Area - areaPixels);

					if (curDiff < minDiff)
						minDiff = curDiff;
					if (prevDiff > minDiff)
					{
						prevDiff = minDiff;
						result = item.Radius;
					}
					else
					{
						break;
					}
				}
				for (int i = 0; i < radiusArr.Length; i++)
				{
					if (radiusArr[i] == result)
					{
						index = i;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return index;
		}

		private static double getAreaPixel(double p)
		{
			HObject circle;
			HOperatorSet.GenEmptyObj(out circle);
			circle.Dispose();

			HTuple area, row, column;

			HOperatorSet.GenCircle(out circle, 0, 0, p);
			HOperatorSet.AreaCenter(circle, out area, out row, out column);
			return area.D;
		}

	}
}
