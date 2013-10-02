using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace MeasureModule
{
	/// <summary>
	/// 計算兩點之間的 X 方向距離
	/// </summary>
	public class MeasurementDistanceX : Measurement
	{
		private IMeasureGeoModel _geoModelOne;
		private IMeasureGeoModel _geoModelTwo;

		private LineResult _Result;
		private LineResult _ResultWorld;

		public MeasurementDistanceX(ROI roiOne, ROI roiTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			var lineF = new MeasurementEdge(roiOne, mAssist);
			var lineS = new MeasurementEdge(roiTwo, mAssist);
			initialize(lineF.GetViewModel(), lineS.GetViewModel(), mAssist);
		}

		public MeasurementDistanceX(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			initialize(geoModelOne, geoModelTwo, mAssist);
		}

		private void initialize(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
		{
			_geoModelOne = geoModelOne;
			_geoModelTwo = geoModelTwo;
			ROIMeasureType = MeasureType.DistanceX;
			_ResultWorld = new LineResult();
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
				_Result = DistanceHelper.DistanceX(_geoModelOne, _geoModelTwo);
				if (mMeasAssist.mIsCalibValid && mMeasAssist.mTransWorldCoord)
				{
					Rectify(_Result.Row1, _Result.Col1, out _ResultWorld.Row1, out _ResultWorld.Col1);
				}
				else
				{
					_ResultWorld = new LineResult(_Result);
				}
			}
			catch (HOperatorException ex)
			{
				mMeasAssist.exceptionText = ex.Message;
				_ResultWorld = new LineResult();
				_Result = new LineResult();
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

			if (_Result.Row1 == null) return;
			if (_Result.Row1.Length > 0)
			{
				var rows = new double[] { _Result.Row1, _Result.Row2 };
				var cols = new double[] { _Result.Col1, _Result.Col2 };
				var pointXLD = new HXLDCont();
				pointXLD.GenContourPolygonXld(rows, cols);

				//output
				mEdgeXLD = mEdgeXLD.ConcatObj(pointXLD);
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
			_ResultWorld = new LineResult();
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
				Col2 = _Result.Col2,
				Row2 = _Result.Row2,
				Distance = _Result.Distance,
			};
		}
	}
}
