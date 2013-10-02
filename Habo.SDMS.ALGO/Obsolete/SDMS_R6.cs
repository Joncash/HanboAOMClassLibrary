using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R6 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion
			// Local iconic variables 

			HObject ho_R6_Circle = null;
			HObject ho_R6_ROI_Image = null, ho_R6_Region = null, ho_R6_RegionBorder = null;
			HObject ho_R6_RegionDilation = null, ho_R6_ImageReduced = null;
			HObject ho_R6_Edges = null, ho_R6_ContoursSplit = null, ho_R6_SingleSegment = null;
			HObject ho_R6_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_R6;
			HTuple hv_STD_R6_Row = new HTuple(), hv_STD_R6_Col = new HTuple();
			HTuple hv_STD_R6_V_Row = new HTuple(), hv_STD_R6_V_Col = new HTuple();
			HTuple hv_R6_X = new HTuple(), hv_R6_Y = new HTuple(), hv_R6_Pos_Row = new HTuple();
			HTuple hv_R6_Pos_Col = new HTuple(), hv_R6_R = new HTuple();
			HTuple hv_R6_R_Max = new HTuple(), hv_R6_R_Inc = new HTuple();
			HTuple hv_R6_NumSegments = new HTuple(), hv_NumCircles = new HTuple();
			HTuple hv_Num_Circle_Point = new HTuple(), hv_i = new HTuple();
			HTuple hv_Attrib = new HTuple(), hv_R6_Row = new HTuple();
			HTuple hv_R6_Column = new HTuple(), hv_R6_Radius = new HTuple();
			HTuple hv_R6_StartPhi = new HTuple(), hv_R6_EndPhi = new HTuple();
			HTuple hv_R6_PointOrder = new HTuple(), hv_R6_MinDist = new HTuple();
			HTuple hv_R6_MaxDist = new HTuple(), hv_R6_AvgDist = new HTuple();
			HTuple hv_R6_SigmaDist = new HTuple();

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_R6_Circle);
			HOperatorSet.GenEmptyObj(out ho_R6_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R6_Region);
			HOperatorSet.GenEmptyObj(out ho_R6_RegionBorder);
			HOperatorSet.GenEmptyObj(out ho_R6_RegionDilation);
			HOperatorSet.GenEmptyObj(out ho_R6_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R6_Edges);
			HOperatorSet.GenEmptyObj(out ho_R6_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R6_SingleSegment);
			HOperatorSet.GenEmptyObj(out ho_R6_ContEllipse);

			//Measure: SDMS_R6
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
			//*****R6
			hv_R6 = 999;
			//STD R6_ 位置
			hv_STD_R6_Row = 1292;
			hv_STD_R6_Col = 639;

			//STD 向量 STD_R6_
			hv_STD_R6_V_Row = hv_STD_R6_Row - hv_STD_Row;
			hv_STD_R6_V_Col = hv_STD_R6_Col - hv_STD_Col;


			//R6_X, R6_Y 分量
			hv_R6_X = (hv_STD_R6_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R6_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R6_Y = (hv_STD_R6_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R6_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R4 位置
			hv_R6_Pos_Row = (hv_STD_Row + hv_R6_Y) + hv_OffsetRow;
			hv_R6_Pos_Col = (hv_STD_Col + hv_R6_X) + hv_OffsetCol;

			//R6_Region 由半徑 29 開始搜尋, 最大搜尋至 41, Inc =2
			hv_R6_R = 29;
			hv_R6_R_Max = 41;
			hv_R6_R_Inc = 2;
			while ((int)((new HTuple(hv_R6.TupleEqual(999))).TupleAnd(new HTuple(hv_R6_R.TupleLess(
				hv_R6_R_Max)))) != 0)
			{
				//*******************************
				ho_R6_Circle.Dispose();
				HOperatorSet.GenCircle(out ho_R6_Circle, hv_R6_Pos_Row, hv_R6_Pos_Col, hv_R6_R);
				ho_R6_ROI_Image.Dispose();
				HOperatorSet.ReduceDomain(ho_Image, ho_R6_Circle, out ho_R6_ROI_Image);
				ho_R6_Region.Dispose();
				HOperatorSet.Threshold(ho_R6_ROI_Image, out ho_R6_Region, 100, 255);
				ho_R6_RegionBorder.Dispose();
				HOperatorSet.Boundary(ho_R6_Region, out ho_R6_RegionBorder, "inner");
				ho_R6_RegionDilation.Dispose();
				HOperatorSet.DilationCircle(ho_R6_RegionBorder, out ho_R6_RegionDilation,
					1.5);
				ho_R6_ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(ho_R6_ROI_Image, ho_R6_RegionDilation, out ho_R6_ImageReduced
					);
				ho_R6_Edges.Dispose();
				HOperatorSet.EdgesSubPix(ho_R6_ImageReduced, out ho_R6_Edges, "lanser2",
					0.3, 40, 90);

				//fast_threshold (R6_ROI_Image, R6_Region, 80, 255, 15)
				//reduce_domain (R6_ROI_Image, R6_Region, R6_ImageReduced)
				//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
				//alpha := 10
				//R6_low := 10
				//R6_high := 60
				//edges_sub_pix (R6_ImageReduced, R6_Edges, 'sobel_fast', alpha, R6_low, R6_high)
				//stop ()
				//*所有的數值越小，表示容錯範圍大，反之亦然
				ho_R6_ContoursSplit.Dispose();
				HOperatorSet.SegmentContoursXld(ho_R6_Edges, out ho_R6_ContoursSplit, "lines_circles",
					6, 4, 4);

				//Display the results
				//===========================================================
				HOperatorSet.CountObj(ho_R6_ContoursSplit, out hv_R6_NumSegments);
				hv_NumCircles = 0;
				hv_Num_Circle_Point = 0;
				for (hv_i = 1; hv_i.Continue(hv_R6_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
				{
					ho_R6_SingleSegment.Dispose();
					HOperatorSet.SelectObj(ho_R6_ContoursSplit, out ho_R6_SingleSegment, hv_i);
					HOperatorSet.GetContourGlobalAttribXld(ho_R6_SingleSegment, "cont_approx",
						out hv_Attrib);
					if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
					{
						HOperatorSet.FitCircleContourXld(ho_R6_SingleSegment, "atukey", -1, 2,
							hv_Num_Circle_Point, 5, 2, out hv_R6_Row, out hv_R6_Column, out hv_R6_Radius,
							out hv_R6_StartPhi, out hv_R6_EndPhi, out hv_R6_PointOrder);
						ho_R6_ContEllipse.Dispose();
						HOperatorSet.GenEllipseContourXld(out ho_R6_ContEllipse, hv_R6_Row, hv_R6_Column,
							0, hv_R6_Radius, hv_R6_Radius, 0, (new HTuple(360)).TupleRad(), "positive",
							1.0);
						if (HDevWindowStack.IsOpen())
						{
							HOperatorSet.DispObj(ho_R6_ContEllipse, HDevWindowStack.GetActive()
								);
						}
						HOperatorSet.DistEllipseContourXld(ho_R6_SingleSegment, "algebraic",
							-1, 0, hv_R6_Row, hv_R6_Column, 0, hv_R6_Radius, hv_R6_Radius, out hv_R6_MinDist,
							out hv_R6_MaxDist, out hv_R6_AvgDist, out hv_R6_SigmaDist);
						hv_NumCircles = hv_NumCircles + 1;
						if ((int)(new HTuple(hv_R6.TupleGreater(hv_R6_Radius))) != 0)
						{
							hv_R6 = hv_R6_Radius.Clone();
							mResult = new CircleResult()
							{
								Row = new HTuple(hv_R6_Row),
								Col = new HTuple(hv_R6_Column),
								Radius = new HTuple(hv_R6_Radius),
								StartPhi = new HTuple(hv_R6_StartPhi),
								EndPhi = new HTuple(hv_R6_EndPhi),
								PointOrder = new HTuple(hv_R6_PointOrder),
							};
						}
					}
				}
				hv_R6_R = hv_R6_R + hv_R6_R_Inc;
			}

			ho_R6_Circle.Dispose();
			ho_R6_ROI_Image.Dispose();
			ho_R6_Region.Dispose();
			ho_R6_RegionBorder.Dispose();
			ho_R6_RegionDilation.Dispose();
			ho_R6_ImageReduced.Dispose();
			ho_R6_Edges.Dispose();
			ho_R6_ContoursSplit.Dispose();
			ho_R6_SingleSegment.Dispose();
			ho_R6_ContEllipse.Dispose();



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
