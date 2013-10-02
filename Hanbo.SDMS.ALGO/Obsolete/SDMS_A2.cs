using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_A2 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			LineResult mResult = null;
			#endregion
			HObject ho_A2_Region = null;
			HObject ho_A2_Reduced = null, ho_A2Region = null, ho_A2_RegionBorder = null;
			HObject ho_A2_RegionDilation = null, ho_A2_Edges = null, ho_A2_Rectangles = null;
			HObject ho_A2Cross = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			//HTuple hv_AllModelId, hv_AllModelRow, hv_AllModelColumn;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_CL_X, hv_CL_Y, hv_STD_CL_1_Row;
			HTuple hv_STD_CL_1_Col, hv_STD_CL_1_V_Row, hv_STD_CL_1_V_Col;
			HTuple hv_CL_1_X, hv_CL_1_Y, hv_CL_1_Pos_Row, hv_CL_1_Pos_Col;
			HTuple hv_A2_Center_X, hv_A2_Center_Y, hv_STD_A2_1_1_Row;
			HTuple hv_STD_A2_1_1_Col, hv_STD_A2_1_1_V_Row, hv_STD_A2_1_1_V_Col;
			HTuple hv_A2_1_1_X, hv_A2_1_1_Y, hv_A2_1_1_Pos_Row, hv_A2_1_1_Pos_Col;
			HTuple hv_A2_ROI_W = new HTuple(), hv_A2_ROI_H = new HTuple();
			HTuple hv_Rec_W = new HTuple(), hv_Rec_H = new HTuple(), hv_A2_Alpha = new HTuple();
			HTuple hv_a2low = new HTuple(), hv_a2high = new HTuple(), hv_a2Limit = new HTuple();
			HTuple hv_A2_RecNumber = new HTuple(), hv_A2Row = new HTuple();
			HTuple hv_A2Column = new HTuple(), hv_A2Phi = new HTuple();
			HTuple hv_A2Length1 = new HTuple(), hv_A2Length2 = new HTuple();
			HTuple hv_A2PointOrder = new HTuple(), hv_A2Number = new HTuple();
			HTuple hv_A2_Dist, hv_X1 = new HTuple(), hv_X2 = new HTuple();
			HTuple hv_Y1 = new HTuple(), hv_Y2 = new HTuple(), hv_STD_Mark_A2_C_Row = new HTuple();
			HTuple hv_STD_Mark_A2_C_Col = new HTuple(), hv_STD_Mark_A2_Start_Row = new HTuple();
			HTuple hv_STD_Mark_A2_Start_Col = new HTuple(), hv_STD_Mark_A2_End_Row = new HTuple();
			HTuple hv_STD_Mark_A2_End_Col = new HTuple(), hv_STD_Mark_A2_C_V_Row = new HTuple();
			HTuple hv_STD_Mark_A2_C_V_Col = new HTuple(), hv_Mark_A2_C_X = new HTuple();
			HTuple hv_Mark_A2_C_Y = new HTuple(), hv_Mark_A2_C_Pos_Row = new HTuple();
			HTuple hv_Mark_A2_C_Pos_Col = new HTuple(), hv_STD_Mark_A2_Start_V_Row = new HTuple();
			HTuple hv_STD_Mark_A2_Start_V_Col = new HTuple(), hv_Mark_A2_Start_X = new HTuple();
			HTuple hv_Mark_A2_Start_Y = new HTuple(), hv_Mark_A2_Start_Pos_Row = new HTuple();
			HTuple hv_Mark_A2_Start_Pos_Col = new HTuple(), hv_STD_Mark_A2_End_V_Row = new HTuple();
			HTuple hv_STD_Mark_A2_End_V_Col = new HTuple(), hv_Mark_A2_End_X = new HTuple();
			HTuple hv_Mark_A2_End_Y = new HTuple(), hv_Mark_A2_End_Pos_Row = new HTuple();
			HTuple hv_Mark_A2_End_Pos_Col = new HTuple();

			// Initialize local and output iconic variables 
			//HOperatorSet.GenEmptyObj(out ho_Image);
			//HOperatorSet.GenEmptyObj(out ho_AllModelContours);
			HOperatorSet.GenEmptyObj(out ho_A2_Region);
			HOperatorSet.GenEmptyObj(out ho_A2_Reduced);
			HOperatorSet.GenEmptyObj(out ho_A2Region);
			HOperatorSet.GenEmptyObj(out ho_A2_RegionBorder);
			HOperatorSet.GenEmptyObj(out ho_A2_RegionDilation);
			HOperatorSet.GenEmptyObj(out ho_A2_Edges);
			HOperatorSet.GenEmptyObj(out ho_A2_Rectangles);
			HOperatorSet.GenEmptyObj(out ho_A2Cross);

			//Measure: SDMS_A2
			//Author: John Hsieh
			//Date: 2012
			//ho_Image.Dispose();
			//HOperatorSet.ReadImage(out ho_Image, "Images/STD.bmp");
			//dev_open_window_fit_image(ho_Image, 0, 0, -1, -1, out hv_WindowHandle);
			// dev_update_off(...); only in hdevelop
			HOperatorSet.SetSystem("border_shape_models", "false");

			//****Message Args
			hv_msgOffsetY = 100;
			hv_msgOffsetX = 100;

			////****Model All
			//HOperatorSet.ReadShapeModel("D:/Projects/Halcon/SDMS/SDMS_Measure/Model/MatchingAll.shm",
			//	out hv_AllModelId);
			//ho_AllModelContours.Dispose();
			//HOperatorSet.GetShapeModelContours(out ho_AllModelContours, hv_AllModelId, 1);
			//HOperatorSet.FindShapeModel(ho_Image, hv_AllModelId, (new HTuple(0)).TupleRad()
			//	, (new HTuple(360)).TupleRad(), 0.5, 1, 0.5, "least_squares", 6, 0.75, out hv_AllModelRow,
			//	out hv_AllModelColumn, out hv_AllModelAngle, out hv_AllModelScore);

			//****Model Args

			//STD 中心點
			hv_STD_Row = 772;
			hv_STD_Col = 1003;

			//目前圖形 中心點
			hv_Img_Row = hv_AllModelRow.Clone();
			hv_Img_Col = hv_AllModelColumn.Clone();

			//目前圖形 Rotate Angle
			hv_Img_Rotate_Angle = hv_AllModelAngle.Clone();

			//目前圖形偏移量
			hv_OffsetRow = hv_Img_Row - hv_STD_Row;
			hv_OffsetCol = hv_Img_Col - hv_STD_Col;

			////****Display
			//if (HDevWindowStack.IsOpen())
			//{
			//	HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
			//}
			//if (HDevWindowStack.IsOpen())
			//{
			//	HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
			//}
			//*****A2
			//step 1.
			//尋找中心點 CL model matching (CL_X, CL_Y)
			hv_CL_X = 0;
			hv_CL_Y = 0;
			//****CL_1
			//STD CL_1_ 位置
			hv_STD_CL_1_Row = 403;
			hv_STD_CL_1_Col = 1054;

			//STD 向量 STD_CL_1_
			hv_STD_CL_1_V_Row = hv_STD_CL_1_Row - hv_STD_Row;
			hv_STD_CL_1_V_Col = hv_STD_CL_1_Col - hv_STD_Col;


			//CL_1_X, CL_1_Y 分量
			hv_CL_1_X = (hv_STD_CL_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_CL_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_CL_1_Y = (hv_STD_CL_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_CL_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 CL_1_ 位置
			hv_CL_1_Pos_Row = (hv_STD_Row + hv_CL_1_Y) + hv_OffsetRow;
			hv_CL_1_Pos_Col = (hv_STD_Col + hv_CL_1_X) + hv_OffsetCol;

			hv_CL_X = hv_CL_1_Pos_Col.Clone();
			hv_CL_Y = hv_CL_1_Pos_Row.Clone();

			//Step 1
			//找左邊數來第一個金手指的中心點 Finger
			hv_A2_Center_X = 0;
			hv_A2_Center_Y = 0;
			//****A2_1_1
			//STD A2_1_1_ 位置
			hv_STD_A2_1_1_Row = 410;
			hv_STD_A2_1_1_Col = 780;

			//STD 向量 STD_A2_1_1_
			hv_STD_A2_1_1_V_Row = hv_STD_A2_1_1_Row - hv_STD_Row;
			hv_STD_A2_1_1_V_Col = hv_STD_A2_1_1_Col - hv_STD_Col;


			//A2_1_1_X, A2_1_1_Y 分量
			hv_A2_1_1_X = (hv_STD_A2_1_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_A2_1_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_A2_1_1_Y = (hv_STD_A2_1_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_A2_1_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 A2_1_1_ 位置
			hv_A2_1_1_Pos_Row = (hv_STD_Row + hv_A2_1_1_Y) + hv_OffsetRow;
			hv_A2_1_1_Pos_Col = (hv_STD_Col + hv_A2_1_1_X) + hv_OffsetCol;

			//for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_AllModelScore.TupleLength()
			//	)) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
			//{
			//A2_ROI
			hv_A2_ROI_W = 40;
			hv_A2_ROI_H = 120;
			ho_A2_Region.Dispose();
			HOperatorSet.GenRectangle2(out ho_A2_Region, hv_A2_1_1_Pos_Row, hv_A2_1_1_Pos_Col,
				hv_Img_Rotate_Angle, hv_A2_ROI_W, hv_A2_ROI_H);
			//stop ()
			ho_A2_Reduced.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A2_Region, out ho_A2_Reduced);
			//fit_Rectangle
			//if (HDevWindowStack.IsOpen())
			//{
			//	//dev_display (Image)
			//}
			//if (HDevWindowStack.IsOpen())
			//{
			//	//dev_display (A2_Region)
			//}
			//stop ()
			//
			ho_A2Region.Dispose();
			HOperatorSet.FastThreshold(ho_A2_Reduced, out ho_A2Region, 50, 150, 20);
			ho_A2_RegionBorder.Dispose();
			HOperatorSet.Boundary(ho_A2Region, out ho_A2_RegionBorder, "inner");
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (A2_RegionBorder)
			}
			hv_Rec_W = 11;
			hv_Rec_H = 11;
			ho_A2_RegionDilation.Dispose();
			HOperatorSet.DilationRectangle1(ho_A2_RegionBorder, out ho_A2_RegionDilation,
				hv_Rec_W, hv_Rec_H);
			hv_A2_Alpha = 0.9;
			hv_a2low = 20;
			hv_a2high = 110;
			ho_A2_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A2_Reduced, out ho_A2_Edges, "canny", hv_A2_Alpha,
				hv_a2low, hv_a2high);
			//stop ()
			hv_a2Limit = 200;
			ho_A2_Rectangles.Dispose();
			HOperatorSet.SelectShapeXld(ho_A2_Edges, out ho_A2_Rectangles, "contlength",
				"and", hv_a2Limit, 99999);
			HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2_RecNumber);
			while ((int)(new HTuple(hv_A2_RecNumber.TupleGreater(1))) != 0)
			{
				hv_a2Limit = hv_a2Limit + 10;
				ho_A2_Rectangles.Dispose();
				HOperatorSet.SelectShapeXld(ho_A2_Edges, out ho_A2_Rectangles, "contlength",
					"and", hv_a2Limit, 99999);
				HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2_RecNumber);
			}
			//stop ()
			HOperatorSet.FitRectangle2ContourXld(ho_A2_Rectangles, "regression", -1, 0,
				0, 3, 2, out hv_A2Row, out hv_A2Column, out hv_A2Phi, out hv_A2Length1,
				out hv_A2Length2, out hv_A2PointOrder);
			HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2Number);
			//取A2
			if ((int)(new HTuple(hv_A2Number.TupleGreater(0))) != 0)
			{
				hv_A2_Center_X = hv_A2Column[0];
				hv_A2_Center_Y = hv_A2Row[0];
				ho_A2Cross.Dispose();
				HOperatorSet.GenCrossContourXld(out ho_A2Cross, hv_A2Row, hv_A2Column, 10,
					0);
				//if (HDevWindowStack.IsOpen())
				//{
				//	HOperatorSet.DispObj(ho_A2Cross, HDevWindowStack.GetActive());
				//}
				//stop ()
			}

			//}
			HOperatorSet.DistancePp(hv_A2_Center_Y, hv_A2_Center_X, hv_CL_Y, hv_CL_X, out hv_A2_Dist);
			mResult = new LineResult(hv_A2_Center_Y, hv_A2_Center_X, hv_CL_Y, hv_CL_X, hv_A2_Dist);

			//step 3
			//show Result
			//hv_A2_Dist = 0;
			//if ((int)((new HTuple(hv_A2_Center_X.TupleGreater(0))).TupleAnd(new HTuple(hv_A2_Center_Y.TupleGreater(
			//	0)))) != 0)
			//{
			//	//轉正
			//	hv_X1 = (hv_A2_Center_X * (hv_AllModelAngle.TupleCos())) - (hv_A2_Center_Y * (hv_AllModelAngle.TupleSin()
			//		));
			//	hv_X2 = (hv_CL_X * (hv_AllModelAngle.TupleCos())) - (hv_CL_Y * (hv_AllModelAngle.TupleSin()
			//		));

			//	hv_Y1 = (hv_A2_Center_X * (hv_AllModelAngle.TupleSin())) + (hv_A2_Center_Y * (hv_AllModelAngle.TupleCos()
			//		));
			//	hv_Y2 = (hv_CL_X * (hv_AllModelAngle.TupleSin())) + (hv_CL_Y * (hv_AllModelAngle.TupleCos()
			//		));

			//	//disp_cross (WindowHandle, CL_Y, CL_X, 10, 0)
			//	//disp_cross (WindowHandle, A2_Center_Y, A2_Center_X, 10, 0)
			//	hv_A2_Dist = ((hv_X1 - hv_X2)).TupleAbs();
			//	//****Mark_A2_C
			//	//STD Mark_A2_C_ 位置
			//	hv_STD_Mark_A2_C_Row = 420;
			//	hv_STD_Mark_A2_C_Col = 890;

			//	//STD Mark_A2_Start_ 位置
			//	hv_STD_Mark_A2_Start_Row = 420;
			//	hv_STD_Mark_A2_Start_Col = 780;

			//	//STD Mark_A2_End_ 位置
			//	hv_STD_Mark_A2_End_Row = 420;
			//	hv_STD_Mark_A2_End_Col = 1055;


			//	//STD 向量 STD_Mark_A2_C_
			//	hv_STD_Mark_A2_C_V_Row = hv_STD_Mark_A2_C_Row - hv_STD_Row;
			//	hv_STD_Mark_A2_C_V_Col = hv_STD_Mark_A2_C_Col - hv_STD_Col;

			//	//Mark_A2_C_X, Mark_A2_C_Y 分量
			//	hv_Mark_A2_C_X = (hv_STD_Mark_A2_C_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_Mark_A2_C_V_Row * (hv_Img_Rotate_Angle.TupleSin()
			//		));
			//	hv_Mark_A2_C_Y = (hv_STD_Mark_A2_C_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_Mark_A2_C_V_Col * (hv_Img_Rotate_Angle.TupleSin()
			//		));

			//	//目前圖形 Mark_A2_C_ 位置
			//	hv_Mark_A2_C_Pos_Row = (hv_STD_Row + hv_Mark_A2_C_Y) + hv_OffsetRow;
			//	hv_Mark_A2_C_Pos_Col = (hv_STD_Col + hv_Mark_A2_C_X) + hv_OffsetCol;



			//	//STD 向量 STD_Mark_A2_Start_
			//	hv_STD_Mark_A2_Start_V_Row = hv_STD_Mark_A2_Start_Row - hv_STD_Row;
			//	hv_STD_Mark_A2_Start_V_Col = hv_STD_Mark_A2_Start_Col - hv_STD_Col;

			//	//Mark_A2_Start_X, Mark_A2_Start_Y 分量
			//	hv_Mark_A2_Start_X = (hv_STD_Mark_A2_Start_V_Col * (hv_Img_Rotate_Angle.TupleCos()
			//		)) + (hv_STD_Mark_A2_Start_V_Row * (hv_Img_Rotate_Angle.TupleSin()));
			//	hv_Mark_A2_Start_Y = (hv_STD_Mark_A2_Start_V_Row * (hv_Img_Rotate_Angle.TupleCos()
			//		)) - (hv_STD_Mark_A2_Start_V_Col * (hv_Img_Rotate_Angle.TupleSin()));

			//	//目前圖形 Mark_A2_Start_ 位置
			//	hv_Mark_A2_Start_Pos_Row = (hv_STD_Row + hv_Mark_A2_Start_Y) + hv_OffsetRow;
			//	hv_Mark_A2_Start_Pos_Col = (hv_STD_Col + hv_Mark_A2_Start_X) + hv_OffsetCol;

			//	//STD 向量 STD_Mark_A2_End_
			//	hv_STD_Mark_A2_End_V_Row = hv_STD_Mark_A2_End_Row - hv_STD_Row;
			//	hv_STD_Mark_A2_End_V_Col = hv_STD_Mark_A2_End_Col - hv_STD_Col;

			//	//Mark_A2_End_X, Mark_A2_End_Y 分量
			//	hv_Mark_A2_End_X = (hv_STD_Mark_A2_End_V_Col * (hv_Img_Rotate_Angle.TupleCos()
			//		)) + (hv_STD_Mark_A2_End_V_Row * (hv_Img_Rotate_Angle.TupleSin()));
			//	hv_Mark_A2_End_Y = (hv_STD_Mark_A2_End_V_Row * (hv_Img_Rotate_Angle.TupleCos()
			//		)) - (hv_STD_Mark_A2_End_V_Col * (hv_Img_Rotate_Angle.TupleSin()));

			//	//目前圖形 Mark_A2_End_ 位置
			//	hv_Mark_A2_End_Pos_Row = (hv_STD_Row + hv_Mark_A2_End_Y) + hv_OffsetRow;
			//	hv_Mark_A2_End_Pos_Col = (hv_STD_Col + hv_Mark_A2_End_X) + hv_OffsetCol;

			//	//HOperatorSet.SetTposition(hv_WindowHandle, hv_Mark_A2_C_Pos_Row - 25, hv_Mark_A2_C_Pos_Col);
			//	//HOperatorSet.WriteString(hv_WindowHandle, "A2");
			//	//HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A2_C_Pos_Row, hv_Mark_A2_C_Pos_Col - 50,
			//	//	hv_Mark_A2_Start_Pos_Row, hv_Mark_A2_Start_Pos_Col, 5);
			//	//HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A2_C_Pos_Row, hv_Mark_A2_C_Pos_Col + 50,
			//	//	hv_Mark_A2_End_Pos_Row, hv_Mark_A2_End_Pos_Col, 5);
			//}
			//hv_MeasureReasult = hv_A2_Dist.Clone();
			//*****A2 End
			//ho_Image.Dispose();
			//ho_AllModelContours.Dispose();
			ho_A2_Region.Dispose();
			ho_A2_Reduced.Dispose();
			ho_A2Region.Dispose();
			ho_A2_RegionBorder.Dispose();
			ho_A2_RegionDilation.Dispose();
			ho_A2_Edges.Dispose();
			ho_A2_Rectangles.Dispose();
			ho_A2Cross.Dispose();


			return mResult;

		}

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
	}
}
