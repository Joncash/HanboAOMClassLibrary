using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace MeasureModule
{
	/// <summary>
	/// 量測兩線段的交點
	/// </summary>
	public class MeasurementTwoLineCrossPoint : Measurement
	{
		private IMeasureGeoModel _geoModelOne;
		private IMeasureGeoModel _geoModelTwo;

		private PointResult _Result;
		private PointResult _ResultWorld;
		public MeasurementTwoLineCrossPoint(ROI roiOne, ROI roiTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			var lineF = new MeasurementEdge(roiOne, mAssist);
			var lineS = new MeasurementEdge(roiTwo, mAssist);
			initialize(lineF.GetViewModel(), lineS.GetViewModel(), mAssist);
		}
		public MeasurementTwoLineCrossPoint(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			initialize(geoModelOne, geoModelTwo, mAssist);
		}

		private void initialize(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
		{
			_geoModelOne = geoModelOne;
			_geoModelTwo = geoModelTwo;
			ROIMeasureType = MeasureType.Point;
			_ResultWorld = new PointResult();
			UpdateResults();
		}

		/// <summary>
		/// 更新量測結果.
		/// 量測演算法放這裡
		/// </summary>
		public override void UpdateResults()
		{
			try
			{
				_Result = DistanceHelper.IntersetionLine(_geoModelOne, _geoModelTwo);
				if (mMeasAssist.mIsCalibValid && mMeasAssist.mTransWorldCoord)
				{
					Rectify(_Result.Row1, _Result.Col1, out _ResultWorld.Row1, out _ResultWorld.Col1);
				}
				else
				{
					_ResultWorld = new PointResult(_Result);
				}
			}
			catch (HOperatorException ex)
			{
				mMeasAssist.exceptionText = ex.Message;
				_ResultWorld = new PointResult();
				_Result = new PointResult();
			}
			UpdateXLD();
		}

		/// <summary>
		/// 顯示 Measure 的幾何元素外觀
		/// </summary>
		public override void UpdateXLD()
		{
			//clear
			mEdgeXLD.Dispose();
			mEdgeXLD.GenEmptyObj();

			var size = 15;
			var angle = 0.735398;
			if (_Result.Row1 == null) return;
			if (_Result.Row1.Length > 0)
			{
				for (int i = 0; i < _Result.Row1.Length; i++)
				{
					var pointXLD = new HXLDCont();
					pointXLD.GenCrossContourXld(_Result.Row1[i].D, _Result.Col1[i].D, size, angle);

					//output
					mEdgeXLD = mEdgeXLD.ConcatObj(pointXLD);
				}
			}
		}

		/// <summary>
		/// override method
		/// </summary>
		/// <returns></returns>
		public override MeasureResult getMeasureResultData()
		{
			return _ResultWorld;
		}
		public override void ClearResultData()
		{
			_ResultWorld = new PointResult();
		}

		/// <summary>
		/// 量測結果的 ViewModel
		/// </summary>
		/// <returns></returns>
		public override MeasureViewModel GetViewModel()
		{
			return new MeasureViewModel()
			{
				Row1 = _Result.Row1,
				Col1 = _Result.Col1,
			};
		}

	}
}
