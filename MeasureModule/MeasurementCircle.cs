using HalconDotNet;
using System;
using ViewROI;

namespace MeasureModule
{
	public class MeasurementCircle : Measurement
	{
		private CircleResult mResult;
		private CircleResult mResultWorld;
		private HTuple _cameraOut;

		#region Fit Circle 演算法參數
		/// <summary>
		/// Algorithm for the fitting of circles
		/// List of values: 'algebraic', 'ahuber', 'atukey', 'geometric', 'geohuber', 'geotukey' 
		/// In practice, the approach of Tukey is recommended. 
		/// </summary>
		private HTuple _Algorithm = new HTuple("atukey");

		/// <summary>
		/// Maximum number of contour points used for the computation (-1 for all points).
		/// </summary>
		private HTuple _MaxNumPoints = new HTuple(-1);

		/// <summary>
		/// Maximum distance between the end points of a contour to be considered as 'closed'.
		/// </summary>
		private HTuple _MaxClosureDist = new HTuple(2);

		/// <summary>
		/// Number of points at the beginning and at the end of the contours to be ignored for the fitting.
		/// </summary>
		private HTuple _ClippingEndPoints = new HTuple(0);

		/// <summary>
		/// Maximum number of iterations for the robust weighted fitting.
		/// </summary>
		private HTuple _Iterations = new HTuple(10);

		/// <summary>
		/// Clipping factor for the elimination of outliers (typical: 1.0 for Huber and 2.0 for Tukey).
		/// The smaller the value choosen for factor, the more outliers are detected.
		/// </summary>
		private HTuple _ClippingFactor = new HTuple(2.0);
		#endregion

		public void SetAlgorithm(string algo)
		{
			if (!String.IsNullOrEmpty(algo))
				_Algorithm = new HTuple(algo);
		}

		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="roi"></param>
		/// <param name="mAssist"></param>
		public MeasurementCircle(ROI roi, MeasureAssistant mAssist)
			: base(roi, mAssist)
		{
			mResult = new CircleResult();
			_cameraOut = new HTuple();
			UpdateMeasure();
		}


		/// <summary>
		/// 更新量測結果.
		/// 量測演算法放這裡
		/// </summary>
		public override void UpdateResults()
		{
			if (mMeasAssist.mImage == null) return;


			#region 初始化數值
			if (!String.IsNullOrEmpty(mMeasAssist.FitCircleAlgo))
				_Algorithm = new HTuple(mMeasAssist.FitCircleAlgo);

			//init result
			mResult = new CircleResult()
			{
				Radius = new HTuple(),
				Row = new HTuple(),
				Col = new HTuple(),
				StartPhi = new HTuple(),
				EndPhi = new HTuple(),
				PointOrder = new HTuple(),
			};

			// Local iconic variables 
			HObject ho_MeasureROI, ho_ImageReduced;
			HObject ho_Border, tmp_Border;

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_MeasureROI);
			HOperatorSet.GenEmptyObj(out ho_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_Border);
			HOperatorSet.GenEmptyObj(out tmp_Border);
			#endregion

			try
			{
				//******* Create ROI ********
				ho_MeasureROI.Dispose();
				HOperatorSet.GenCircle(out ho_MeasureROI, mROICoord[0], mROICoord[1], mROICoord[2]);
				HRegion region = new HRegion();
				region.GenCircle(mROICoord[0].D, mROICoord[1].D, mROICoord[2].D);

				//****** Area Center, 取得目標區域的 pixels 數 ***********
				HTuple area, areaRow, areaColumn;
				double areaPixels = 0.0;
				HOperatorSet.AreaCenter(region, out area, out areaRow, out areaColumn);
				areaPixels = area.D;

				//****** 取得影像 ******
				var himage = mMeasAssist.getImage();

				//******* Extract ROI Image *****
				ho_ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(himage, ho_MeasureROI, out ho_ImageReduced);

				//******* Filter ****************
				var subpixThreadhold = mMeasAssist.SubpixThreadhold;
				HOperatorSet.GenEmptyObj(out ho_Border);
				HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border, subpixThreadhold);

