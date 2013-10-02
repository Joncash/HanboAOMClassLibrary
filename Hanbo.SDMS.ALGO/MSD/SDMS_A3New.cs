using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace Habo.SDMS.ALGO.MSD
{
	public class SDMS_A3New : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			DistanceResult mResult = null;
			#endregion


			var fMidLine = new SDMS_FirstFingerMidLine_New();
			var image = new HImage(ho_Image);
			fMidLine.Initialize(image, hv_AllModelRow, hv_AllModelColumn, hv_AllModelAngle);
			var fMidLineModel = fMidLine.GetMidLine() as IMeasureGeoModel;

			var sMidLine = new SDMS_LastFingerMidLine_New();
			sMidLine.Initialize(image, hv_AllModelRow, hv_AllModelColumn, hv_AllModelAngle);
			var sMidLineModel = sMidLine.GetMidLine() as IMeasureGeoModel;

			var distance = DistanceHelper.LineToLine(fMidLineModel, sMidLineModel);

			mResult = new DistanceResult()
			{
				FirstRowBegin = fMidLineModel.Row1,
				FirstColBegin = fMidLineModel.Col1,
				FirstRowEnd = fMidLineModel.Row2,
				FirstColEnd = fMidLineModel.Col2,
				SecondRowBegin = sMidLineModel.Row1,
				SecondColBegin = sMidLineModel.Col1,
				SecondRowEnd = sMidLineModel.Row2,
				SecondColEnd = sMidLineModel.Col2,
				Angle = hv_AllModelAngle,
				Direction = LineDirection.Horizontal,
				Distance = distance,
			};
			return mResult;
		}

		#region Initialize
		private HObject ho_Image;
		private HTuple hv_AllModelRow;
		private HTuple hv_AllModelColumn;
		private HTuple hv_AllModelAngle;

		public void Initialize(HImage image, HTuple modelRow, HTuple modelColumn, HTuple modelAngle)
		{
			ho_Image = image;
			hv_AllModelRow = new HTuple(modelRow);
			hv_AllModelColumn = new HTuple(modelColumn);
			hv_AllModelAngle = new HTuple(modelAngle);
		}
		#endregion
	}
}
