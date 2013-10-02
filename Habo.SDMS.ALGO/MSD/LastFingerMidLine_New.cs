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
	public class SDMS_LastFingerMidLine_New
	{
		public MeasureViewModel GetMidLine()
		{
			#region 輸出結果
			LineResult mResult = null;
			MeasureViewModel midLineModel = null;
			#endregion

			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol;
			HOperatorSet.SetSystem("border_shape_models", "false");

			//STD 中心點
			hv_STD_Row = 839.5;
			hv_STD_Col = 1046.5;

			//目前圖形 中心點
			hv_Img_Row = hv_AllModelRow.Clone();
			hv_Img_Col = hv_AllModelColumn.Clone();

			//目前圖形 Rotate Angle
			hv_Img_Rotate_Angle = hv_AllModelAngle.Clone();

			//目前圖形偏移量
			hv_OffsetRow = hv_Img_Row - hv_STD_Row;
			hv_OffsetCol = hv_Img_Col - hv_STD_Col;

			//第一個 線段 ROI
			HTuple f_ROI_Row = 519.896484375;
			HTuple f_ROI_Col = 1319.67320261438;
			HTuple f_angle = 0;
			HTuple f_ROI_Length1 = 27.4509803921569;
			HTuple f_ROI_Length2 = 99.845703125;

			HTuple f_angle_offset = f_angle - hv_Img_Rotate_Angle;
			HTuple f_ROI_Cur_Row, f_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol, f_ROI_Row, f_ROI_Col, out f_ROI_Cur_Row, out f_ROI_Cur_Col);

			//第二個 線段 ROI
			HTuple s_ROI_Row = 521.728515625;
			HTuple s_ROI_Col = 1350.78431372549;
			HTuple s_angle = 3.14159265358979;
			HTuple s_ROI_Length1 = 19.2156862745099;
			HTuple s_ROI_Length2 = 98.013671875;

			HTuple s_angle_offset = s_angle - hv_Img_Rotate_Angle;
			HTuple s_ROI_Cur_Row, s_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol
					, s_ROI_Row, s_ROI_Col, out s_ROI_Cur_Row, out s_ROI_Cur_Col);

			#region Measure
			var cROIController = new ROIController();
			var cAssistant = new MeasureAssistant(cROIController);

			var hImage = ho_Image as HImage;
			cAssistant.setImage(hImage);

			/*參數值*/
			cAssistant.mThresh = 40.0;
			cAssistant.mSigma = 1.0;
			cAssistant.mRoiWidth = 10;
			cAssistant.mInterpolation = "bilinear";
			cAssistant.mSelPair = false;
			cAssistant.mTransition = "all";
			cAssistant.mPosition = "last";
			cAssistant.mDispEdgeLength = 30;
			cAssistant.mDispROIWidth = true;
			cAssistant.setUnit("cm");

			cAssistant.mInitThresh = 40.0;
			cAssistant.mInitSigma = 1.0;
			cAssistant.mInitRoiWidth = 10;

			var roiF = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiF.MakeROI(416, 998, 0, 26.5, 71.2);
			roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_angle_offset, f_ROI_Length1, f_ROI_Length2);

			var roiS = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiS.MakeROI(400, 1041, 0, 13.3, 75.7);
			roiS.MakeROI(s_ROI_Cur_Row, s_ROI_Cur_Col, s_angle_offset, s_ROI_Length1, s_ROI_Length2);

			var lineF = new MeasurementEdge(roiF, cAssistant);
			var lineS = new MeasurementEdge(roiS, cAssistant);
			mResult = DistanceHelper.CalculateSymmetryLine(lineF.GetViewModel(), lineS.GetViewModel());

			midLineModel = new MeasureViewModel() { 
				Row1 = mResult.Row1,
				Row2 = mResult.Row2,
				Col1 = mResult.Col1,
				Col2 = mResult.Col2,
				Distance = mResult.Distance,
			};

			//var firstModel = lineF.GetViewModel();
			//var secondModel = lineS.GetViewModel();
			//if (firstModel != null && secondModel != null && firstModel.Distance != null && secondModel.Distance != null
			//	&& firstModel.Distance.TupleLength() > 0 && secondModel.Distance.TupleLength() > 0)
			//{
			//	//作線段
			//	var centerRow = (f_ROI_Cur_Row + s_ROI_Cur_Row) / 2.0;
			//	var centerCol = (f_ROI_Cur_Col + s_ROI_Cur_Col) / 2.0;

			//	midLineModel = DistanceHelper.MakeLine(centerRow, centerCol, hv_AllModelAngle, mResult.Distance / 2.0);
			//}

			return midLineModel;
			#endregion


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