				if (mMeasAssist.ApplyCalibration && mMeasAssist.IsCalibrationValid)
				{
					if (_cameraOut.TupleLength() == 0)
					{
						_cameraOut = HMisc.ChangeRadialDistortionCamPar("adaptive", mMeasAssist.CameraIn, 0.0);
					}
					//var rectifyImage = himage.ChangeRadialDistortionImage(region, mMeasAssist.CameraIn, cameraOut);
					HObject calibrationBorder;
					HOperatorSet.GenEmptyObj(out calibrationBorder);
					HOperatorSet.ChangeRadialDistortionContoursXld(ho_Border, out calibrationBorder, mMeasAssist.CameraIn, _cameraOut);
					mResult = fitCircle(calibrationBorder, areaPixels);
				}
				else
				{
					mResult = fitCircle(ho_Border, areaPixels);
				}

			}
			catch (HOperatorException ex)
			{
				Hanbo.Log.LogManager.Error(ex);
				mResultWorld = new CircleResult();
				mResult = new CircleResult();
			}
			finally
			{
				ho_MeasureROI.Dispose();
				ho_ImageReduced.Dispose();
				ho_Border.Dispose();
			}
			UpdateXLD();
		}

		private CircleResult fitCircle(HObject ho_Border, double areaPixels)
		{
			CircleResult result = new CircleResult()
			{
				Radius = new HTuple(),
				Row = new HTuple(),
				Col = new HTuple(),
				StartPhi = new HTuple(),
				EndPhi = new HTuple(),
				PointOrder = new HTuple(),
			};
			try
			{
				// Local control variables 
				HTuple hv_Row, hv_Column, hv_Radius, hv_StartPhi;
				HTuple hv_EndPhi, hv_PointOrder;

				//******* Choice Candidate Objects
				HObject ho_SelectedContours;
				HOperatorSet.GenEmptyObj(out ho_SelectedContours);
				ho_SelectedContours.Dispose();
				HOperatorSet.SelectContoursXld(ho_Border, out ho_SelectedContours, "contour_length",
					10, Int16.MaxValue, -0.5, 0.5);

				//******* Fit Circle ************
				HTuple hv_Number, hv_Index;
				HObject ho_ObjectSelected;
				HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
				HOperatorSet.CountObj(ho_SelectedContours, out hv_Number);
				var resultRadius = 0.0;
				for (hv_Index = 1; hv_Index.Continue(hv_Number, 1); hv_Index = hv_Index.TupleAdd(1))
				{
					ho_ObjectSelected.Dispose();
					HOperatorSet.SelectObj(ho_SelectedContours, out ho_ObjectSelected, hv_Index);
					HOperatorSet.FitCircleContourXld(ho_SelectedContours
					, _Algorithm, _MaxNumPoints, _MaxClosureDist, _ClippingEndPoints, _Iterations, _ClippingFactor
					, out hv_Row, out hv_Column, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);

					//Answer
					var radiusIndex = -1;
					if (hv_Radius.TupleLength() > 0)
					{
						radiusIndex = DistanceHelper.GetApproximateRadiusIndex(hv_Radius.DArr, areaPixels);
						if (radiusIndex > -1)
						{
							//取最大的 Circle
							if (resultRadius < hv_Radius.DArr[radiusIndex])
							{
								result = new CircleResult(new HTuple(hv_Row.DArr[radiusIndex])
															, new HTuple(hv_Column.DArr[radiusIndex])
															, new HTuple(hv_Radius.DArr[radiusIndex])
															, hv_StartPhi
															, hv_EndPhi
															, hv_PointOrder) { };
								resultRadius = hv_Radius.DArr[radiusIndex];
							}
						}
					}
				}
			}
			catch (HOperatorException ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
			return result;
		}
		/// <summary>
		/// 顯示 Measure 的幾何元素外觀
		/// </summary>
		public override void UpdateXLD()
		{
			//clear
			mEdgeXLD.Dispose();
			mEdgeXLD.GenEmptyObj();


			if (mResult.Row == null) return;

			try
			{
				if (mResult.Row.Length > 0)
				{
					for (int i = 0; i < mResult.Row.Length; i++)
					{
						var circleXLD = new HXLDCont();
						var radius = mResult.Radius[i].D;
						//var radius = diameter / 2.0; // 
						circleXLD.GenCircleContourXld(mResult.Row[i].D, mResult.Col[i].D, radius, 0.0, 6.28318, "positive", 1.0);

						//output
						mEdgeXLD = mEdgeXLD.ConcatObj(circleXLD);
					}
				}
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
		}
		public override MeasureResult getMeasureResultData()
		{
			return mResult;
		}
		public override void ClearResultData()
		{
			mResultWorld = new CircleResult();
		}

		/// <summary>
		/// 量測結果的 ViewModel
		/// </summary>
		/// <returns></returns>
		public override MeasureViewModel GetViewModel()
		{
			return new MeasureViewModel()
			{
				GeoType = MeasureType.Circle,
				Row1 = mResult.Row,
				Col1 = mResult.Col,
				Distance = mResult.Radius,
				StartPhi = mResult.StartPhi,
				EndPhi = mResult.EndPhi,
				PointOrder = mResult.PointOrder,
			};
		}
	}
}
