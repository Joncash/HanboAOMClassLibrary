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
	public class SDMS_B1Plus : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			LineResult mResult = null;
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
			//HTuple f_ROI_Row = 341.21875;
			//HTuple f_ROI_Col = 803.078431372549;
			//HTuple f_angle = 1.5707963267949;
			//HTuple f_ROI_Length1 = 77.5390625;
			//HTuple f_ROI_Length2 = 148.705882352941;
			HTuple f_ROI_Row = 357.08984375;
			HTuple f_ROI_Col = 816.555555555556;
			HTuple f_angle = 1.5707963267949;
			HTuple f_ROI_Length1 = 69.8359375;
			HTuple f_ROI_Length2 = 122.277777777778;


			HTuple f_angleOffset = f_angle - hv_Img_Rotate_Angle;
			HTuple f_ROI_Cur_Row, f_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol, f_ROI_Row, f_ROI_Col, out f_ROI_Cur_Row, out f_ROI_Cur_Col);

			//兩線段交點
			HTuple p1_ROI_Row = 715.40234375;
			HTuple p1_ROI_Col = 744.222222222222;
			HTuple p1_angle = 0.764250656215704;
			HTuple p1_ROI_Length1 = 68.0072446324003;
			HTuple p1_ROI_Length2 = 105.756749157524;

			HTuple p1_angleOffset = p1_angle - hv_Img_Rotate_Angle;
			HTuple p1_ROI_Cur_Row, p1_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol
					, p1_ROI_Row, p1_ROI_Col, out p1_ROI_Cur_Row, out p1_ROI_Cur_Col);


			HTuple p2_ROI_Row = 794.64453125;
			HTuple p2_ROI_Col = 702.888888888889;
			HTuple p2_angle = 0;
			HTuple p2_ROI_Length1 = 100;
			HTuple p2_ROI_Length2 = 50;
			HTuple p2_angleOffset = p2_angle - hv_Img_Rotate_Angle;
			HTuple p2_ROI_Cur_Row, p2_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol
					, p2_ROI_Row, p2_ROI_Col, out p2_ROI_Cur_Row, out p2_ROI_Cur_Col);





			#region Measure
			var cROIController = new ROIController();
			var cAssistant = new MeasureAssistant(cROIController);

			var hImage = ho_Image as HImage;
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

			var p1Line = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiF.MakeROI(416, 998, 0, 26.5, 71.2);
			p1Line.MakeROI(p1_ROI_Cur_Row, p1_ROI_Cur_Col, p1_angleOffset, p1_ROI_Length1, p1_ROI_Length2);

			var p2Line = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiS.MakeROI(400, 1041, 0, 13.3, 75.7);
			p2Line.MakeROI(p2_ROI_Cur_Row, p2_ROI_Cur_Col, p2_angleOffset, p2_ROI_Length1, p2_ROI_Length2);

			var p1F = new MeasurementEdge(p1Line, cAssistant);
			var p2S = new MeasurementEdge(p2Line, cAssistant);
			var angleResult = DistanceHelper.AngleLineToLine(p1F, p2S);

			var roiF = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_angleOffset, f_ROI_Length1, f_ROI_Length2);
			var lineF = new MeasurementEdge(roiF, cAssistant);

			if (angleResult != null && lineF != null)
			{
				var pointViewModel = new MeasureViewModel()
				{
					Row1 = angleResult.Row,
					Col1 = angleResult.Col,
				};
				var lineViewModel = lineF.GetViewModel();
				var distance = DistanceHelper.PointToLine(lineViewModel, pointViewModel);
				mResult = new LineResult()
				{
					Row1 = lineViewModel.Row2,
					Col1 = lineViewModel.Col2,
					Row2 = pointViewModel.Row1,
					Col2 = pointViewModel.Col1,
					Distance = distance,
				};
			}

			#endregion

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
