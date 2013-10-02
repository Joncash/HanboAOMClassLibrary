using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_B : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			DistanceResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_ROI_Rec = null;
			HObject ho_ROI_Image = null, ho_ROI_Edges = null, ho_ROI_ContoursSplit = null;
			HObject ho_ROI_SortedContours = null, ho_FirstLine = null, ho_SecondLine = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_B_Row, hv_STD_B_Col;
			HTuple hv_STD_B_V_Row, hv_STD_B_V_Col, hv_B_X, hv_B_Y;
			HTuple hv_B_Pos_Row, hv_B_Pos_Col, hv_ROI_W = new HTuple();
			HTuple hv_ROI_H = new HTuple(), hv_low = new HTuple(), hv_high = new HTuple();
			HTuple hv_algo = new HTuple(), hv_Number = new HTuple(), hv_FirstRowBegin = new HTuple();
			HTuple hv_FirstColBegin = new HTuple(), hv_FirstRowEnd = new HTuple();
			HTuple hv_FirstColEnd = new HTuple(), hv_FirstNr = new HTuple();
			HTuple hv_FirstNc = new HTuple(), hv_FirstDist = new HTuple();
			HTuple hv_SecondRowBegin = new HTuple(), hv_SecondColBegin = new HTuple();
			HTuple hv_SecondRowEnd = new HTuple(), hv_SecondColEnd = new HTuple();
			HTuple hv_SecondNr = new HTuple(), hv_SecondNc = new HTuple();
			HTuple hv_SecondDist = new HTuple(), hv_minDist = new HTuple();
			HTuple hv_maxDist = new HTuple(), hv_STD_Mark_B_C_Row = new HTuple();
			HTuple hv_STD_Mark_B_C_Col = new HTuple(), hv_STD_Mark_B_Start_Row = new HTuple();
			HTuple hv_STD_Mark_B_Start_Col = new HTuple(), hv_STD_Mark_B_End_Row = new HTuple();
			HTuple hv_STD_Mark_B_End_Col = new HTuple(), hv_STD_Mark_B_C_V_Row = new HTuple();
			HTuple hv_STD_Mark_B_C_V_Col = new HTuple(), hv_Mark_B_C_X = new HTuple();
			HTuple hv_Mark_B_C_Y = new HTuple(), hv_Mark_B_C_Pos_Row = new HTuple();
			HTuple hv_Mark_B_C_Pos_Col = new HTuple(), hv_STD_Mark_B_Start_V_Row = new HTuple();
			HTuple hv_STD_Mark_B_Start_V_Col = new HTuple(), hv_Mark_B_Start_X = new HTuple();
			HTuple hv_Mark_B_Start_Y = new HTuple(), hv_Mark_B_Start_Pos_Row = new HTuple();
			HTuple hv_Mark_B_Start_Pos_Col = new HTuple(), hv_STD_Mark_B_End_V_Row = new HTuple();
			HTuple hv_STD_Mark_B_End_V_Col = new HTuple(), hv_Mark_B_End_X = new HTuple();
			HTuple hv_Mark_B_End_Y = new HTuple(), hv_Mark_B_End_Pos_Row = new HTuple();
			HTuple hv_Mark_B_End_Pos_Col = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_ROI_Rec);
			HOperatorSet.GenEmptyObj(out ho_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_ROI_Edges);
			HOperatorSet.GenEmptyObj(out ho_ROI_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_ROI_SortedContours);
			HOperatorSet.GenEmptyObj(out ho_FirstLine);
			HOperatorSet.GenEmptyObj(out ho_SecondLine);

			//Measure: SDMS_B
			//Author: John Hsieh
			//Date: 2012
			//ho_Image.Dispose();
			//HOperatorSet.ReadImage(out ho_Image, "Images/STD.bmp");
			//dev_open_window_fit_image(ho_Image, 0, 0, -1, -1, out hv_WindowHandle);
			//// dev_update_off(...); only in hdevelop
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

			//****Display
			if (HDevWindowStack.IsOpen())
			{
				HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
			}
			if (HDevWindowStack.IsOpen())
			{
				HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
			}
			//*****B
			//STD B_ 位置
			hv_STD_B_Row = 743;
			hv_STD_B_Col = 1118;

			//STD 向量 STD_B_
			hv_STD_B_V_Row = hv_STD_B_Row - hv_STD_Row;
			hv_STD_B_V_Col = hv_STD_B_Col - hv_STD_Col;


			//B_X, B_Y 分量
			hv_B_X = (hv_STD_B_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_B_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_B_Y = (hv_STD_B_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_B_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 B_ 位置
			hv_B_Pos_Row = (hv_STD_Row + hv_B_Y) + hv_OffsetRow;
			hv_B_Pos_Col = (hv_STD_Col + hv_B_X) + hv_OffsetCol;



			hv_ROI_W = 150;
			hv_ROI_H = 600;
			ho_ROI_Rec.Dispose();
			HOperatorSet.GenRectangle2(out ho_ROI_Rec, hv_B_Pos_Row, hv_B_Pos_Col, hv_Img_Rotate_Angle,
				hv_ROI_W, hv_ROI_H);
			//stop ()
			ho_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_ROI_Rec, out ho_ROI_Image);
			//corner detect
			hv_low = 130;
			hv_high = 180;
			ho_ROI_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_ROI_Image, out ho_ROI_Edges, "lanser2", 0.3, hv_low,
				hv_high);
			//stop ()
			hv_algo = "lines";
			ho_ROI_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_ROI_Edges, out ho_ROI_ContoursSplit, hv_algo,
				9, 4, 2);
			ho_ROI_SortedContours.Dispose();
			HOperatorSet.SortContoursXld(ho_ROI_ContoursSplit, out ho_ROI_SortedContours,
				"upper_left", "true", "row");
			HOperatorSet.CountObj(ho_ROI_SortedContours, out hv_Number);
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (ROI_SortedContours)
			}
			//兩線段
			if ((int)(new HTuple(hv_Number.TupleEqual(2))) != 0)
			{
				ho_FirstLine.Dispose();
				HOperatorSet.SelectObj(ho_ROI_SortedContours, out ho_FirstLine, 1);
				ho_SecondLine.Dispose();
				HOperatorSet.SelectObj(ho_ROI_SortedContours, out ho_SecondLine, 2);
				HOperatorSet.FitLineContourXld(ho_FirstLine, "tukey", -1, 0, 5, 2, out hv_FirstRowBegin,
					out hv_FirstColBegin, out hv_FirstRowEnd, out hv_FirstColEnd, out hv_FirstNr,
					out hv_FirstNc, out hv_FirstDist);
				HOperatorSet.FitLineContourXld(ho_SecondLine, "tukey", -1, 0, 5, 2, out hv_SecondRowBegin,
					out hv_SecondColBegin, out hv_SecondRowEnd, out hv_SecondColEnd, out hv_SecondNr,
					out hv_SecondNc, out hv_SecondDist);

				//line to line
				//取兩線段的最小距離
				HOperatorSet.DistanceSs(hv_FirstRowBegin, hv_FirstColBegin, hv_FirstRowEnd,
					hv_FirstColEnd, hv_SecondRowBegin, hv_SecondColBegin, hv_SecondRowEnd,
					hv_SecondColEnd, out hv_minDist, out hv_maxDist);

				mResult = new DistanceResult()
				{
					FirstRowBegin = new HTuple(hv_FirstRowBegin),
					FirstColBegin = new HTuple(hv_FirstColBegin),
					FirstRowEnd = new HTuple(hv_FirstRowEnd),
					FirstColEnd = new HTuple(hv_FirstColEnd),
					SecondRowBegin = new HTuple(hv_SecondRowBegin),
					SecondColBegin = new HTuple(hv_SecondColBegin),
					SecondRowEnd = new HTuple(hv_SecondRowEnd),
					SecondColEnd = new HTuple(hv_SecondColEnd),
					Angle = new HTuple(this.hv_AllModelAngle),
					Distance = new HTuple(hv_minDist),
					Direction = LineDirection.Vertical,
				};

			}

			//*****B End

			ho_ROI_Rec.Dispose();
			ho_ROI_Image.Dispose();
			ho_ROI_Edges.Dispose();
			ho_ROI_ContoursSplit.Dispose();
			ho_ROI_SortedContours.Dispose();
			ho_FirstLine.Dispose();
			ho_SecondLine.Dispose();




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
