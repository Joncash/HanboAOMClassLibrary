using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI;

namespace MeasureModule
{
	/// <summary>
	/// 量測兩線段的夾角
	/// </summary>
	public class MeasurementTwoLineAngle : Measurement
	{
		private AngleResult _Result;
		private AngleResult _ResultWorld;

		private IMeasureGeoModel _geoModelOne;
		private IMeasureGeoModel _geoModelTwo;


		/// <summary>
		/// 兩個 MeasureType 為 Line 的 ROI
		/// </summary>
		/// <param name="roiOne"></param>
		/// <param name="roiTwo"></param>
		/// <param name="mAssist"></param>
		public MeasurementTwoLineAngle(ROI roiOne, ROI roiTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			var lineF = new MeasurementEdge(roiOne, mAssist);
			var lineS = new MeasurementEdge(roiTwo, mAssist);
			initialize(lineF.GetViewModel(), lineS.GetViewModel(), mAssist);
		}

		/// <summary>
		/// 兩個線段的幾何模型
		/// </summary>
		/// <param name="geoModelOne"></param>
		/// <param name="geoModelTwo"></param>
		/// <param name="mAssist"></param>
		public MeasurementTwoLineAngle(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
			: base(null, mAssist)
		{
			initialize(geoModelOne, geoModelTwo, mAssist);
		}

		private void initialize(IMeasureGeoModel geoModelOne, IMeasureGeoModel geoModelTwo, MeasureAssistant mAssist)
		{
			_geoModelOne = geoModelOne;
			_geoModelTwo = geoModelTwo;
			ROIMeasureType = MeasureType.Angle;
			//_Result = new AngleResult();
			_ResultWorld = new AngleResult();
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
				_Result = DistanceHelper.AngleLineToLine(_geoModelOne, _geoModelTwo);
				if (mMeasAssist.mIsCalibValid && mMeasAssist.mTransWorldCoord)
				{
					Rectify(_Result.Row, _Result.Col, out _ResultWorld.Row, out _ResultWorld.Col);
				}
				else
				{
					_ResultWorld = new AngleResult(_Result);
				}
			}
			catch (HOperatorException ex)
			{
				mMeasAssist.exceptionText = ex.Message;
				_ResultWorld = new AngleResult();
				_Result = new AngleResult();
			}
			UpdateXLD();
		}

		/// <summary>
		/// 顯示 Measure 的幾何元素外觀
		/// </summary>
		public override void UpdateXLD()
		{
			//不畫結果
		}

		public override MeasureResult getMeasureResultData()
		{
			return _ResultWorld;
		}
		public override void ClearResultData()
		{
			_ResultWorld = new AngleResult();
		}

		/// <summary>
		/// 量測結果的 ViewModel
		/// </summary>
		/// <returns></returns>
		public override MeasureViewModel GetViewModel()
		{
			return new MeasureViewModel()
			{
				Row1 = _Result.Row,
				Col1 = _Result.Col,
				StartPhi = _Result.StartPhi,
				EndPhi = _Result.EndPhi,
				Angle = _Result.Angle,
				Distance = _Result.Distance
			};
		}
	}
}
