using System;
using ViewROI;
using HalconDotNet;

namespace MeasureModule
{

	/// <summary>
	/// The class MeasurementEdge describes single-edge measurement
	/// and inherits from the base class Measurement. Virtual methods 
	/// defined in the base class are customized here to apply
	/// HALCON operators for single-edge extraction.
	/// </summary>
	public class MeasurementEdge : Measurement
	{
		/// <summary>
		/// Result container for the edge information returned
		/// by the HALCON measure operator.
		/// </summary>
		private EdgeResult mResult;
		/// <summary>
		/// Result container for the edge information converted
		/// into world coordinates. If calibration data is not available,
		/// the variable contains the same information as mResult.
		/// </summary>
		private EdgeResult mResultWorld;

		private double _crossPointSize = 12.0;
		/// <summary>
		/// Creates a measurement object for the provided ROI instance.
		/// </summary>
		/// <param name="roi">ROI instance</param>
		/// <param name="mAssist">Reference to controller class</param>
		public MeasurementEdge(ROI roi, MeasureAssistant mAssist)
			: base(roi, mAssist)
		{
			mResult = new EdgeResult();
			mResultWorld = new EdgeResult();
			UpdateMeasure();
		}

		/// <summary>
		/// Triggers an update of the measure results because of  
		/// changes in the parameter setup or a recreation of the measure 
		/// object caused by an update in the ROI model.
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
				var row = roiModel[0];
				var column = roiModel[1];
				var phi = roiModel[2];
				var length1 = roiModel[3];
				var length2 = roiModel[4];
				HRegion region = new HRegion();
				region.GenRectangle2(row.D, column.D, phi.D, length1.D, length2.D);

				if (mMeasAssist.ApplyCalibration && mMeasAssist.IsCalibrationValid)
				{
					HTuple cameraOut = HMisc.ChangeRadialDistortionCamPar("adaptive", mMeasAssist.CameraIn, 0.0);
					var rectifyImage = mMeasAssist.mImage.ChangeRadialDistortionImage(region, mMeasAssist.CameraIn, cameraOut);
					measurePos(rectifyImage);
				}
				else
				{
					HOperatorSet.ReduceDomain(mMeasAssist.mImage, region, out imageReduced);
					measurePos(new HImage(imageReduced));
				}
				mResultWorld = new EdgeResult(mResult);
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

		private void measurePos(HImage imageReduced)
		{
			mHandle.MeasurePos(imageReduced,
										  mMeasAssist.mSigma, mMeasAssist.mThresh,
										  mMeasAssist.mTransition, mMeasAssist.mPosition,
										  out mResult.rowEdge, out mResult.colEdge,
										  out mResult.amplitude, out mResult.distance);
		}

		/// <summary>Updates display object for measured edge results</summary>
		public override void UpdateXLD()
		{
			double width, phi, cRow, cCol, radius;

			if (mHandle == null && ((int)mHandle.Handle < 0))
				return;

			mMeasAssist.exceptionText = "";
			width = mMeasROI[4]; //mMeasAssist.mDispROIWidth ? mMeasAssist.mRoiWidth : mMeasAssist.mDispEdgeLength;
			mEdgeXLD.Dispose();
			mEdgeXLD.GenEmptyObj();

			try
			{
				if (mROIType == ROI.ROI_TYPE_LINE)
				{
					phi = mMeasROI[2].D;

					for (int i = 0; i < mResult.rowEdge.Length; i++)
						mEdgeXLD = mEdgeXLD.ConcatObj(DetermineEdgeLine(mResult.rowEdge[i].D, mResult.colEdge[i].D, phi, width));

				}
				else if (mROIType == ROI.ROI_TYPE_POINT)
				{
					phi = mMeasROI[2].D;
					for (int i = 0; i < mResult.rowEdge.Length; i++)
					{
						mEdgeXLD = mEdgeXLD.ConcatObj(DetermineEdgeLine(mResult.rowEdge[i].D, mResult.colEdge[i].D, phi, width));
						mEdgeXLD = mEdgeXLD.ConcatObj(DetermineCrossPoint(mResult.rowEdge[i].D, mResult.colEdge[i].D, _crossPointSize, 0.5));
					}
				}
				else if (mROIType == ROI.ROI_TYPE_CIRCLEARC)
				{
					cRow = mROICoord[0].D;
					cCol = mROICoord[1].D;
					radius = mROICoord[2].D;

					for (int i = 0; i < mResult.rowEdge.Length; i++)
						mEdgeXLD = mEdgeXLD.ConcatObj(DetermineEdgeCircularArc(mResult.rowEdge[i].D, mResult.colEdge[i].D, cRow, cCol, radius, width));
				}
			}
			catch (HOperatorException e)
			{
				mMeasAssist.exceptionText = e.Message;
			}
		}

		/// <summary>Returns measurement result.</summary>
		public override MeasureResult getMeasureResultData()
		{
			return mResultWorld;
		}

		/// <summary>Clears measurement result.</summary>
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
				if (mROIType == ROI.ROI_TYPE_POINT)
				{
					edgeXLD = DetermineCrossPoint(mResult.rowEdge.D, mResult.colEdge.D, _crossPointSize, 0.5);
					edgeXLD.GetContourXld(out rows, out cols);
					viewMoel = new MeasureViewModel()
					{
						Row1 = rows[0],
						Col1 = cols[0],
					};
				}
				else
				{
					var phi = mMeasROI[2].D;
					var width = mMeasROI[4].D;
					edgeXLD = DetermineEdgeLine(mResult.rowEdge.D, mResult.colEdge.D, phi, width);
					edgeXLD.GetContourXld(out rows, out cols);
					viewMoel = new MeasureViewModel()
					{
						Row1 = rows[0],
						Col1 = cols[0],
						Row2 = rows.TupleLength() > 1 ? rows[1] : null,
						Col2 = cols.TupleLength() > 1 ? cols[1] : null,
						Distance = mResult.distance,
					};
				}
			}
			return viewMoel;
		}


		/// <summary>
		/// 取得中點
		/// </summary>
		/// <param name="midX"></param>
		/// <param name="midY"></param>
		public void GetMidPoint(out double midX, out double midY)
		{
			//有值
			midX = midY = -1;
			if (mResult.rowEdge != null && mResult.rowEdge.TupleLength() > 0)
			{
				HXLDCont edgeXLD;
				HTuple rows, cols;
				edgeXLD = DetermineCrossPoint(mResult.rowEdge.D, mResult.colEdge.D, _crossPointSize, 0.5);
				edgeXLD.GetContourXld(out rows, out cols);
				midX = cols[0];
				midY = rows[0];
			}
		}

	}//end of class
}//end of namespace
