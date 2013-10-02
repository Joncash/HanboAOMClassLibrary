using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	/// <summary>
	/// 位置定位器
	/// 影像經由 mathcing 後， Model 的中心位置與訓練時的影像中心位置已有變化
	/// 使用 PositionLocater, 重新定位
	/// </summary>
	public static class PositionLocater
	{
		/// <summary>
		/// 重定位
		/// </summary>
		/// <param name="stdRow">訓練圖形中心點 Row</param>
		/// <param name="stdCol">訓練圖形中心點 Col</param>
		/// <param name="curModelAngle">目前圖形 Angle</param>
		/// <param name="hv_OffsetRow">(現在圖形中心點 Row - 訓練圖形中心點 Row</param>
		/// <param name="hv_OffsetCol">(現在圖形中心點 Col - 訓練圖形中心點 Col</param>		
		/// <param name="roiRow">訓練圖形時設定的 ROI Row</param>
		/// <param name="roiCol">訓練圖形時設定的 ROI Col</param>
		/// <param name="curROI_Row">回傳值 - 重定位後的 Row</param>
		/// <param name="curROI_Col">回傳值 - 重定位後的 Col</param>
		public static void ReLocater(HTuple stdRow, HTuple stdCol, HTuple curModelAngle, HTuple offsetRow, HTuple offsetCol
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
