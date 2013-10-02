using HalconDotNet;
using Hanbo.Helper;
using Hanbo.Interface;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;

namespace Habo.SDMS.ALGO.MSD
{
	public class SDMS_R5 : IMeasure
	{
		public MeasureResult Action()
		{
			#region 輸出結果
			CircleResult mResult = null;
			#endregion

			HTuple hv_STD_Row;
			HTuple hv_STD_Col, hv_Img_Row, hv_Img_Col, hv_Img_Rotate_Angle;
			HTuple hv_OffsetRow, hv_OffsetCol;
			HOperatorSet.SetSystem("border_shape_models", "false");

			//STD 中心點
			hv_STD_Row = 839.5;
			hv_STD_Col = 1046.5;

			//目前圖形 中心點
			hv_Img_Row = hv_AllModelRow.Clone();
			hv_Img_Col = hv_AllModelColumn.Clone();

			//目前圖形 Rotate Angle
			hv_Img_Rotate_Angle = hv_AllModelAngle.Clone();

			//目前圖形偏移量
			hv_OffsetRow = hv_Img_Row - hv_STD_Row;
			hv_OffsetCol = hv_Img_Col - hv_STD_Col;

			//圓ROI
			HTuple f_ROI_Row = 1294.556640625;
			HTuple f_ROI_Col = 1363.94281045752;
			HTuple f_radius = 33.9528486452269;

			//重定位
			HTuple f_ROI_Cur_Row, f_ROI_Cur_Col;
			PositionLocater.ReLocater(hv_STD_Row, hv_STD_Col, hv_AllModelAngle, hv_OffsetRow, hv_OffsetCol, f_ROI_Row, f_ROI_Col, out f_ROI_Cur_Row, out f_ROI_Cur_Col);


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

			var roiF = new ROICircle() { ROIMeasureType = MeasureType.Circle };
			//roiF.MakeROI(416, 998, 0, 26.5, 71.2);
			roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_radius);


			double inc = 0.3;
			double miniRadius = 0.7;
			double maxRadius = 0.9;
			double currentRadius = 0;
			int doingCount = 0;
			//,0.2,0.4,0.6
			//0.2,0.4,0.6
			//0.7,0.8,0.9
			MeasureViewModel model = null;
			var inBoundry = false;
			do
			{
				roiF.MakeROI(f_ROI_Cur_Row, f_ROI_Cur_Col, f_radius);
				var fitCircle = new MeasurementCircle(roiF, cAssistant);
				model = fitCircle.GetViewModel();
				if (model.Distance != null && model.Distance.TupleLength() > 0)
				{
					currentRadius = CameraSystem.ToRealWorldValue(model.Distance.D);
					inBoundry = (miniRadius <= currentRadius && currentRadius <= maxRadius);
					if (inBoundry)
					{
						mResult = new CircleResult()
						{
							Row = new HTuple(model.Row1),
							Col = new HTuple(model.Col1),
							Radius = new HTuple(model.Distance),
							StartPhi = new HTuple(model.StartPhi),
							EndPhi = new HTuple(model.EndPhi),
							PointOrder = new HTuple(model.PointOrder),
						};
						break;
					}
				}
				if (currentRadius < miniRadius) f_radius += inc;
				else f_radius -= inc;
				doingCount++;
				if (doingCount > 15) break;
			}
			while (!inBoundry);

			#endregion
			return mResult;
		}

		#region Initialize
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
		#endregion
	}
}
