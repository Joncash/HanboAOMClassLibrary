using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R2 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion

			// Local iconic variables 

			HObject ho_R2_Circle = null;
			HObject ho_R2_ROI_Image = null, ho_R2_Region = null, ho_R2_ImageReduced = null;
			HObject ho_R2_Edges = null, ho_R2_ContoursSplit = null, ho_R2_SingleSegment = null;
			HObject ho_R2_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_R2_Row, hv_STD_R2_Col;
			HTuple hv_STD_R2_V_Row, hv_STD_R2_V_Col, hv_R2_X, hv_R2_Y;
			HTuple hv_R2_Pos_Row, hv_R2_Pos_Col, hv_R2_R;
			HTuple hv_alpha = new HTuple(), hv_R2_low = new HTuple(), hv_R2_high = new HTuple();
			HTuple hv_R2_NumSegments = new HTuple(), hv_NumCircles = new HTuple();
			HTuple hv_Num_Circle_Point = new HTuple(), hv_R2 = new HTuple();
			HTuple hv_i = new HTuple(), hv_Attrib = new HTuple(), hv_R2_Row = new HTuple();
			HTuple hv_R2_Column = new HTuple(), hv_R2_Radius = new HTuple();
			HTuple hv_R2_StartPhi = new HTuple(), hv_R2_EndPhi = new HTuple();
			HTuple hv_R2_PointOrder = new HTuple(), hv_R2_MinDist = new HTuple();
			HTuple hv_R2_MaxDist = new HTuple(), hv_R2_AvgDist = new HTuple();
			HTuple hv_R2_SigmaDist = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_R2_Circle);
			HOperatorSet.GenEmptyObj(out ho_R2_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R2_Region);
			HOperatorSet.GenEmptyObj(out ho_R2_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R2_Edges);
			HOperatorSet.GenEmptyObj(out ho_R2_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R2_SingleSegment);
			HOperatorSet.GenEmptyObj(out ho_R2_ContEllipse);

			//Measure: SDMS_R2
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
			//****R2
			//STD R2_ 位置
			hv_STD_R2_Row = 689;
			hv_STD_R2_Col = 621;

			//STD 向量 STD_R2_
			hv_STD_R2_V_Row = hv_STD_R2_Row - hv_STD_Row;
			hv_STD_R2_V_Col = hv_STD_R2_Col - hv_STD_Col;


			//R2_X, R2_Y 分量
			hv_R2_X = (hv_STD_R2_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R2_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R2_Y = (hv_STD_R2_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R2_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R2_ 位置
			hv_R2_Pos_Row = (hv_STD_Row + hv_R2_Y) + hv_OffsetRow;
			hv_R2_Pos_Col = (hv_STD_Col + hv_R2_X) + hv_OffsetCol;


			hv_R2_R = 21;

			ho_R2_Circle.Dispose();
			HOperatorSet.GenCircle(out ho_R2_Circle, hv_R2_Pos_Row, hv_R2_Pos_Col, hv_R2_R);
			ho_R2_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_R2_Circle, out ho_R2_ROI_Image);
			ho_R2_Region.Dispose();
			HOperatorSet.FastThreshold(ho_R2_ROI_Image, out ho_R2_Region, 100, 255, 15);
			ho_R2_ImageReduced.Dispose();
			HOperatorSet.ReduceDomain(ho_R2_ROI_Image, ho_R2_Region, out ho_R2_ImageReduced
				);
			//stop ()
			//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
			hv_alpha = 0.9;
			hv_R2_low = 10;
			hv_R2_high = 60;
			ho_R2_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_R2_ImageReduced, out ho_R2_Edges, "sobel_fast",
				hv_alpha, hv_R2_low, hv_R2_high);
			//stop ()
			//*所有的數值越小，表示容錯範圍大，反之亦然
			ho_R2_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_R2_Edges, out ho_R2_ContoursSplit, "lines_circles",
				17, 1, 1);
			//Display the results
			//===========================================================
			HOperatorSet.CountObj(ho_R2_ContoursSplit, out hv_R2_NumSegments);
			hv_NumCircles = 0;
			hv_Num_Circle_Point = 3;
			hv_R2 = 999;
			for (hv_i = 1; hv_i.Continue(hv_R2_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
			{
				ho_R2_SingleSegment.Dispose();
				HOperatorSet.SelectObj(ho_R2_ContoursSplit, out ho_R2_SingleSegment, hv_i);
				HOperatorSet.GetContourGlobalAttribXld(ho_R2_SingleSegment, "cont_approx",
					out hv_Attrib);
				if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
				{
					HOperatorSet.FitCircleContourXld(ho_R2_SingleSegment, "atukey", -1, 2,
						hv_Num_Circle_Point, 5, 2, out hv_R2_Row, out hv_R2_Column, out hv_R2_Radius,
						out hv_R2_StartPhi, out hv_R2_EndPhi, out hv_R2_PointOrder);

					ho_R2_ContEllipse.Dispose();

					hv_NumCircles = hv_NumCircles + 1;
					if ((int)(new HTuple(hv_R2.TupleGreater(hv_R2_Radius))) != 0)
					{
						hv_R2 = hv_R2_Radius.Clone();
						mResult = new CircleResult()
						{
							Row = new HTuple(hv_R2_Row),
							Col = new HTuple(hv_R2_Column),
							Radius = new HTuple(hv_R2_Radius),
							StartPhi = new HTuple(hv_R2_StartPhi),
							EndPhi = new HTuple(hv_R2_EndPhi),
							PointOrder = new HTuple(hv_R2_PointOrder),
						};

						//HOperatorSet.SetTposition(hv_WindowHandle, hv_R2_Pos_Row - hv_msgOffsetY,
						//	hv_R2_Pos_Col - hv_msgOffsetX);
						//HOperatorSet.WriteString(hv_WindowHandle, "R2");
					}
				}
			}

			//*****R2 End

			ho_R2_Circle.Dispose();
			ho_R2_ROI_Image.Dispose();
			ho_R2_Region.Dispose();
			ho_R2_ImageReduced.Dispose();
			ho_R2_Edges.Dispose();
			ho_R2_ContoursSplit.Dispose();
			ho_R2_SingleSegment.Dispose();
			ho_R2_ContEllipse.Dispose();



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
