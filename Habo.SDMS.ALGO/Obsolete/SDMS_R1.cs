using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Habo.SDMS.ALGO
{
	public class SDMS_R1 : IMeasure
	{

		/// <summary>
		/// 初始化後才能叫用 Action
		/// </summary>
		public MeasureResult Action()
		{
			#region 輸出結果， CircleResult or Distance Result
			CircleResult mResult = null;
			#endregion

			// Local iconic variables 

			HObject ho_R1_Circle = null;
			HObject ho_R1_ROI_Image = null, ho_R1_Region = null, ho_R1_ImageReduced = null;
			HObject ho_R1_Edges = null, ho_R1_ContoursSplit = null, ho_R1_SingleSegment = null;
			//HObject ho_R1_ContEllipse = null;


			// Local control variables 

			HTuple hv_msgOffsetY, hv_msgOffsetX;
			//HTuple hv_AllModelId;
			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol, hv_STD_R1_Row, hv_STD_R1_Col;
			HTuple hv_STD_R1_V_Row, hv_STD_R1_V_Col, hv_R1_X, hv_R1_Y;
			HTuple hv_R1_Pos_Row, hv_R1_Pos_Col, hv_R1_R;
			HTuple hv_alpha = new HTuple(), hv_R1_low = new HTuple(), hv_R1_high = new HTuple();
			HTuple hv_R1_NumSegments = new HTuple(), hv_NumCircles = new HTuple();
			HTuple hv_Num_Circle_Point = new HTuple(), hv_R1 = new HTuple();
			HTuple hv_R1_limit = new HTuple(), hv_i = new HTuple(), hv_Attrib = new HTuple();
			HTuple hv_R1_Row = new HTuple(), hv_R1_Column = new HTuple();
			HTuple hv_R1_Radius = new HTuple(), hv_R1_StartPhi = new HTuple();
			HTuple hv_R1_EndPhi = new HTuple(), hv_R1_PointOrder = new HTuple();
			HTuple hv_R1_MinDist = new HTuple(), hv_R1_MaxDist = new HTuple();
			HTuple hv_R1_AvgDist = new HTuple(), hv_R1_SigmaDist = new HTuple();
			HTuple hv_ResultText = new HTuple(), hv_MeasureReasult;

			// Initialize local and output iconic variables 
			//HOperatorSet.GenEmptyObj(out ho_Image);
			HOperatorSet.GenEmptyObj(out ho_R1_Circle);
			HOperatorSet.GenEmptyObj(out ho_R1_ROI_Image);
			HOperatorSet.GenEmptyObj(out ho_R1_Region);
			HOperatorSet.GenEmptyObj(out ho_R1_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_R1_Edges);
			HOperatorSet.GenEmptyObj(out ho_R1_ContoursSplit);
			HOperatorSet.GenEmptyObj(out ho_R1_SingleSegment);
			//HOperatorSet.GenEmptyObj(out ho_R1_ContEllipse);

			//Measure: SDMS_R1
			//Author: John Hsieh
			//Date: 2012
			//ho_Image.Dispose();
			//HOperatorSet.ReadImage(out ho_Image, "D:/Projects/Halcon/SDMS/SDMS_Measure/Images/E-1/E1-1.bmp");
			//dev_open_window_fit_image(ho_Image, 0, 0, -1, -1, out hv_WindowHandle);
			//dev_update_off ()
			// dev_update_window(...); only in hdevelop
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
			//	out hv_AllModelColumn, out hv_AllModelAngle, out modelScore);

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


			//****R1
			//STD R1_ 位置
			hv_STD_R1_Row = 589;
			hv_STD_R1_Col = 705;

			//STD 向量 STD_R1_
			hv_STD_R1_V_Row = hv_STD_R1_Row - hv_STD_Row;
			hv_STD_R1_V_Col = hv_STD_R1_Col - hv_STD_Col;


			//R1_X, R1_Y 分量
			hv_R1_X = (hv_STD_R1_V_Col * (hv_Img_Rotate_Angle.TupleCos())) + (hv_STD_R1_V_Row * (hv_Img_Rotate_Angle.TupleSin()
				));
			hv_R1_Y = (hv_STD_R1_V_Row * (hv_Img_Rotate_Angle.TupleCos())) - (hv_STD_R1_V_Col * (hv_Img_Rotate_Angle.TupleSin()
				));


			//目前圖形 R1_ 位置
			hv_R1_Pos_Row = (hv_STD_Row + hv_R1_Y) + hv_OffsetRow;
			hv_R1_Pos_Col = (hv_STD_Col + hv_R1_X) + hv_OffsetCol;

			hv_R1_R = 20;
			//*ROI
			ho_R1_Circle.Dispose();
			HOperatorSet.GenCircle(out ho_R1_Circle, hv_R1_Pos_Row, hv_R1_Pos_Col, hv_R1_R);
			ho_R1_ROI_Image.Dispose();
			HOperatorSet.ReduceDomain(ho_Image, ho_R1_Circle, out ho_R1_ROI_Image);
			ho_R1_Region.Dispose();
			HOperatorSet.FastThreshold(ho_R1_ROI_Image, out ho_R1_Region, 100, 255, 20);
			ho_R1_ImageReduced.Dispose();
			HOperatorSet.ReduceDomain(ho_R1_ROI_Image, ho_R1_Region, out ho_R1_ImageReduced
				);

			//sobel_fast 具有較寬的選擇範圍，搭配 alpha 參數 (alpha 越大, 容錯範圍大)
			hv_alpha = 0.9;
			hv_R1_low = 20;
			hv_R1_high = 40;
			ho_R1_Edges.Dispose();
			HOperatorSet.EdgesSubPix(ho_R1_ImageReduced, out ho_R1_Edges, "sobel_fast",
				hv_alpha, hv_R1_low, hv_R1_high);
			//stop ()
			//*所有的數值越小，表示容錯範圍大，反之亦然
			ho_R1_ContoursSplit.Dispose();
			HOperatorSet.SegmentContoursXld(ho_R1_Edges, out ho_R1_ContoursSplit, "lines_circles",
				17, 1, 1);
			//Display the results
			//===========================================================
			HOperatorSet.CountObj(ho_R1_ContoursSplit, out hv_R1_NumSegments);
			hv_NumCircles = 0;
			hv_Num_Circle_Point = 5;
			hv_R1 = 999;
			hv_R1_limit = 10;
			for (hv_i = 1; hv_i.Continue(hv_R1_NumSegments, 1); hv_i = hv_i.TupleAdd(1))
			{
				ho_R1_SingleSegment.Dispose();
				HOperatorSet.SelectObj(ho_R1_ContoursSplit, out ho_R1_SingleSegment, hv_i);
				HOperatorSet.GetContourGlobalAttribXld(ho_R1_SingleSegment, "cont_approx", out hv_Attrib);


				if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
				{
					HOperatorSet.FitCircleContourXld(ho_R1_SingleSegment, "atukey", -1, 2,
						hv_Num_Circle_Point, 5, 2, out hv_R1_Row, out hv_R1_Column, out hv_R1_Radius,
						out hv_R1_StartPhi, out hv_R1_EndPhi, out hv_R1_PointOrder);

					//ho_R1_ContEllipse.Dispose();
					//HOperatorSet.GenEllipseContourXld(out ho_R1_ContEllipse, hv_R1_Row, hv_R1_Column,
					//	0, hv_R1_Radius, hv_R1_Radius, 0, (new HTuple(360)).TupleRad(), "positive",
					//	1.0);

					//HOperatorSet.DistEllipseContourXld(ho_R1_SingleSegment, "algebraic", -1,
					//	0, hv_R1_Row, hv_R1_Column, 0, hv_R1_Radius, hv_R1_Radius, out hv_R1_MinDist,
					//	out hv_R1_MaxDist, out hv_R1_AvgDist, out hv_R1_SigmaDist);

					hv_NumCircles = hv_NumCircles + 1;
					if ((int)(new HTuple(hv_R1.TupleGreater(hv_R1_Radius))) != 0)
					{
						hv_R1 = hv_R1_Radius.Clone();
						//hv_ResultText = (((("C" + hv_NumCircles) + ": Radius = ") + (hv_R1_Radius.TupleString(
						//	".3"))) + " / MaxDeviation: ") + (hv_R1_MaxDist.TupleString(".3"));
						//HOperatorSet.SetTposition(hv_WindowHandle, hv_R1_Pos_Row - hv_msgOffsetY,
						//	hv_R1_Pos_Col - hv_msgOffsetX);
						//HOperatorSet.WriteString(hv_WindowHandle, "R1");

						#region 組合結果
						mResult = new CircleResult(new HTuple(hv_R1_Row)
												, new HTuple(hv_R1_Column)
												, new HTuple(hv_R1_Radius)
												, new HTuple(hv_R1_StartPhi)
												, new HTuple(hv_R1_EndPhi)
												, new HTuple(hv_R1_PointOrder)) { };
						#endregion
					}
				}
			}
			hv_MeasureReasult = hv_R1.Clone();
			//****R1 End
			//ho_Image.Dispose();
			ho_R1_Circle.Dispose();
			ho_R1_ROI_Image.Dispose();
			ho_R1_Region.Dispose();
			ho_R1_ImageReduced.Dispose();
			ho_R1_Edges.Dispose();
			ho_R1_ContoursSplit.Dispose();
			ho_R1_SingleSegment.Dispose();
			//ho_R1_ContEllipse.Dispose();

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
