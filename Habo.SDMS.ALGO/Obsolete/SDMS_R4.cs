using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R4 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_R4_Circle = null;
			HObject ho_R4_ROI_Image = null, ho_R4_Region = null, ho_R4_ImageReduced = null;
			HObject ho_R4_Edges = null, ho_R4_ContoursSplit = null, ho_R4_SingleSegment = null;
			HObject ho_R4_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_R4_R;
			HTuple hv_STD_R4_Row = new HTuple(), hv_STD_R4_Col = new HTuple();
			HTuple hv_STD_R4_V_Row = new HTuple(), hv_STD_R4_V_Col = new HTuple();
			HTuple hv_R4_X = new HTuple(), hv_R4_Y = new HTuple(), hv_R4_Pos_Row = new HTuple();
			HTuple hv_R4_Pos_Col = new HTuple(), hv_alpha = new HTuple();
			HTuple hv_R4_low = new HTuple(), hv_R4_high = new HTuple();
			HTuple hv_R4_NumSegments = new HTuple(), hv_NumCircles = new HTuple();
			HTuple hv_Num_Circle_Point = new HTuple(), hv_R4 = new HTuple();
			HTuple hv_i = new HTuple(), hv_Attrib = new HTuple(), hv_R4_Row = new HTuple();
			HTuple hv_R4_Column = new HTuple(), hv_R4_Radius = new HTuple();
			HTuple hv_R4_StartPhi = new HTuple(), hv_R4_EndPhi = new HTuple();
			HTuple hv_R4_PointOrder = new HTuple(), hv_R4_MinDist = new HTuple();
			HTuple hv_R4_MaxDist = new HTuple(), hv_R4_AvgDist = new HTuple();
			HTuple hv_R4_SigmaDist = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_R4_Circle);
			HOperatorSet.GenEmptyObj(out ho_R4_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R4_Region);
			HOperatorSet.GenEmptyObj(out ho_R4_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R4_Edges);
			HOperatorSet.GenEmptyObj(out ho_R4_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R4_SingleSegment);
			HOperatorSet.GenEmptyObj(out ho_R4_ContEllipse);

			//Measure: SDMS_R4
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
			//*****R4
			hv_R4_R = 29;

			//STD R4 位置
			hv_STD_R4_Row = 252;
			hv_STD_R4_Col = 728;

			//STD 向量 STD_R4
			hv_STD_R4_V_Row = hv_STD_R4_Row - hv_STD_Row;
			hv_STD_R4_V_Col = hv_STD_R4_Col - hv_STD_Col;

			//R4_X, R4_Y 分量
			hv_R4_X = (hv_STD_R4_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R4_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R4_Y = (hv_STD_R4_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R4_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R4 位置
			hv_R4_Pos_Row = (hv_STD_Row + hv_R4_Y) + hv_OffsetRow;
			hv_R4_Pos_Col = (hv_STD_Col + hv_R4_X) + hv_OffsetCol;

			//** 開始計算
			ho_R4_Circle.Dispose();
			HOperatorSet.GenCircle(out ho_R4_Circle, hv_R4_Pos_Row, hv_R4_Pos_Col, hv_R4_R);
			ho_R4_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_R4_Circle, out ho_R4_ROI_Image);
			ho_R4_Region.Dispose();
			HOperatorSet.FastThreshold(ho_R4_ROI_Image, out ho_R4_Region, 100, 255, 15);
			ho_R4_ImageReduced.Dispose();
			HOperatorSet.ReduceDomain(ho_R4_ROI_Image, ho_R4_Region, out ho_R4_ImageReduced
				);
			//stop ()
			//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
			hv_alpha = 0.9;
			hv_R4_low = 20;
			hv_R4_high = 60;
			ho_R4_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_R4_ImageReduced, out ho_R4_Edges, "sobel_fast",
				hv_alpha, hv_R4_low, hv_R4_high);
			//stop ()
			//*所有的數值越小，表示容錯範圍大，反之亦然
			ho_R4_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_R4_Edges, out ho_R4_ContoursSplit, "lines_circles",
				17, 4, 2);
			//Display the results
			//===========================================================
			HOperatorSet.CountObj(ho_R4_ContoursSplit, out hv_R4_NumSegments);
			hv_NumCircles = 0;
			hv_Num_Circle_Point = 0;
			hv_R4 = 999;
			for (hv_i = 1; hv_i.Continue(hv_R4_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
			{
				ho_R4_SingleSegment.Dispose();
				HOperatorSet.SelectObj(ho_R4_ContoursSplit, out ho_R4_SingleSegment, hv_i);
				HOperatorSet.GetContourGlobalAttribXld(ho_R4_SingleSegment, "cont_approx",
					out hv_Attrib);
				if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
				{
					HOperatorSet.FitCircleContourXld(ho_R4_SingleSegment, "atukey", -1, 2,
						hv_Num_Circle_Point, 5, 2, out hv_R4_Row, out hv_R4_Column, out hv_R4_Radius,
						out hv_R4_StartPhi, out hv_R4_EndPhi, out hv_R4_PointOrder);
					ho_R4_ContEllipse.Dispose();
					HOperatorSet.GenEllipseContourXld(out ho_R4_ContEllipse, hv_R4_Row, hv_R4_Column,
						0, hv_R4_Radius, hv_R4_Radius, 0, (new HTuple(360)).TupleRad(), "positive",
						1.0);
					if (HDevWindowStack.IsOpen())
					{
						HOperatorSet.DispObj(ho_R4_ContEllipse, HDevWindowStack.GetActive());
					}
					HOperatorSet.DistEllipseContourXld(ho_R4_SingleSegment, "algebraic", -1,
						0, hv_R4_Row, hv_R4_Column, 0, hv_R4_Radius, hv_R4_Radius, out hv_R4_MinDist,
						out hv_R4_MaxDist, out hv_R4_AvgDist, out hv_R4_SigmaDist);
					hv_NumCircles = hv_NumCircles + 1;
					if ((int)(new HTuple(hv_R4.TupleGreater(hv_R4_Radius))) != 0)
					{
						hv_R4 = hv_R4_Radius.Clone();
						mResult = new CircleResult()
						{
							Row = new HTuple(hv_R4_Row),
							Col = new HTuple(hv_R4_Column),
							Radius = new HTuple(hv_R4_Radius),
							StartPhi = new HTuple(hv_R4_StartPhi),
							EndPhi = new HTuple(hv_R4_EndPhi),
							PointOrder = new HTuple(hv_R4_PointOrder),
						};

					}
					//stop ()
				}
			}

			//*****R4 End
			ho_R4_Circle.Dispose();
			ho_R4_ROI_Image.Dispose();
			ho_R4_Region.Dispose();
			ho_R4_ImageReduced.Dispose();
			ho_R4_Edges.Dispose();
			ho_R4_ContoursSplit.Dispose();
			ho_R4_SingleSegment.Dispose();
			ho_R4_ContEllipse.Dispose();




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
