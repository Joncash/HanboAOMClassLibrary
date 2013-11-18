using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace MeasureModule
{
	public class MeasurementFitLine : Measurement
	{
		private FitLineResult mResult;
		private FitLineResult mResultWorld;

		#region Edge sub pixel 演算法參數
		/*
		 Filter 是演算法的名稱，
		 alpha 是 演算法的參數值
		 "small values result in strong smoothing, and thus less detail"
		 
		 Point 若計算出來的值 > _high, 則被歸類為 "Edge 的點集合"
		 若 < _low, 則被排除，
		 其他介於 high 與 low 之間的點，則會與 "Edge 的點集合" 中的點作 connected 計算，
		 若有 connected, 則被歸類進 "Edge 的點集合" 中
		 
		 */
		private HTuple _filter = "lanser2";
		private HTuple _alpha = 0.5;
		private HTuple _low = 40;
		private HTuple _high = 90;

		#endregion

		#region SegmentContour 演算法參數
		/*
		 
		 
		 
		 */
		private HTuple _mode = "lines";
		private HTuple _smoothCont = 6;
		private HTuple _maxLineDist1 = 4.0;
		private HTuple _maxLineDist2 = 4.0;

		#endregion

		#region FitContour 演算法參數
		/*
		 */

		private HTuple _algorithm = "tukey";
		private HTuple _maxNumPoints = -1;
		private HTuple _clippingEndPoints = 0;
		private HTuple _iterations = 5;
		private HTuple _clippingFactor = 2;
		#endregion



		public MeasurementFitLine(ROI roi, MeasureAssistant mAssist)
			: base(roi, mAssist)
		{
			mResult = new FitLineResult();
			UpdateMeasure();
		}
		/// <summary>
		/// 更新量測結果.
		/// 量測演算法放這裡
		/// </summary>
		public override void UpdateResults()
		{
			if (mMeasAssist.mImage == null) return;
			//init result
			mResult = new FitLineResult()
			{
				Col1 = new HTuple(),
				Row1 = new HTuple(),
				Col2 = new HTuple(),
				Row2 = new HTuple(),
			};

			var image = mMeasAssist.mImage;

			HObject rectangle, imageReduced, edges, contoursSplit;
			HOperatorSet.GenEmptyObj(out rectangle);
			HOperatorSet.GenEmptyObj(out imageReduced);
			HOperatorSet.GenEmptyObj(out edges);
			HOperatorSet.GenEmptyObj(out contoursSplit);

			//建 ROI
			var roiModel = mRoi.getModelData();
			var row = roiModel[0];
			var column = roiModel[1];
			var phi = roiModel[2];
			var length1 = roiModel[3];
			var length2 = roiModel[4];
			//{ midR, midC, phi, length1, length2 });

			HOperatorSet.GenRectangle2(out rectangle, row, column, phi, length1, length2);
			HOperatorSet.ReduceDomain(image, rectangle, out imageReduced);

			//Edgesubpix
			HOperatorSet.EdgesSubPix(imageReduced, out edges, _filter, _alpha, _low, _high);
			HOperatorSet.SegmentContoursXld(edges, out contoursSplit, _mode, _smoothCont, _maxLineDist1, _maxLineDist2);

			//fitLine
			HTuple number, rowBegin, colBegin, rowEnd, colEnd, nr, nc, dist;
			HObject objectSelected;
			HOperatorSet.GenEmptyObj(out objectSelected);

			HTuple distance, preDistance = 0.0;

			HOperatorSet.CountObj(contoursSplit, out number);
			for (HTuple hv_i = 1; hv_i.Continue(number, 1); hv_i = hv_i.TupleAdd(1))
			{
				HOperatorSet.SelectObj(contoursSplit, out objectSelected, hv_i);
				HOperatorSet.FitLineContourXld(objectSelected, _algorithm, _maxNumPoints, _clippingEndPoints, _iterations, _clippingFactor,
					out rowBegin, out colBegin, out rowEnd, out colEnd, out nr, out nc, out dist);

				HOperatorSet.DistancePp(rowBegin, colBegin, rowEnd, colEnd, out distance);
				if (distance > preDistance)
				{
					preDistance = new HTuple(distance);
					//Answer
					mResult = new FitLineResult()
					{
						Row1 = new HTuple(rowBegin),
						Col1 = new HTuple(colBegin),
						Row2 = new HTuple(rowEnd),
						Col2 = new HTuple(colEnd),
					};
					mResultWorld = new FitLineResult(mResult);
				}
			}
			UpdateXLD();
		}

		/// <summary>
		/// 顯示 Measure 的幾何元素外觀
		/// </summary>
		public override void UpdateXLD()
		{
			//clear display
			mEdgeXLD.Dispose();
			mEdgeXLD.GenEmptyObj();

			if (mResult.Row1 == null || mResult.Row1.TupleLength() == 0) return;
			HXLDCont edge = new HXLDCont();
			var rows = new HTuple(new double[] { mResult.Row1, mResult.Row2 });
			var cols = new HTuple(new double[] { mResult.Col1, mResult.Col2 });
			edge.GenContourPolygonXld(rows, cols);
			mEdgeXLD = mEdgeXLD.ConcatObj(edge);
		}

		public override MeasureResult getMeasureResultData()
		{
			return mResultWorld;
		}

		public override void ClearResultData()
		{
			mResultWorld = new FitLineResult();
		}

		public override MeasureViewModel GetViewModel()
		{
			return new MeasureViewModel()
			{
				Row1 = mResult.Row1,
				Col1 = mResult.Col1,
				Row2 = mResult.Row2,
				Col2 = mResult.Col2,
			};
		}

	}
}
