using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_A1 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			LineResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_A1Rectangle = null;
			HObject ho_A1_ROI = null, ho_A1Edges = null, ho_A1ContoursSplit = null;
			HObject ho_SelectedA1Contours = null, ho_TopLineContour = null;
			HObject ho_B_Rectangle = null, ho_B__ROI = null, ho_B_Edges = null;
			HObject ho_B_ContoursSplit = null, ho_SelectedB_ContoursSplit = null;
			HObject ho_B_LeftLine_Contour = null, ho_Cross, ho_A1_RRectangle = null;
			HObject ho_A1_R_ROI = null, ho_A1_REdges = null, ho_A1_RContoursSplit = null;
			HObject ho_SelectedA1_RContoursSplit = null, ho_B_RightLine_Contour = null;
			HObject ho_CrossQ;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			//HTuple hv_AllModelId, hv_AllModelRow, hv_AllModelColumn;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_A1_1_1_Row, hv_STD_A1_1_1_Col;
			HTuple hv_STD_A1_1_1_V_Row, hv_STD_A1_1_1_V_Col, hv_A1_1_1_X;
			HTuple hv_A1_1_1_Y, hv_A1_1_1_Pos_Row, hv_A1_1_1_Pos_Col;
			HTuple hv_ROI_W = new HTuple(), hv_ROI_H = new HTuple();
			HTuple hv_low = new HTuple(), hv_high = new HTuple(), hv_Attrib = new HTuple();
			HTuple hv_A1RowBegin = new HTuple(), hv_A1ColBegin = new HTuple();
			HTuple hv_A1RowEnd = new HTuple(), hv_A1ColEnd = new HTuple();
			HTuple hv_A1Nr = new HTuple(), hv_A1Nc = new HTuple(), hv_A1Dist = new HTuple();
			HTuple hv_STD_B1_1_1_Row, hv_STD_B1_1_1_Col, hv_STD_B1_1_1_V_Row;
			HTuple hv_STD_B1_1_1_V_Col, hv_B1_1_1_X, hv_B1_1_1_Y, hv_B1_1_1_Pos_Row;
			HTuple hv_B1_1_1_Pos_Col, hv_B_ROI_W = new HTuple(), hv_B_ROI_H = new HTuple();
			HTuple hv_B_EdgesNumber = new HTuple(), hv_inc = new HTuple();
			HTuple hv_B_RowBegin = new HTuple(), hv_B_ColBegin = new HTuple();
			HTuple hv_B_RowEnd = new HTuple(), hv_B_ColEnd = new HTuple();
			HTuple hv_B_Nr = new HTuple(), hv_B_Nc = new HTuple(), hv_B_Dist = new HTuple();
			HTuple hv_B1_InterY, hv_B1_InterX, hv_B1_IsParallel1, hv_STD_B_1_1_Row;
			HTuple hv_STD_B_1_1_Col, hv_STD_B_1_1_V_Row, hv_STD_B_1_1_V_Col;
			HTuple hv_B_1_1_X, hv_B_1_1_Y, hv_B_1_1_Pos_Row, hv_B_1_1_Pos_Col;
			HTuple hv_A1_RNumber = new HTuple(), hv_A1_RRowBegin = new HTuple();
			HTuple hv_A1_RColBegin = new HTuple(), hv_A1_RRowEnd = new HTuple();
			HTuple hv_A1_RColEnd = new HTuple(), hv_A1_RNr = new HTuple();
			HTuple hv_A1_RNc = new HTuple(), hv_A1_RDist = new HTuple();
			HTuple hv_Q_InterY, hv_Q_InterX, hv_Q_IsParallel1, hv_DistanceMin;
			HTuple hv_DistanceMax, hv_DistanceMinP, hv_DistanceMaxP;
			HTuple hv_DistanceMin1, hv_DistanceMin1P, hv_DistanceMin2;
			HTuple hv_X1 = new HTuple(), hv_X2 = new HTuple(), hv_Y1 = new HTuple();
			HTuple hv_Y2 = new HTuple(), hv_STD_Mark_A1_C_Row = new HTuple();
			HTuple hv_STD_Mark_A1_C_Col = new HTuple(), hv_STD_Mark_A1_Start_Row = new HTuple();
			HTuple hv_STD_Mark_A1_Start_Col = new HTuple(), hv_STD_Mark_A1_End_Row = new HTuple();
			HTuple hv_STD_Mark_A1_End_Col = new HTuple(), hv_STD_Mark_A1_C_V_Row = new HTuple();
			HTuple hv_STD_Mark_A1_C_V_Col = new HTuple(), hv_Mark_A1_C_X = new HTuple();
			HTuple hv_Mark_A1_C_Y = new HTuple(), hv_Mark_A1_C_Pos_Row = new HTuple();
			HTuple hv_Mark_A1_C_Pos_Col = new HTuple(), hv_STD_Mark_A1_Start_V_Row = new HTuple();
			HTuple hv_STD_Mark_A1_Start_V_Col = new HTuple(), hv_Mark_A1_Start_X = new HTuple();
			HTuple hv_Mark_A1_Start_Y = new HTuple(), hv_Mark_A1_Start_Pos_Row = new HTuple();
			HTuple hv_Mark_A1_Start_Pos_Col = new HTuple(), hv_STD_Mark_A1_End_V_Row = new HTuple();
			HTuple hv_STD_Mark_A1_End_V_Col = new HTuple(), hv_Mark_A1_End_X = new HTuple();
			HTuple hv_Mark_A1_End_Y = new HTuple(), hv_Mark_A1_End_Pos_Row = new HTuple();
			HTuple hv_Mark_A1_End_Pos_Col = new HTuple();

			// Initialize local and output iconic variables 
			//HOperatorSet.GenEmptyObj(out ho_Image);
			//HOperatorSet.GenEmptyObj(out ho_AllModelContours);
			HOperatorSet.GenEmptyObj(out ho_A1Rectangle);
			HOperatorSet.GenEmptyObj(out ho_A1_ROI);
			HOperatorSet.GenEmptyObj(out ho_A1Edges);
			HOperatorSet.GenEmptyObj(out ho_A1ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_SelectedA1Contours);
			HOperatorSet.GenEmptyObj(out ho_TopLineContour);
			HOperatorSet.GenEmptyObj(out ho_B_Rectangle);
			HOperatorSet.GenEmptyObj(out ho_B__ROI);
			HOperatorSet.GenEmptyObj(out ho_B_Edges);
			HOperatorSet.GenEmptyObj(out ho_B_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_SelectedB_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_B_LeftLine_Contour);
			HOperatorSet.GenEmptyObj(out ho_Cross);
			HOperatorSet.GenEmptyObj(out ho_A1_RRectangle);
			HOperatorSet.GenEmptyObj(out ho_A1_R_ROI);
			HOperatorSet.GenEmptyObj(out ho_A1_REdges);
			HOperatorSet.GenEmptyObj(out ho_A1_RContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_SelectedA1_RContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_B_RightLine_Contour);
			HOperatorSet.GenEmptyObj(out ho_CrossQ);

			//Measure: SDMS_A1
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

			//****Model All
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
			//*****A1
			//(1)A1 線段 與 B1線段的交點，取得 點 P
			//****A1_1_1
			//STD A1_1_1_ 位置
			hv_STD_A1_1_1_Row = 240;
			hv_STD_A1_1_1_Col = 1080;

			//STD 向量 STD_A1_1_1_
			hv_STD_A1_1_1_V_Row = hv_STD_A1_1_1_Row - hv_STD_Row;
			hv_STD_A1_1_1_V_Col = hv_STD_A1_1_1_Col - hv_STD_Col;


			//A1_1_1_X, A1_1_1_Y 分量
			hv_A1_1_1_X = (hv_STD_A1_1_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_A1_1_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_A1_1_1_Y = (hv_STD_A1_1_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_A1_1_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 A1_1_1_ 位置
			hv_A1_1_1_Pos_Row = (hv_STD_Row + hv_A1_1_1_Y) + hv_OffsetRow;
			hv_A1_1_1_Pos_Col = (hv_STD_Col + hv_A1_1_1_X) + hv_OffsetCol;

			//for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_AllModelScore.TupleLength()
			//	)) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
			//{
			//step 1 由 matchingObj 得知目標區域中心座標 (A1ModelRow, A1ModelColumn... 等資訊) 建立 ROI
			hv_ROI_W = 200;
			hv_ROI_H = 20;
			ho_A1Rectangle.Dispose();
			HOperatorSet.GenRectangle2(out ho_A1Rectangle, hv_A1_1_1_Pos_Row, hv_A1_1_1_Pos_Col,
				hv_Img_Rotate_Angle, hv_ROI_W, hv_ROI_H);
			ho_A1_ROI.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A1Rectangle, out ho_A1_ROI);
			//step 2 Extract contours and segment it
			hv_low = 70;
			hv_high = 135;
			ho_A1Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A1_ROI, out ho_A1Edges, "lanser2", 1.5, hv_low,
				hv_high);
			ho_A1ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_A1Edges, out ho_A1ContoursSplit, "lines_circles",
				9, 4, 2);
			ho_SelectedA1Contours.Dispose();
			HOperatorSet.SelectShapeXld(ho_A1ContoursSplit, out ho_SelectedA1Contours,
				"contlength", "and", 150, 99999);

			//step 3 取得目標線段
			ho_TopLineContour.Dispose();
			HOperatorSet.SelectObj(ho_SelectedA1Contours, out ho_TopLineContour, 1);
			HOperatorSet.GetContourGlobalAttribXld(ho_TopLineContour, "cont_approx", out hv_Attrib);
			//線段 A1 的資訊
			HOperatorSet.FitLineContourXld(ho_TopLineContour, "tukey", -1, 0, 5, 2, out hv_A1RowBegin,
				out hv_A1ColBegin, out hv_A1RowEnd, out hv_A1ColEnd, out hv_A1Nr, out hv_A1Nc,
				out hv_A1Dist);
			//	if (HDevWindowStack.IsOpen())
			//	{
			//		HOperatorSet.DispObj(ho_TopLineContour, HDevWindowStack.GetActive());
			//	}
			//	//stop ()
			//}

			//*找線段 B1
			//****B1_1_1
			//STD B1_1_1_ 位置
			hv_STD_B1_1_1_Row = 400;
			hv_STD_B1_1_1_Col = 700;

			//STD 向量 STD_B1_1_1_
			hv_STD_B1_1_1_V_Row = hv_STD_B1_1_1_Row - hv_STD_Row;
			hv_STD_B1_1_1_V_Col = hv_STD_B1_1_1_Col - hv_STD_Col;


			//B1_1_1_X, B1_1_1_Y 分量
			hv_B1_1_1_X = (hv_STD_B1_1_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_B1_1_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_B1_1_1_Y = (hv_STD_B1_1_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_B1_1_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));

			//目前圖形 B1_1_1_ 位置
			hv_B1_1_1_Pos_Row = (hv_STD_Row + hv_B1_1_1_Y) + hv_OffsetRow;
			hv_B1_1_1_Pos_Col = (hv_STD_Col + hv_B1_1_1_X) + hv_OffsetCol;
			//for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_AllModelScore.TupleLength()
			//	)) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
			//{
			//step 1 由 matchingObj 得知目標區域中心座標 (B_ModelRow, B_ModelColumn... 等資訊) 建立 ROI
			hv_B_ROI_W = 40;
			hv_B_ROI_H = 90;
			ho_B_Rectangle.Dispose();
			HOperatorSet.GenRectangle2(out ho_B_Rectangle, hv_B1_1_1_Pos_Row, hv_B1_1_1_Pos_Col,
				hv_Img_Rotate_Angle, hv_B_ROI_W, hv_B_ROI_H);
			//stop ()
			ho_B__ROI.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_B_Rectangle, out ho_B__ROI);
			//step 2 Extract contours and segment it
			hv_low = 60;
			hv_high = 140;
			ho_B_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_B__ROI, out ho_B_Edges, "lanser2", 0.3, hv_low,
				hv_high);
			HOperatorSet.CountObj(ho_B_Edges, out hv_B_EdgesNumber);

			//stop ()
			//修正上限
			hv_inc = -3;
			while ((int)(new HTuple(hv_B_EdgesNumber.TupleLess(1))) != 0)
			{
				hv_high = hv_high + hv_inc;
				ho_B_Edges.Dispose();
				HOperatorSet.EdgesSubPix(ho_B__ROI, out ho_B_Edges, "lanser2", 0.3, hv_low,
					hv_high);
				HOperatorSet.CountObj(ho_B_Edges, out hv_B_EdgesNumber);
			}
			//stop ()
			ho_B_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_B_Edges, out ho_B_ContoursSplit, "lines",
				9, 4, 2);
			ho_SelectedB_ContoursSplit.Dispose();
			HOperatorSet.SelectShapeXld(ho_B_ContoursSplit, out ho_SelectedB_ContoursSplit,
				"contlength", "and", 20, 99999);
			//step 3 取得目標線段
			ho_B_LeftLine_Contour.Dispose();
			HOperatorSet.SelectObj(ho_SelectedB_ContoursSplit, out ho_B_LeftLine_Contour,
				1);
			HOperatorSet.GetContourGlobalAttribXld(ho_B_LeftLine_Contour, "cont_approx",
				out hv_Attrib);
			//線段 B_ 的資訊
			HOperatorSet.FitLineContourXld(ho_B_LeftLine_Contour, "tukey", -1, 0, 5, 2,
				out hv_B_RowBegin, out hv_B_ColBegin, out hv_B_RowEnd, out hv_B_ColEnd,
				out hv_B_Nr, out hv_B_Nc, out hv_B_Dist);
			//	if (HDevWindowStack.IsOpen())
			//	{
			//		HOperatorSet.DispObj(ho_B_LeftLine_Contour, HDevWindowStack.GetActive());
			//	}
			//	//stop ()
			//}

			//計算 A1 與 B 之交點 P
			HOperatorSet.IntersectionLl(hv_B_RowBegin, hv_B_ColBegin, hv_B_RowEnd, hv_B_ColEnd,
				hv_A1RowBegin, hv_A1ColBegin, hv_A1RowEnd, hv_A1ColEnd, out hv_B1_InterY,
				out hv_B1_InterX, out hv_B1_IsParallel1);

			mResult = new LineResult()
			{
				Row1 = new HTuple(hv_B1_InterY),
				Col1 = new HTuple(hv_B1_InterX)
			};

			//ho_Cross.Dispose();
			//HOperatorSet.GenCrossContourXld(out ho_Cross, hv_B1_InterY, hv_B1_InterX, 0,
			//	0);
			//if (HDevWindowStack.IsOpen())
			//{
			//	//dev_display (Image)
			//}
			//if (HDevWindowStack.IsOpen())
			//{
			//	HOperatorSet.DispObj(ho_Cross, HDevWindowStack.GetActive());
			//}
			//stop ()
			//part 2 ,  計算另一邊的交點 Q
			//****B_1_1
			//STD B_1_1_ 位置
			hv_STD_B_1_1_Row = 800;
			hv_STD_B_1_1_Col = 1400;

			//STD 向量 STD_B_1_1_
			hv_STD_B_1_1_V_Row = hv_STD_B_1_1_Row - hv_STD_Row;
			hv_STD_B_1_1_V_Col = hv_STD_B_1_1_Col - hv_STD_Col;


			//B_1_1_X, B_1_1_Y 分量
			hv_B_1_1_X = (hv_STD_B_1_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_B_1_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_B_1_1_Y = (hv_STD_B_1_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_B_1_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 B_1_1_ 位置
			hv_B_1_1_Pos_Row = (hv_STD_Row + hv_B_1_1_Y) + hv_OffsetRow;
			hv_B_1_1_Pos_Col = (hv_STD_Col + hv_B_1_1_X) + hv_OffsetCol;

			//for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_AllModelScore.TupleLength()
			//	)) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
			//{
			//step 1 由 matchingObj 得知目標區域中心座標 (A1_RModelRow, A1_RModelColumn... 等資訊) 建立 ROI
			hv_ROI_W = 50;
			hv_ROI_H = 150;
			ho_A1_RRectangle.Dispose();
			HOperatorSet.GenRectangle2(out ho_A1_RRectangle, hv_B_1_1_Pos_Row, hv_B_1_1_Pos_Col,
				hv_Img_Rotate_Angle, hv_ROI_W, hv_ROI_H);
			ho_A1_R_ROI.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A1_RRectangle, out ho_A1_R_ROI);
			//stop ()
			//step 2 Extract contours and segment it
			hv_low = 60;
			hv_high = 140;
			ho_A1_REdges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A1_R_ROI, out ho_A1_REdges, "lanser2", 0.3, hv_low,
				hv_high);
			HOperatorSet.CountObj(ho_A1_REdges, out hv_A1_RNumber);
			//修正上限
			hv_inc = -2;
			while ((int)(new HTuple(hv_A1_RNumber.TupleLess(1))) != 0)
			{
				hv_high = hv_high + hv_inc;
				ho_A1_REdges.Dispose();
				HOperatorSet.EdgesSubPix(ho_A1_R_ROI, out ho_A1_REdges, "lanser2", 0.3, hv_low,
					hv_high);
				HOperatorSet.CountObj(ho_A1_REdges, out hv_A1_RNumber);
			}
			ho_A1_RContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_A1_REdges, out ho_A1_RContoursSplit, "lines",
				9, 4, 2);
			ho_SelectedA1_RContoursSplit.Dispose();
			HOperatorSet.SelectShapeXld(ho_A1_RContoursSplit, out ho_SelectedA1_RContoursSplit,
				"contlength", "and", 20, 99999);
			//stop ()
			//step 3 取得目標線段
			ho_B_RightLine_Contour.Dispose();
			HOperatorSet.SelectObj(ho_SelectedA1_RContoursSplit, out ho_B_RightLine_Contour,
				1);
			//if (HDevWindowStack.IsOpen())
			//{
			//	//dev_set_color ('yellow')
			//}
			HOperatorSet.GetContourGlobalAttribXld(ho_B_RightLine_Contour, "cont_approx",
				out hv_Attrib);
			//線段 A1_R 的資訊
			HOperatorSet.FitLineContourXld(ho_B_RightLine_Contour, "tukey", -1, 0, 5, 2,
				out hv_A1_RRowBegin, out hv_A1_RColBegin, out hv_A1_RRowEnd, out hv_A1_RColEnd,
				out hv_A1_RNr, out hv_A1_RNc, out hv_A1_RDist);
			//	if (HDevWindowStack.IsOpen())
			//	{
			//		HOperatorSet.DispObj(ho_B_RightLine_Contour, HDevWindowStack.GetActive());
			//	}
			//	//stop ()
			//}
			//交點 Q

			//計算 A1 與 A1_R 之交點 Q'
			HOperatorSet.IntersectionLl(hv_A1_RRowBegin, hv_A1_RColBegin, hv_A1_RRowEnd,
				hv_A1_RColEnd, hv_A1RowBegin, hv_A1ColBegin, hv_A1RowEnd, hv_A1ColEnd, out hv_Q_InterY,
				out hv_Q_InterX, out hv_Q_IsParallel1);

			mResult.Row2 = new HTuple(hv_Q_InterY);
			mResult.Col2 = new HTuple(hv_Q_InterX);

			//ho_CrossQ.Dispose();
			//HOperatorSet.GenCrossContourXld(out ho_CrossQ, hv_Q_InterY, hv_Q_InterX, 0, 0);
			//if (HDevWindowStack.IsOpen())
			//{
			//	//dev_display (Image)
			//}
			//if (HDevWindowStack.IsOpen())
			//{
			//	HOperatorSet.DispObj(ho_CrossQ, HDevWindowStack.GetActive());
			//}
			HOperatorSet.DistanceCc(ho_Cross, ho_CrossQ, "point_to_point", out hv_DistanceMin,
				out hv_DistanceMax);
			HOperatorSet.DistanceCc(ho_Cross, ho_CrossQ, "point_to_segment", out hv_DistanceMinP,
				out hv_DistanceMaxP);
			HOperatorSet.DistanceCcMin(ho_Cross, ho_CrossQ, "fast_point_to_segment", out hv_DistanceMin1);
			HOperatorSet.DistanceCcMin(ho_Cross, ho_CrossQ, "point_to_segment", out hv_DistanceMin1P);
			HOperatorSet.DistanceCcMin(ho_B_LeftLine_Contour, ho_B_RightLine_Contour, "fast_point_to_segment",
				out hv_DistanceMin2);

			#region 距離
			HOperatorSet.DistancePp(mResult.Row1, mResult.Col1, mResult.Row2, mResult.Col2, out mResult.Distance);
			#endregion
			
			//687.113
			//699.113
			//HDevelopStop();
			//Final, 計算 PQ 線段

			//if ((int)((new HTuple(hv_B1_InterY.TupleGreater(0))).TupleAnd(new HTuple(hv_Q_InterY.TupleGreater(
			//	0)))) != 0)
			//{
			//	//轉正
			//	hv_X1 = (hv_B1_InterX * (hv_AllModelAngle.TupleCos())) - (hv_B1_InterY * (hv_AllModelAngle.TupleSin()
			//		));
			//	hv_X2 = (hv_Q_InterX * (hv_AllModelAngle.TupleCos())) - (hv_Q_InterY * (hv_AllModelAngle.TupleSin()
			//		));

			//	hv_Y1 = (hv_B1_InterX * (hv_AllModelAngle.TupleSin())) + (hv_B1_InterY * (hv_AllModelAngle.TupleCos()
			//		));
			//	hv_Y2 = (hv_Q_InterX * (hv_AllModelAngle.TupleSin())) + (hv_Q_InterY * (hv_AllModelAngle.TupleCos()
			//		));

			//	hv_A1Dist = ((hv_X1 - hv_X2)).TupleAbs();


			//	//****Mark_A1_C
			//	//STD Mark_A1_C_ 位置
			//	hv_STD_Mark_A1_C_Row = 102;
			//	hv_STD_Mark_A1_C_Col = 1035;

			//	//STD Mark_A1_Start_ 位置
			//	hv_STD_Mark_A1_Start_Row = 102;
			//	hv_STD_Mark_A1_Start_Col = 690;

			//	//STD Mark_A1_End_ 位置
			//	hv_STD_Mark_A1_End_Row = 102;
			//	hv_STD_Mark_A1_End_Col = 1410;


			//	//STD 向量 STD_Mark_A1_C_
			//	hv_STD_Mark_A1_C_V_Row = hv_STD_Mark_A1_C_Row - hv_STD_Row;
			//	hv_STD_Mark_A1_C_V_Col = hv_STD_Mark_A1_C_Col - hv_STD_Col;

			//	//Mark_A1_C_X, Mark_A1_C_Y 分量
			//	hv_Mark_A1_C_X = (hv_STD_Mark_A1_C_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_Mark_A1_C_V_Row * (hv_Img_Rotate_Angle.TupleSin()
			//		));
			//	hv_Mark_A1_C_Y = (hv_STD_Mark_A1_C_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_Mark_A1_C_V_Col * (hv_Img_Rotate_Angle.TupleSin()
			//		));

			//	//目前圖形 Mark_A1_C_ 位置
			//	hv_Mark_A1_C_Pos_Row = (hv_STD_Row + hv_Mark_A1_C_Y) + hv_OffsetRow;
			//	hv_Mark_A1_C_Pos_Col = (hv_STD_Col + hv_Mark_A1_C_X) + hv_OffsetCol;



			//	//STD 向量 STD_Mark_A1_Start_
			//	hv_STD_Mark_A1_Start_V_Row = hv_STD_Mark_A1_Start_Row - hv_STD_Row;
			//	hv_STD_Mark_A1_Start_V_Col = hv_STD_Mark_A1_Start_Col - hv_STD_Col;

			//	//Mark_A1_Start_X, Mark_A1_Start_Y 分量
			//	hv_Mark_A1_Start_X = (hv_STD_Mark_A1_Start_V_Col * (hv_Img_Rotate_Angle.TupleCos()
			//		)) + (hv_STD_Mark_A1_Start_V_Row * (hv_Img_Rotate_Angle.TupleSin()));
			//	hv_Mark_A1_Start_Y = (hv_STD_Mark_A1_Start_V_Row * (hv_Img_Rotate_Angle.TupleCos()
			//		)) - (hv_STD_Mark_A1_Start_V_Col * (hv_Img_Rotate_Angle.TupleSin()));

			//	//目前圖形 Mark_A1_Start_ 位置
			//	hv_Mark_A1_Start_Pos_Row = (hv_STD_Row + hv_Mark_A1_Start_Y) + hv_OffsetRow;
			//	hv_Mark_A1_Start_Pos_Col = (hv_STD_Col + hv_Mark_A1_Start_X) + hv_OffsetCol;

			//	//STD 向量 STD_Mark_A1_End_
			//	hv_STD_Mark_A1_End_V_Row = hv_STD_Mark_A1_End_Row - hv_STD_Row;
			//	hv_STD_Mark_A1_End_V_Col = hv_STD_Mark_A1_End_Col - hv_STD_Col;

			//	//Mark_A1_End_X, Mark_A1_End_Y 分量
			//	hv_Mark_A1_End_X = (hv_STD_Mark_A1_End_V_Col * (hv_Img_Rotate_Angle.TupleCos()
			//		)) + (hv_STD_Mark_A1_End_V_Row * (hv_Img_Rotate_Angle.TupleSin()));
			//	hv_Mark_A1_End_Y = (hv_STD_Mark_A1_End_V_Row * (hv_Img_Rotate_Angle.TupleCos()
			//		)) - (hv_STD_Mark_A1_End_V_Col * (hv_Img_Rotate_Angle.TupleSin()));

			//	//目前圖形 Mark_A1_End_ 位置
			//	hv_Mark_A1_End_Pos_Row = (hv_STD_Row + hv_Mark_A1_End_Y) + hv_OffsetRow;
			//	hv_Mark_A1_End_Pos_Col = (hv_STD_Col + hv_Mark_A1_End_X) + hv_OffsetCol;

			//	//HOperatorSet.SetTposition(hv_WindowHandle, hv_Mark_A1_C_Pos_Row - 25, hv_Mark_A1_C_Pos_Col);
			//	//HOperatorSet.WriteString(hv_WindowHandle, "A1");
			//	//HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A1_C_Pos_Row, hv_Mark_A1_C_Pos_Col - 50,
			//	//	hv_Mark_A1_Start_Pos_Row, hv_Mark_A1_Start_Pos_Col, 5);
			//	//HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A1_C_Pos_Row, hv_Mark_A1_C_Pos_Col + 50,
			//	//	hv_Mark_A1_End_Pos_Row, hv_Mark_A1_End_Pos_Col, 5);

			//}
			//hv_MeasureReasult = hv_A1Dist.Clone();
			//*****A1 End
			//ho_Image.Dispose();
			//ho_AllModelContours.Dispose();
			ho_A1Rectangle.Dispose();
			ho_A1_ROI.Dispose();
			ho_A1Edges.Dispose();
			ho_A1ContoursSplit.Dispose();
			ho_SelectedA1Contours.Dispose();
			ho_TopLineContour.Dispose();
			ho_B_Rectangle.Dispose();
			ho_B__ROI.Dispose();
			ho_B_Edges.Dispose();
			ho_B_ContoursSplit.Dispose();
			ho_SelectedB_ContoursSplit.Dispose();
			ho_B_LeftLine_Contour.Dispose();
			ho_Cross.Dispose();
			ho_A1_RRectangle.Dispose();
			ho_A1_R_ROI.Dispose();
			ho_A1_REdges.Dispose();
			ho_A1_RContoursSplit.Dispose();
			ho_SelectedA1_RContoursSplit.Dispose();
			ho_B_RightLine_Contour.Dispose();
			ho_CrossQ.Dispose();


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
