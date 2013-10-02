using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R17 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion

			// Local iconic variables 

			HObject ho_R17_Circle = null;
			HObject ho_R17_ROI_Image = null, ho_R17_Region = null, ho_R17_ImageReduced = null;
			HObject ho_R17_Edges = null, ho_R17_ContoursSplit = null, ho_R17_SingleSegment = null;
			HObject ho_R17_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_R17_Row, hv_STD_R17_Col;
			HTuple hv_STD_R17_V_Row, hv_STD_R17_V_Col, hv_R17_X, hv_R17_Y;
			HTuple hv_R17_Pos_Row, hv_R17_Pos_Col, hv_R17_R;
			HTuple hv_alpha = new HTuple(), hv_R17_low = new HTuple();
			HTuple hv_R17_high = new HTuple(), hv_R17_NumSegments = new HTuple();
			HTuple hv_NumCircles = new HTuple(), hv_Num_Circle_Point = new HTuple();
			HTuple hv_R17 = new HTuple(), hv_i = new HTuple(), hv_Attrib = new HTuple();
			HTuple hv_R17_Row = new HTuple(), hv_R17_Column = new HTuple();
			HTuple hv_R17_Radius = new HTuple(), hv_R17_StartPhi = new HTuple();
			HTuple hv_R17_EndPhi = new HTuple(), hv_R17_PointOrder = new HTuple();
			HTuple hv_R17_MinDist = new HTuple(), hv_R17_MaxDist = new HTuple();
			HTuple hv_R17_AvgDist = new HTuple(), hv_R17_SigmaDist = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_R17_Circle);
			HOperatorSet.GenEmptyObj(out ho_R17_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R17_Region);
			HOperatorSet.GenEmptyObj(out ho_R17_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R17_Edges);
			HOperatorSet.GenEmptyObj(out ho_R17_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R17_SingleSegment);
			HOperatorSet.GenEmptyObj(out ho_R17_ContEllipse);

			//Measure: SDMS_R17
			//Author: John Hsieh
			//Date: 2012
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
			//*****R17
			//STD R17_ 位置
			hv_STD_R17_Row = 802;
			hv_STD_R17_Col = 660;

			//STD 向量 STD_R17_
			hv_STD_R17_V_Row = hv_STD_R17_Row - hv_STD_Row;
			hv_STD_R17_V_Col = hv_STD_R17_Col - hv_STD_Col;


			//R17_X, R17_Y 分量
			hv_R17_X = (hv_STD_R17_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R17_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R17_Y = (hv_STD_R17_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R17_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R17_ 位置
			hv_R17_Pos_Row = (hv_STD_Row + hv_R17_Y) + hv_OffsetRow;
			hv_R17_Pos_Col = (hv_STD_Col + hv_R17_X) + hv_OffsetCol;

			hv_R17_R = 17;

			ho_R17_Circle.Dispose();
			HOperatorSet.GenCircle(out ho_R17_Circle, hv_R17_Pos_Row, hv_R17_Pos_Col, hv_R17_R);
			if (HDevWindowStack.IsOpen())
			{
				//dev_display (R17_Circle)
			}
			//stop ()

			ho_R17_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_R17_Circle, out ho_R17_ROI_Image);
			ho_R17_Region.Dispose();
			HOperatorSet.FastThreshold(ho_R17_ROI_Image, out ho_R17_Region, 100, 255, 15);
			ho_R17_ImageReduced.Dispose();
			HOperatorSet.ReduceDomain(ho_R17_ROI_Image, ho_R17_Region, out ho_R17_ImageReduced
				);
			//stop ()
			//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
			hv_alpha = 0.9;
			hv_R17_low = 10;
			hv_R17_high = 60;
			ho_R17_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_R17_ImageReduced, out ho_R17_Edges, "sobel_fast",
				hv_alpha, hv_R17_low, hv_R17_high);
			//stop ()
			//*所有的數值越小，表示容錯範圍大，反之亦然
			ho_R17_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_R17_Edges, out ho_R17_ContoursSplit, "lines_circles",
				17, 1, 1);
			//Display the results
			//===========================================================
			HOperatorSet.CountObj(ho_R17_ContoursSplit, out hv_R17_NumSegments);
			hv_NumCircles = 0;
			hv_Num_Circle_Point = 0;
			hv_R17 = 999;
			for (hv_i = 1; hv_i.Continue(hv_R17_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
			{
				ho_R17_SingleSegment.Dispose();
				HOperatorSet.SelectObj(ho_R17_ContoursSplit, out ho_R17_SingleSegment, hv_i);
				HOperatorSet.GetContourGlobalAttribXld(ho_R17_SingleSegment, "cont_approx",
					out hv_Attrib);
				if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
				{
					HOperatorSet.FitCircleContourXld(ho_R17_SingleSegment, "atukey", -1, 2,
						hv_Num_Circle_Point, 5, 2, out hv_R17_Row, out hv_R17_Column, out hv_R17_Radius,
						out hv_R17_StartPhi, out hv_R17_EndPhi, out hv_R17_PointOrder);
					ho_R17_ContEllipse.Dispose();
					HOperatorSet.GenEllipseContourXld(out ho_R17_ContEllipse, hv_R17_Row, hv_R17_Column,
						0, hv_R17_Radius, hv_R17_Radius, 0, (new HTuple(360)).TupleRad(), "positive",
						1.0);
					if (HDevWindowStack.IsOpen())
					{
						HOperatorSet.DispObj(ho_R17_ContEllipse, HDevWindowStack.GetActive());
					}
					HOperatorSet.DistEllipseContourXld(ho_R17_SingleSegment, "algebraic", -1,
						0, hv_R17_Row, hv_R17_Column, 0, hv_R17_Radius, hv_R17_Radius, out hv_R17_MinDist,
						out hv_R17_MaxDist, out hv_R17_AvgDist, out hv_R17_SigmaDist);
					hv_NumCircles = hv_NumCircles + 1;
					if ((int)(new HTuple(hv_R17.TupleGreater(hv_R17_Radius))) != 0)
					{
						hv_R17 = hv_R17_Radius.Clone();
						mResult = new CircleResult()
						{
							Row = new HTuple(hv_R17_Row),
							Col = new HTuple(hv_R17_Column),
							Radius = new HTuple(hv_R17_Radius),
							StartPhi = new HTuple(hv_R17_StartPhi),
							EndPhi = new HTuple(hv_R17_EndPhi),
							PointOrder = new HTuple(hv_R17_PointOrder),
						};
					}
					//stop ()
				}
			}

			ho_R17_Circle.Dispose();
			ho_R17_ROI_Image.Dispose();
			ho_R17_Region.Dispose();
			ho_R17_ImageReduced.Dispose();
			ho_R17_Edges.Dispose();
			ho_R17_ContoursSplit.Dispose();
			ho_R17_SingleSegment.Dispose();
			ho_R17_ContEllipse.Dispose();


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
