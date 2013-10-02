using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R3 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion

			// Local iconic variables 

			HObject ho_R3_Circle = null;
			HObject ho_R3_ROI_Image = null, ho_R3_Region = null, ho_R3_ImageReduced = null;
			HObject ho_R3_Edges = null, ho_R3_ContoursSplit = null, ho_R3_SingleSegment = null;
			HObject ho_R3_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_R3_R;
			HTuple hv_STD_R3_Row = new HTuple(), hv_STD_R3_Col = new HTuple();
			HTuple hv_STD_R3_V_Row = new HTuple(), hv_STD_R3_V_Col = new HTuple();
			HTuple hv_R3_X = new HTuple(), hv_R3_Y = new HTuple(), hv_R3_Pos_Row = new HTuple();
			HTuple hv_R3_Pos_Col = new HTuple(), hv_alpha = new HTuple();
			HTuple hv_R3_low = new HTuple(), hv_R3_high = new HTuple();
			HTuple hv_R3_NumSegments = new HTuple(), hv_NumCircles = new HTuple();
			HTuple hv_Num_Circle_Point = new HTuple(), hv_R3 = new HTuple();
			HTuple hv_i = new HTuple(), hv_Attrib = new HTuple(), hv_R3_Row = new HTuple();
			HTuple hv_R3_Column = new HTuple(), hv_R3_Radius = new HTuple();
			HTuple hv_R3_StartPhi = new HTuple(), hv_R3_EndPhi = new HTuple();
			HTuple hv_R3_PointOrder = new HTuple(), hv_R3_MinDist = new HTuple();
			HTuple hv_R3_MaxDist = new HTuple(), hv_R3_AvgDist = new HTuple();
			HTuple hv_R3_SigmaDist = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_R3_Circle);
			HOperatorSet.GenEmptyObj(out ho_R3_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R3_Region);
			HOperatorSet.GenEmptyObj(out ho_R3_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R3_Edges);
			HOperatorSet.GenEmptyObj(out ho_R3_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R3_SingleSegment);
			HOperatorSet.GenEmptyObj(out ho_R3_ContEllipse);

			//Measure: SDMS_R3
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

			//****Display
			if (HDevWindowStack.IsOpen())
			{
				HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
			}
			if (HDevWindowStack.IsOpen())
			{
				HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
			}
			//*****R3
			hv_R3_R = 29;

			//STD R3_ 位置
			hv_STD_R3_Row = 252;
			hv_STD_R3_Col = 1381;

			//STD 向量 STD_R3_
			hv_STD_R3_V_Row = hv_STD_R3_Row - hv_STD_Row;
			hv_STD_R3_V_Col = hv_STD_R3_Col - hv_STD_Col;

			//R3_X, R3_Y 分量
			hv_R3_X = (hv_STD_R3_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R3_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R3_Y = (hv_STD_R3_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R3_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R3_ 位置
			hv_R3_Pos_Row = (hv_STD_Row + hv_R3_Y) + hv_OffsetRow;
			hv_R3_Pos_Col = (hv_STD_Col + hv_R3_X) + hv_OffsetCol;

			//** 開始計算
			ho_R3_Circle.Dispose();
			HOperatorSet.GenCircle(out ho_R3_Circle, hv_R3_Pos_Row, hv_R3_Pos_Col, hv_R3_R);
			ho_R3_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_R3_Circle, out ho_R3_ROI_Image);
			ho_R3_Region.Dispose();
			HOperatorSet.FastThreshold(ho_R3_ROI_Image, out ho_R3_Region, 100, 255, 15);
			ho_R3_ImageReduced.Dispose();
			HOperatorSet.ReduceDomain(ho_R3_ROI_Image, ho_R3_Region, out ho_R3_ImageReduced
				);
			//stop ()
			//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
			hv_alpha = 0.9;
			hv_R3_low = 20;
			hv_R3_high = 60;
			ho_R3_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_R3_ImageReduced, out ho_R3_Edges, "sobel_fast",
				hv_alpha, hv_R3_low, hv_R3_high);
			//stop ()
			//*所有的數值越小，表示容錯範圍大，反之亦然
			ho_R3_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_R3_Edges, out ho_R3_ContoursSplit, "lines_circles",
				17, 4, 2);
			//Display the results
			//===========================================================
			HOperatorSet.CountObj(ho_R3_ContoursSplit, out hv_R3_NumSegments);
			hv_NumCircles = 0;
			hv_Num_Circle_Point = 0;
			hv_R3 = 999;
			for (hv_i = 1; hv_i.Continue(hv_R3_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
			{
				ho_R3_SingleSegment.Dispose();
				HOperatorSet.SelectObj(ho_R3_ContoursSplit, out ho_R3_SingleSegment, hv_i);
				HOperatorSet.GetContourGlobalAttribXld(ho_R3_SingleSegment, "cont_approx",
					out hv_Attrib);
				if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
				{
					HOperatorSet.FitCircleContourXld(ho_R3_SingleSegment, "atukey", -1, 2,
						hv_Num_Circle_Point, 5, 2, out hv_R3_Row, out hv_R3_Column, out hv_R3_Radius,
						out hv_R3_StartPhi, out hv_R3_EndPhi, out hv_R3_PointOrder);

					hv_NumCircles = hv_NumCircles + 1;
					if ((int)(new HTuple(hv_R3.TupleGreater(hv_R3_Radius))) != 0)
					{
						hv_R3 = hv_R3_Radius.Clone();
						mResult = new CircleResult()
						{
							Row = new HTuple(hv_R3_Row),
							Col = new HTuple(hv_R3_Column),
							Radius = new HTuple(hv_R3_Radius),
							StartPhi = new HTuple(hv_R3_StartPhi),
							EndPhi = new HTuple(hv_R3_EndPhi),
							PointOrder = new HTuple(hv_R3_PointOrder),
						};
						//HOperatorSet.SetTposition(hv_WindowHandle, (hv_R3_Pos_Row - hv_msgOffsetY) + 60,
						//	(hv_R3_Pos_Col + hv_msgOffsetX) - 60);
						//HOperatorSet.WriteString(hv_WindowHandle, "R3");
						//stop ()
					}
				}
			}

			//*****R3 End
			ho_R3_Circle.Dispose();
			ho_R3_ROI_Image.Dispose();
			ho_R3_Region.Dispose();
			ho_R3_ImageReduced.Dispose();
			ho_R3_Edges.Dispose();
			ho_R3_ContoursSplit.Dispose();
			ho_R3_SingleSegment.Dispose();
			ho_R3_ContEllipse.Dispose();


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
