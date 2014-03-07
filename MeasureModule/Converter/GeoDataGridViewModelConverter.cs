using MeasureModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;
using ViewROI.Model;

namespace MeasureModule
{
	/// <summary>
	/// 轉換器
	/// </summary>
	public class GeoDataGridViewModelConverter
	{
		public ROI ConvertToProgROI(GeoDataGridViewModel raw, GeoDataGridViewModel[] dependROIs, int _circleDistanceSetting)
		{
			var model = convertToProgGraphicModel(raw, dependROIs);
			ROI progROI = null;
			switch (model.GeoType)
			{
				case MeasureType.Angle:
					progROI = new ROIProgAngle(model);
					break;
				case MeasureType.Circle:
				case MeasureType.PointCircle:
					progROI = new ROIProgCircle(model) { CircleDistanceSetting = _circleDistanceSetting };
					break;
				case MeasureType.CrossPoint:
					progROI = new ROIProgPoint(model);
					break;
				case MeasureType.Distance:
				case MeasureType.DistanceX:
				case MeasureType.DistanceY:
					progROI = new ROIProgDistance(model);
					break;
				case MeasureType.SymmetryLine:
					progROI = new ROIProgSymmetryLine(model);
					break;
			}
			return progROI;
		}

		/// <summary>
		/// 轉換為 工程圖 ROI
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="dependROIs"></param>
		/// <returns></returns>
		public ROI ConvertToProgROI(GeoDataGridViewModel raw, GeoDataGridViewModel[] dependROIs)
		{
			return ConvertToProgROI(raw, dependROIs, 1);
		}
		#region private methods
		private ProgGraphicModel convertToProgGraphicModel(GeoDataGridViewModel raw, GeoDataGridViewModel[] dependROIs)
		{
			var model = assignValue(raw);

			/*var basicGeoType = new List<MeasureType>() { 
				{MeasureType.FitLine},
				{MeasureType.Line},
				{MeasureType.Point},
				{MeasureType.Circle}
			};
			 */
			//有相依的ROI	
			var basicROIs = dependROIs;//.Where(p => basicGeoType.Contains(p.GeoType)).ToArray();
			int dependROICount = basicROIs.Length;
			if (dependROICount > 0)
			{
				model.ROIs = new ProgGraphicModel[dependROICount];
				for (int i = 0; i < dependROICount; i++)
				{
					var dependROI = basicROIs[i];

					var dependItem = assignValue(dependROI);
					model.ROIs[i] = dependItem;
				}
			}
			return model;
		}
		private ProgGraphicModel assignValue(GeoDataGridViewModel raw)
		{
			ProgGraphicModel model = null;
			try
			{
				model = new ProgGraphicModel()
					{
						ID = raw.RecordID,
						Name = raw.Name,
						GeoType = raw.GeoType,
						RowBegin = raw.Row1,
						RowEnd = raw.Row2,
						ColBegin = raw.Col1,
						ColEnd = raw.Col2,
						StartPhi = (raw.StartPhi),
						EndPhi = raw.EndPhi,
						PointerOrder = raw.PointOrder,
						Distance = raw.Distance,
						IsExportItem = raw.IsExportItem,
					};
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
			return model;
		}
		#endregion

	}
}
