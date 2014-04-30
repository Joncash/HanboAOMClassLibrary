using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;
using Hanbo.Extensions;
namespace MeasureModule.SmartMeasurements
{
	public class AutoFitPoint : Measurement
	{
		#region private variables
		private EdgeResult mResult;
		private EdgeResult mResultWorld;
		private double _crossPointSize = 12.0;
		private HTuple _cameraOut;
		#endregion
		public bool HasResult
		{
			get
			{
				return (mResult.rowEdge != null && mResult.rowEdge.TupleLength() > 0);
			}
		}

		public AutoFitPoint(ROI roi, MeasureAssistant mAssist)
			: base(roi, mAssist)
		{
			mResult = new EdgeResult();
			mResultWorld = new EdgeResult();
			if (mMeasAssist.IsCalibrationValid)
			{
				_cameraOut = HMisc.ChangeRadialDistortionCamPar("adaptive", mMeasAssist.CameraIn, 0.0);
			}
			UpdateMeasure();

		}

		#region override methods ================================

		/// <summary>
		/// 演算法改這裡
		/// </summary>
		public override void UpdateResults()
		{
			if (mHandle == null)
				return;

			mMeasAssist.exceptionText = "";

			try
			{
				HObject imageReduced;
				HOperatorSet.GenEmptyObj(out imageReduced);
				//建 ROI
				var roiModel = mRoi.getModelData();
				HTuple row = roiModel[0];
				var column = roiModel[1];
				var phi = roiModel[2];
				var length1 = roiModel[3];
				var length2 = roiModel[4];
				//Halcon 徑度轉角度
				double degree = phi.D.HalconPhiToDegree();

				bool terminate = false;
				double degreeSeed, finalDegree;
				degreeSeed = finalDegree = degree;

				//reset result
				mResult = new EdgeResult();
				//旋轉角度, Search Point
				while (!HasResult)
				{
					var region = getRectangle2(row, column, degreeSeed, length1, length2);
					var regionImage = getRegionImage(region);
					//Reset Handle and mResult					
					mHandle = new HMeasure(mMeasROI[0].D, mMeasROI[1].D,
											   degreeSeed.ToHalconPhi(), mMeasROI[3].D, mMeasROI[4].D,
											   mMeasAssist.mWidth, mMeasAssist.mHeight,
											   mMeasAssist.mInterpolation);
					measurePos(regionImage);
					finalDegree = degreeSeed;
					degreeSeed = getNextSeed(degreeSeed, phi.D, ref terminate);
					if (terminate) break;
				}
				mResultWorld = new EdgeResult(mResult);

				//Update Model
				var updateableROI = mRoi as ViewROI.Interface.IROIModelUpdateable;
				if (updateableROI != null)
				{
					mMeasROI[2].D = finalDegree.ToHalconPhi();
					var updateROIModel = new ROIViewModel()
					{
						CenterRow = row.D,
						CenterCol = column.D,
						Phi = finalDegree.ToHalconPhi(),
						Length = length1.D,
						Width = length2.D,
					};
					updateableROI.UpdateROIModel(updateROIModel);
				}

			}
			catch (HOperatorException e)
			{
				mEdgeXLD.Dispose();
				mMeasAssist.exceptionText = e.Message;
				mResultWorld = new EdgeResult();
				return;
			}
			UpdateXLD();
		}
		/// <summary>
		/// 找下一個搜尋的角度
		/// </summary>
		/// <param name="degreeSeed"></param>
		/// <param name="phi">Halcon 徑度</param>
		/// <param name="isTerminate"></param>
		/// <returns></returns>
		private double getNextSeed(double degreeSeed, double phi, ref bool isTerminate)
		{
			double upperBound = phi.HalconPhiToDegree() + 180;
			if (degreeSeed <= upperBound)
			{
				degreeSeed++;
			}
			else if (degreeSeed > upperBound)
			{
				degreeSeed = upperBound;
				isTerminate = true;
			}
			return degreeSeed;
		}
		private HRegion getRectangle2(HTuple row, HTuple column, double degree, HTuple length1, HTuple length2)
		{
			var theta = new HTuple(degree.ToHalconPhi());
			HRegion region = new HRegion();
			region.GenRectangle2(row, column, theta, length1, length2);
			return region;
		}

		private HImage getRegionImage(HRegion region)
		{
			HImage regionImage = null;
			if (mMeasAssist.ApplyCalibration && mMeasAssist.IsCalibrationValid)
			{
				regionImage = mMeasAssist.mImage.ChangeRadialDistortionImage(region, mMeasAssist.CameraIn, _cameraOut);
			}
			else
			{
				HObject imageReduced;
				HOperatorSet.GenEmptyObj(out imageReduced);
				HOperatorSet.ReduceDomain(mMeasAssist.mImage, region, out imageReduced);
				regionImage = new HImage(imageReduced);
			}
			return regionImage;
		}

		/// <summary>
		/// 顯示量測外觀
		/// </summary>
		public override void UpdateXLD()
		{
			if (mHandle == null && ((int)mHandle.Handle < 0))
				return;

			mMeasAssist.exceptionText = "";
			var width = mMeasROI[4];
			mEdgeXLD.Dispose();
			mEdgeXLD.GenEmptyObj();

			try
			{
				var phi = mMeasROI[2].D;
				for (int i = 0; i < mResult.rowEdge.Length; i++)
				{
					//mEdgeXLD = mEdgeXLD.ConcatObj(DetermineEdgeLine(mResult.rowEdge[i].D, mResult.colEdge[i].D, phi, width));
					mEdgeXLD = mEdgeXLD.ConcatObj(DetermineCrossPoint(mResult.rowEdge[i].D, mResult.colEdge[i].D, _crossPointSize, 0.5));
				}
			}
			catch (HOperatorException e)
			{
				mMeasAssist.exceptionText = e.Message;
			}
		}
		public override MeasureResult getMeasureResultData()
		{
			return mResultWorld;
		}
		public override void ClearResultData()
		{
			mResultWorld = new EdgeResult();
		}
		public override MeasureViewModel GetViewModel()
		{
			//init value
			MeasureViewModel viewMoel = new MeasureViewModel()
			{
				Row1 = new HTuple(),
				Col1 = new HTuple(),
				Row2 = new HTuple(),
				Col2 = new HTuple(),
				Distance = new HTuple(),
			};
			if (mResult.rowEdge != null && mResult.rowEdge.TupleLength() > 0)
			{
				HXLDCont edgeXLD;
				HTuple rows, cols;
				edgeXLD = DetermineCrossPoint(mResult.rowEdge.D, mResult.colEdge.D, _crossPointSize, 0.5);
				edgeXLD.GetContourXld(out rows, out cols);
				viewMoel = new MeasureViewModel()
				{
					Row1 = rows[0],
					Col1 = cols[0],
					GeoType = MeasureType.Point,
				};
			}
			return viewMoel;
		}
		#endregion===============================================
		private void measurePos(HImage imageReduced)
		{
			mHandle.MeasurePos(imageReduced,
										  mMeasAssist.mSigma, mMeasAssist.mThresh,
										  mMeasAssist.mTransition, mMeasAssist.mPosition,
										  out mResult.rowEdge, out mResult.colEdge,
										  out mResult.amplitude, out mResult.distance);
		}
	}
}
