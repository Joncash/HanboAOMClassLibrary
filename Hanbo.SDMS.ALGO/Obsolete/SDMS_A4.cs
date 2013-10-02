using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_A4 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			LineResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_A2_Region = null;
			HObject ho_A2_Reduced = null, ho_A2Region = null, ho_A2_RegionBorder = null;
			HObject ho_A2_RegionDilation = null, ho_A2_Edges = null, ho_A2_Rectangles = null;
			HObject ho_A2Cross = null, ho_A4Cross = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_A2_Center_X, hv_A2_Center_Y;
			HTuple hv_STD_A2_1_1_Row, hv_STD_A2_1_1_Col, hv_STD_A2_1_1_V_Row;
			HTuple hv_STD_A2_1_1_V_Col, hv_A2_1_1_X, hv_A2_1_1_Y, hv_A2_1_1_Pos_Row;
			HTuple hv_A2_1_1_Pos_Col, hv_A2_ROI_W = new HTuple();
			HTuple hv_A2_ROI_H = new HTuple(), hv_Rec_W = new HTuple();
			HTuple hv_Rec_H = new HTuple(), hv_A2_Alpha = new HTuple();
			HTuple hv_a2low = new HTuple(), hv_a2high = new HTuple(), hv_a2Limit = new HTuple();
			HTuple hv_A2_RecNumber = new HTuple(), hv_A2Row = new HTuple();
			HTuple hv_A2Column = new HTuple(), hv_A2Phi = new HTuple();
			HTuple hv_A2Length1 = new HTuple(), hv_A2Length2 = new HTuple();
			HTuple hv_A2PointOrder = new HTuple(), hv_A2Number = new HTuple();
			HTuple hv_A4_Center_X, hv_A4_Center_Y, hv_STD_A4_Row, hv_STD_A4_Col;
			HTuple hv_STD_A4_V_Row, hv_STD_A4_V_Col, hv_A4_X, hv_A4_Y;
			HTuple hv_A4_Pos_Row, hv_A4_Pos_Col, hv_mini = new HTuple();
			HTuple hv_X1 = new HTuple(), hv_X2 = new HTuple();
			HTuple hv_Y1 = new HTuple(), hv_Y2 = new HTuple(), hv_STD_Mark_A4_C_Row = new HTuple();
			HTuple hv_STD_Mark_A4_C_Col = new HTuple(), hv_STD_Mark_A4_Start_Row = new HTuple();
			HTuple hv_STD_Mark_A4_Start_Col = new HTuple(), hv_STD_Mark_A4_End_Row = new HTuple();
			HTuple hv_STD_Mark_A4_End_Col = new HTuple(), hv_STD_Mark_A4_C_V_Row = new HTuple();
			HTuple hv_STD_Mark_A4_C_V_Col = new HTuple(), hv_Mark_A4_C_X = new HTuple();
			HTuple hv_Mark_A4_C_Y = new HTuple(), hv_Mark_A4_C_Pos_Row = new HTuple();
			HTuple hv_Mark_A4_C_Pos_Col = new HTuple(), hv_STD_Mark_A4_Start_V_Row = new HTuple();
			HTuple hv_STD_Mark_A4_Start_V_Col = new HTuple(), hv_Mark_A4_Start_X = new HTuple();
			HTuple hv_Mark_A4_Start_Y = new HTuple(), hv_Mark_A4_Start_Pos_Row = new HTuple();
			HTuple hv_Mark_A4_Start_Pos_Col = new HTuple(), hv_STD_Mark_A4_End_V_Row = new HTuple();
			HTuple hv_STD_Mark_A4_End_V_Col = new HTuple(), hv_Mark_A4_End_X = new HTuple();
			HTuple hv_Mark_A4_End_Y = new HTuple(), hv_Mark_A4_End_Pos_Row = new HTuple();
			HTuple hv_Mark_A4_End_Pos_Col = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_A2_Region);
			HOperatorSet.GenEmptyObj(out ho_A2_Reduced);
			HOperatorSet.GenEmptyObj(out ho_A2Region);
			HOperatorSet.GenEmptyObj(out ho_A2_RegionBorder);
			HOperatorSet.GenEmptyObj(out ho_A2_RegionDilation);
			HOperatorSet.GenEmptyObj(out ho_A2_Edges);
			HOperatorSet.GenEmptyObj(out ho_A2_Rectangles);
			HOperatorSet.GenEmptyObj(out ho_A2Cross);
			HOperatorSet.GenEmptyObj(out ho_A4Cross);

			//Measure: SDMS_A4
			//Author: John Hsieh
			//Date: 2012
			//Note: A4 related to A2
			//ho_Image.Dispose();
			//HOperatorSet.ReadImage(out ho_Image, "Images/STD.bmp");
			//dev_open_window_fit_image(ho_Image, 0, 0, -1, -1, out hv_WindowHandle);
			// dev_update_off(...); only in hdevelop
			HOperatorSet.SetSystem("border_shape_models", "false");

			//****Message Args
			hv_msgOffsetY = 100;
			hv_msgOffsetX = 100;


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



			//*****A4
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
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (Image)
			}
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (A2_Region)
			}
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
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (A2_Edges)
			}
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
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (A2_Rectangles)
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

				mResult = new LineResult()
				{
					Row1 = new HTuple(hv_A2Row),
					Col1 = new HTuple(hv_A2Column),
				};
			}




			//Step2
			//找左邊數來第二個金手指的中心點 Finger
			hv_A4_Center_X = 0;
			hv_A4_Center_Y = 0;
			//****A4
			//STD A4_ 位置
			hv_STD_A4_Row = 418;
			hv_STD_A4_Col = 773;

			//STD 向量 STD_A4_
			hv_STD_A4_V_Row = hv_STD_A4_Row - hv_STD_Row;
			hv_STD_A4_V_Col = hv_STD_A4_Col - hv_STD_Col;


			//A4_X, A4_Y 分量
			hv_A4_X = (hv_STD_A4_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_A4_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_A4_Y = (hv_STD_A4_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_A4_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 A4_ 位置
			hv_A4_Pos_Row = (hv_STD_Row + hv_A4_Y) + hv_OffsetRow;
			hv_A4_Pos_Col = (hv_STD_Col + hv_A4_X) + hv_OffsetCol;


			//A2_ROI
			hv_A2_ROI_W = 160;
			hv_A2_ROI_H = 130;
			ho_A2_Region.Dispose();
			HOperatorSet.GenRectangle2(out ho_A2_Region, hv_A4_Pos_Row, hv_A4_Pos_Col,
				hv_Img_Rotate_Angle, hv_A2_ROI_W, hv_A2_ROI_H);
			//stop ()
			ho_A2_Reduced.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A2_Region, out ho_A2_Reduced);
			//fit_Rectangle
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (A2_Region)
			}
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
			hv_A2_Alpha = 4;
			ho_A2_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A2_Reduced, out ho_A2_Edges, "canny", hv_A2_Alpha,
				10, 50);
			hv_mini = 350;
			ho_A2_Rectangles.Dispose();
			HOperatorSet.SelectShapeXld(ho_A2_Edges, out ho_A2_Rectangles, "contlength",
				"and", hv_mini, 99999);
			HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2_RecNumber);
			while ((int)(new HTuple(hv_A2_RecNumber.TupleGreater(2))) != 0)
			{
				hv_mini = hv_mini + 10;
				ho_A2_Rectangles.Dispose();
				HOperatorSet.SelectShapeXld(ho_A2_Edges, out ho_A2_Rectangles, "contlength",
					"and", hv_mini, 99999);
				HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2_RecNumber);
			}
			HOperatorSet.FitRectangle2ContourXld(ho_A2_Rectangles, "regression", -1, 0,
				0, 3, 2, out hv_A2Row, out hv_A2Column, out hv_A2Phi, out hv_A2Length1,
				out hv_A2Length2, out hv_A2PointOrder);
			HOperatorSet.CountObj(ho_A2_Rectangles, out hv_A2Number);
			//取A4
			if ((int)(new HTuple(hv_A2Number.TupleGreater(1))) != 0)
			{
				//取 A4Object 資訊
				hv_A4_Center_X = hv_A2Column[1];
				hv_A4_Center_Y = hv_A2Row[1];
				ho_A4Cross.Dispose();
				HOperatorSet.GenCrossContourXld(out ho_A4Cross, hv_A4_Center_Y, hv_A4_Center_X, 10, 0);

				if (mResult != null)
				{
					HOperatorSet.DistancePp(mResult.Row1, mResult.Col1, hv_A4_Center_Y, hv_A4_Center_X, out mResult.Distance);
					mResult.Row2 = new HTuple(hv_A4_Center_Y);
					mResult.Col2 = new HTuple(hv_A4_Center_X);
				}

			}



			//*****A4 End
			ho_A2_Region.Dispose();
			ho_A2_Reduced.Dispose();
			ho_A2Region.Dispose();
			ho_A2_RegionBorder.Dispose();
			ho_A2_RegionDilation.Dispose();
			ho_A2_Edges.Dispose();
			ho_A2_Rectangles.Dispose();
			ho_A2Cross.Dispose();
			ho_A4Cross.Dispose();


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
