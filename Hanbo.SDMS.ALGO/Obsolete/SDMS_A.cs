using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_A : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			DistanceResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_A_1_1_Rectangle = null;
			HObject ho_A_1_1_Image = null, ho_A_1_1_Edges = null, ho_A_1_1_Contours = null;
			HObject ho_A_1_1_SelectedLines = null, ho_A_1_1_Line = null;
			HObject ho_A_1_2_Rectangle = null, ho_A_1_2_Image = null, ho_A_1_2_Edges = null;
			HObject ho_A_1_2_Contours = null, ho_A_1_2_SelectedLines = null;
			HObject ho_A_1_2_Line = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_A_1_1_Row, hv_STD_A_1_1_Col;
			HTuple hv_STD_A_1_1_V_Row, hv_STD_A_1_1_V_Col, hv_A_1_1_X;
			HTuple hv_A_1_1_Y, hv_A_1_1_Pos_Row, hv_A_1_1_Pos_Col;
			HTuple hv_Corner_W = new HTuple(), hv_Corner_H = new HTuple();
			HTuple hv_low = new HTuple(), hv_high = new HTuple(), hv_algo = new HTuple();
			HTuple hv_A_1_1Number = new HTuple(), hv_A_1_1_LineRowBegin = new HTuple();
			HTuple hv_A_1_1_LineColBegin = new HTuple(), hv_A_1_1_LineRowEnd = new HTuple();
			HTuple hv_A_1_1_LineColEnd = new HTuple(), hv_A_1_1_LineNr = new HTuple();
			HTuple hv_A_1_1_LineNc = new HTuple(), hv_A_1_1_LineDist = new HTuple();
			HTuple hv_STD_A_1_2_Row, hv_STD_A_1_2_Col, hv_STD_A_1_2_V_Row;
			HTuple hv_STD_A_1_2_V_Col, hv_A_1_2_X, hv_A_1_2_Y, hv_A_1_2_Pos_Row;
			HTuple hv_A_1_2_Pos_Col, hv_A_1_2Number = new HTuple(), hv_A_1_2_LineRowBegin = new HTuple();
			HTuple hv_A_1_2_LineColBegin = new HTuple(), hv_A_1_2_LineRowEnd = new HTuple();
			HTuple hv_A_1_2_LineColEnd = new HTuple(), hv_A_1_2_LineNr = new HTuple();
			HTuple hv_A_1_2_LineNc = new HTuple(), hv_A_1_2_LineDist = new HTuple();
			HTuple hv_DistanceMin, hv_DistanceMax;
			HTuple hv_MeasureReasult;

			// Initialize local and output iconic variables 
			//HOperatorSet.GenEmptyObj(out ho_Image);
			//HOperatorSet.GenEmptyObj(out ho_AllModelContours);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_Rectangle);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_Image);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_Edges);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_Contours);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_SelectedLines);
			HOperatorSet.GenEmptyObj(out ho_A_1_1_Line);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_Rectangle);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_Image);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_Edges);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_Contours);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_SelectedLines);
			HOperatorSet.GenEmptyObj(out ho_A_1_2_Line);

			//Measure: SDMS_A
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


			//*****A
			//****A_1_1
			//STD A_1_1_ 位置
			HTuple newR, newC;
			hv_STD_A_1_1_Row = 1110;
			hv_STD_A_1_1_Col = 630;
			ReLocater(hv_STD_Row, hv_STD_Col, hv_Img_Rotate_Angle, hv_OffsetRow, hv_OffsetCol,
				hv_STD_A_1_1_Row, hv_STD_A_1_1_Col, out newR, out newC);

			//STD 向量 STD_A_1_1_
			hv_STD_A_1_1_V_Row = hv_STD_A_1_1_Row - hv_STD_Row;
			hv_STD_A_1_1_V_Col = hv_STD_A_1_1_Col - hv_STD_Col;


			//A_1_1_X, A_1_1_Y 分量
			hv_A_1_1_X = (hv_STD_A_1_1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_A_1_1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_A_1_1_Y = (hv_STD_A_1_1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_A_1_1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 A_1_1_ 位置
			hv_A_1_1_Pos_Row = (hv_STD_Row + hv_A_1_1_Y) + hv_OffsetRow;
			hv_A_1_1_Pos_Col = (hv_STD_Col + hv_A_1_1_X) + hv_OffsetCol;

			


			hv_Corner_W = 30;
			hv_Corner_H = 120;
			ho_A_1_1_Rectangle.Dispose();
			HOperatorSet.GenRectangle2(out ho_A_1_1_Rectangle, hv_A_1_1_Pos_Row, hv_A_1_1_Pos_Col,
				hv_Img_Rotate_Angle, hv_Corner_W, hv_Corner_H);
			ho_A_1_1_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A_1_1_Rectangle, out ho_A_1_1_Image);

			//corner detect

			hv_low = 60;
			hv_high = 90;
			ho_A_1_1_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A_1_1_Image, out ho_A_1_1_Edges, "lanser2", 0.3,
				hv_low, hv_high);

			hv_algo = "lines";
			ho_A_1_1_Contours.Dispose();
			HOperatorSet.SegmentContoursXld(ho_A_1_1_Edges, out ho_A_1_1_Contours, hv_algo,
				9, 4, 2);
			//取長線段
			ho_A_1_1_SelectedLines.Dispose();
			HOperatorSet.SelectShapeXld(ho_A_1_1_Contours, out ho_A_1_1_SelectedLines,
				"contlength", "and", 50, 99999);
			HOperatorSet.CountObj(ho_A_1_1_SelectedLines, out hv_A_1_1Number);
			if ((int)(new HTuple(hv_A_1_1Number.TupleGreater(0))) != 0)
			{
				ho_A_1_1_Line.Dispose();
				HOperatorSet.SelectObj(ho_A_1_1_SelectedLines, out ho_A_1_1_Line, 1);
				HOperatorSet.FitLineContourXld(ho_A_1_1_Line, "tukey", -1, 0, 5, 2, out hv_A_1_1_LineRowBegin,
					out hv_A_1_1_LineColBegin, out hv_A_1_1_LineRowEnd, out hv_A_1_1_LineColEnd,
					out hv_A_1_1_LineNr, out hv_A_1_1_LineNc, out hv_A_1_1_LineDist);

				mResult = new DistanceResult()
				{
					FirstRowBegin = new HTuple(hv_A_1_1_LineRowBegin),
					FirstColBegin = new HTuple(hv_A_1_1_LineColBegin),
					FirstRowEnd = new HTuple(hv_A_1_1_LineRowEnd),
					FirstColEnd = new HTuple(hv_A_1_1_LineColEnd),
					Angle = this.hv_AllModelAngle,
				};

			}

			//*********
			//****A_1_2
			//STD A_1_2_ 位置
			hv_STD_A_1_2_Row = 997;
			hv_STD_A_1_2_Col = 1376;

			//STD 向量 STD_A_1_2_
			hv_STD_A_1_2_V_Row = hv_STD_A_1_2_Row - hv_STD_Row;
			hv_STD_A_1_2_V_Col = hv_STD_A_1_2_Col - hv_STD_Col;


			//A_1_2_X, A_1_2_Y 分量
			hv_A_1_2_X = (hv_STD_A_1_2_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_A_1_2_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_A_1_2_Y = (hv_STD_A_1_2_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_A_1_2_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 A_1_2_ 位置
			hv_A_1_2_Pos_Row = (hv_STD_Row + hv_A_1_2_Y) + hv_OffsetRow;
			hv_A_1_2_Pos_Col = (hv_STD_Col + hv_A_1_2_X) + hv_OffsetCol;

			//for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_AllModelScore.TupleLength()
			//	)) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
			//{
			hv_Corner_W = 30;
			hv_Corner_H = 600;
			ho_A_1_2_Rectangle.Dispose();
			HOperatorSet.GenRectangle2(out ho_A_1_2_Rectangle, hv_A_1_2_Pos_Row, hv_A_1_2_Pos_Col,
				hv_Img_Rotate_Angle, hv_Corner_W, hv_Corner_H);
			//stop ()
			ho_A_1_2_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_A_1_2_Rectangle, out ho_A_1_2_Image);

			//corner detect

			hv_low = 60;
			hv_high = 90;
			ho_A_1_2_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_A_1_2_Image, out ho_A_1_2_Edges, "lanser2", 0.3,
				hv_low, hv_high);

			hv_algo = "lines";
			ho_A_1_2_Contours.Dispose();
			HOperatorSet.SegmentContoursXld(ho_A_1_2_Edges, out ho_A_1_2_Contours, hv_algo,
				9, 4, 2);
			//取長線段
			ho_A_1_2_SelectedLines.Dispose();
			HOperatorSet.SelectShapeXld(ho_A_1_2_Contours, out ho_A_1_2_SelectedLines,
				"contlength", "and", 50, 99999);
			HOperatorSet.CountObj(ho_A_1_2_SelectedLines, out hv_A_1_2Number);
			if ((int)(new HTuple(hv_A_1_2Number.TupleGreater(0))) != 0)
			{
				ho_A_1_2_Line.Dispose();
				HOperatorSet.SelectObj(ho_A_1_2_SelectedLines, out ho_A_1_2_Line, 1);
				HOperatorSet.FitLineContourXld(ho_A_1_2_Line, "tukey", -1, 0, 5, 2, out hv_A_1_2_LineRowBegin,
					out hv_A_1_2_LineColBegin, out hv_A_1_2_LineRowEnd, out hv_A_1_2_LineColEnd,
					out hv_A_1_2_LineNr, out hv_A_1_2_LineNc, out hv_A_1_2_LineDist);

				mResult.SecondColBegin = new HTuple(hv_A_1_2_LineColBegin);
				mResult.SecondColEnd = new HTuple(hv_A_1_2_LineColEnd);
				mResult.SecondRowBegin = new HTuple(hv_A_1_2_LineRowBegin);
				mResult.SecondRowEnd = new HTuple(hv_A_1_2_LineRowEnd);
			}

			//*********

			HOperatorSet.DistanceCc(ho_A_1_1_Line, ho_A_1_2_Line, "point_to_segment", out hv_DistanceMin,
				out hv_DistanceMax);

			mResult.Distance = new HTuple(hv_DistanceMin);

			/*
			//****Mark_A_C
			//STD Mark_A_C_ 位置
			hv_STD_Mark_A_C_Row = 1400;
			hv_STD_Mark_A_C_Col = 1000;

			//STD 向量 STD_Mark_A_C_
			hv_STD_Mark_A_C_V_Row = hv_STD_Mark_A_C_Row - hv_STD_Row;
			hv_STD_Mark_A_C_V_Col = hv_STD_Mark_A_C_Col - hv_STD_Col;


			//Mark_A_C_X, Mark_A_C_Y 分量
			hv_Mark_A_C_X = (hv_STD_Mark_A_C_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_Mark_A_C_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_Mark_A_C_Y = (hv_STD_Mark_A_C_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_Mark_A_C_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 Mark_A_C_ 位置
			hv_Mark_A_C_Pos_Row = (hv_STD_Row + hv_Mark_A_C_Y) + hv_OffsetRow;
			hv_Mark_A_C_Pos_Col = (hv_STD_Col + hv_Mark_A_C_X) + hv_OffsetCol;

			//****Mark_A_Start
			//STD Mark_A_Start_ 位置
			hv_STD_Mark_A_Start_Row = 1400;
			hv_STD_Mark_A_Start_Col = 618;

			//STD 向量 STD_Mark_A_Start_
			hv_STD_Mark_A_Start_V_Row = hv_STD_Mark_A_Start_Row - hv_STD_Row;
			hv_STD_Mark_A_Start_V_Col = hv_STD_Mark_A_Start_Col - hv_STD_Col;


			//Mark_A_Start_X, Mark_A_Start_Y 分量
			hv_Mark_A_Start_X = (hv_STD_Mark_A_Start_V_Col * (hv_Img_Rotate_Angle.TupleCos()
				)) + (hv_STD_Mark_A_Start_V_Row * (hv_Img_Rotate_Angle.TupleSin()));
			hv_Mark_A_Start_Y = (hv_STD_Mark_A_Start_V_Row * (hv_Img_Rotate_Angle.TupleCos()
				)) - (hv_STD_Mark_A_Start_V_Col * (hv_Img_Rotate_Angle.TupleSin()));


			//目前圖形 Mark_A_Start_ 位置
			hv_Mark_A_Start_Pos_Row = (hv_STD_Row + hv_Mark_A_Start_Y) + hv_OffsetRow;
			hv_Mark_A_Start_Pos_Col = (hv_STD_Col + hv_Mark_A_Start_X) + hv_OffsetCol;

			//****Mark_A_End
			//STD Mark_A_End_ 位置
			hv_STD_Mark_A_End_Row = 1400;
			hv_STD_Mark_A_End_Col = 1415;

			//STD 向量 STD_Mark_A_End_
			hv_STD_Mark_A_End_V_Row = hv_STD_Mark_A_End_Row - hv_STD_Row;
			hv_STD_Mark_A_End_V_Col = hv_STD_Mark_A_End_Col - hv_STD_Col;


			//Mark_A_End_X, Mark_A_End_Y 分量
			hv_Mark_A_End_X = (hv_STD_Mark_A_End_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_Mark_A_End_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_Mark_A_End_Y = (hv_STD_Mark_A_End_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_Mark_A_End_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 Mark_A_End_ 位置
			hv_Mark_A_End_Pos_Row = (hv_STD_Row + hv_Mark_A_End_Y) + hv_OffsetRow;
			hv_Mark_A_End_Pos_Col = (hv_STD_Col + hv_Mark_A_End_X) + hv_OffsetCol;

			HOperatorSet.SetTposition(hv_WindowHandle, hv_Mark_A_C_Pos_Row - 25, hv_Mark_A_C_Pos_Col);
			HOperatorSet.WriteString(hv_WindowHandle, "A");
			HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A_C_Pos_Row, hv_Mark_A_C_Pos_Col - 50,
				hv_Mark_A_Start_Pos_Row, hv_Mark_A_Start_Pos_Col, 5);
			HOperatorSet.DispArrow(hv_WindowHandle, hv_Mark_A_C_Pos_Row, hv_Mark_A_C_Pos_Col + 50,
				hv_Mark_A_End_Pos_Row, hv_Mark_A_End_Pos_Col, 5);
			 */

			//*********
			hv_MeasureReasult = hv_DistanceMin.Clone();
			//*****A End

			//ho_Image.Dispose();
			//ho_AllModelContours.Dispose();
			ho_A_1_1_Rectangle.Dispose();
			ho_A_1_1_Image.Dispose();
			ho_A_1_1_Edges.Dispose();
			ho_A_1_1_Contours.Dispose();
			ho_A_1_1_SelectedLines.Dispose();
			ho_A_1_1_Line.Dispose();
			ho_A_1_2_Rectangle.Dispose();
			ho_A_1_2_Image.Dispose();
			ho_A_1_2_Edges.Dispose();
			ho_A_1_2_Contours.Dispose();
			ho_A_1_2_SelectedLines.Dispose();
			ho_A_1_2_Line.Dispose();

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
		private void ReLocater(HTuple stdRow, HTuple stdCol, HTuple curModelAngle, HTuple offsetRow, HTuple offsetCol
									, HTuple roiRow, HTuple roiCol
									, out HTuple curROI_Row, out HTuple curROI_Col)
		{
			//roiRow = 1110;
			//roiCol = 630;

			//STD 向量 STD_A_1_1_
			HTuple veterRow = roiRow - stdRow;
			HTuple vertCol = roiCol - stdCol;

			HTuple roiVeterCol = (vertCol * (curModelAngle.TupleCos())) + (veterRow * (curModelAngle.TupleSin()));
			HTuple roiVectorRow = (veterRow * (curModelAngle.TupleCos())) - (vertCol * (curModelAngle.TupleSin()));


			//目前圖形 A_1_1_ 位置
			curROI_Row = (stdRow + roiVectorRow) + offsetRow;
			curROI_Col = (stdCol + roiVeterCol) + offsetCol;
		}
	}
}
