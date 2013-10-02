using HalconDotNet;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace Habo.SDMS.ALGO
{
	public class SDMS_B1 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			DistanceResult mResult = null;
			#endregion

			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol;
			HOperatorSet.SetSystem("border_shape_models", "false");

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

			//第一個 線段 ROI
			HTuple f_angle = 1.599;
			HTuple f_angleOffset = f_angle - hv_Img_Rotate_Angle;
			HTuple f_ROI_Row = 176.17, f_ROI_Col = 796;
			HTuple f_ROI_Cur_Row, f_ROI_Cur_Col;
			ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol, f_ROI_Row, f_ROI_Col, out f_ROI_Cur_Row, out f_ROI_Cur_Col);

			//

			//第二個 線段 ROI
			HTuple s_angel = 0;
			HTuple s_angleOffset = s_angel - hv_Img_Rotate_Angle;
			HTuple s_ROI_Row = 686.550, s_ROI_Col = 650.954;
			HTuple s_ROI_Cur_Row, s_ROI_Cur_Col;
			ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol
					, s_ROI_Row, s_ROI_Col, out s_ROI_Cur_Row, out s_ROI_Cur_Col);
			//roiS.MakeROI(s_ROI_Cur_Row, s_ROI_Col, 0, 13.3, 75.7);

			//Angle
			
			

			/**/

			#region Measure
			var cROIController = new ROIController();
			var cAssistant = new MeasureAssistant(cROIController);

			var hImage = ho_Image as HImage;
			cAssistant.setImage(hImage);

			/*參數值*/
			cAssistant.mThresh = 40.0;
			cAssistant.mSigma = 1.0;
			cAssistant.mRoiWidth = 10;
			cAssistant.mInterpolation = "nearest_neighbor";
			cAssistant.mSelPair = false;
			cAssistant.mTransition = "all";
			cAssistant.mPosition = "all";
			cAssistant.mDispEdgeLength = 30;
			cAssistant.mDispROIWidth = true;
			cAssistant.setUnit("cm");

			cAssistant.mInitThresh = 40.0;
			cAssistant.mInitSigma = 1.0;
			cAssistant.mInitRoiWidth = 10;

			var roiF = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiF.MakeROI(416, 998, 0, 26.5, 71.2);
			roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_angleOffset, 641.9, 187.3);

			var roiS = new ROIRectangle2() { ROIMeasureType = MeasureType.Line };
			//roiS.MakeROI(400, 1041, 0, 13.3, 75.7);
			roiS.MakeROI(s_ROI_Cur_Row, s_ROI_Cur_Col, s_angleOffset, 63.22, 23.74);

			var lineF = new MeasurementEdge(roiF, cAssistant);
			var lineS = new MeasurementEdge(roiS, cAssistant);
			mResult = PointToLine(lineF, lineS);

			#endregion

			return mResult;
		}

		private DistanceResult PointToLine(Measurement first, Measurement second)
		{
			var firstData = first.GetViewModel(); // GetMeasureResultDataModle(first.getMeasureResultData());
			var secData = second.GetViewModel(); //GetMeasureResultDataModle(second.getMeasureResultData());
			HTuple distanceMin, distanceMax;

			var point = (firstData.Row2 != null) ? secData : firstData;
			var line = (firstData.Row2 != null) ? firstData : secData;

			HOperatorSet.DistancePs(point.Row1, point.Col1, line.Row1, line.Col1, line.Row2, line.Col2, out distanceMin, out distanceMax);

			return new DistanceResult()
			{
				FirstRowBegin = new HTuple(line.Row1),
				FirstColBegin = new HTuple(line.Col1),
				FirstRowEnd = new HTuple(line.Row2),
				FirstColEnd = new HTuple(line.Col2),
				SecondRowBegin = new HTuple(point.Row1),
				SecondColBegin = new HTuple(point.Col1),
				//SecondRowEnd = new HTuple(line2.Row2),
				//SecondColEnd = new HTuple(line2.Col2),
				Direction = LineDirection.Vertical,
				Distance = new HTuple(distanceMin),
				Angle = this.hv_AllModelAngle,
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stdRow">訓練圖形中心點 Row</param>
		/// <param name="stdCol">訓練圖形中心點 Col</param>
		/// <param name="curModelAngle">目前圖形 Angle</param>
		/// <param name="hv_OffsetRow">hv_Img_Row - hv_STD_Row (現在圖形中心點 Row - 訓練圖形中心點 Row</param>
		/// <param name="hv_OffsetCol">hv_Img_Col - hv_STD_Col (現在圖形中心點 Col - 訓練圖形中心點 Col</param>		
		/// <param name="roiRow">訓練圖形時設定的 ROI Row</param>
		/// <param name="roiCol">訓練圖形時設定的 ROI Col</param>
		/// <param name="curROI_Row">回傳值 - 重定位後的 Row</param>
		/// <param name="curROI_Col">回傳值 - 重定位後的 Col</param>
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
