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
		public ProgGraphicModel ConvertToProgGraphicModel(GeoDataGridViewModel raw, GeoDataGridViewModel[] dependROIs)
		{
			var model = assignValue(raw);

			//有相依的ROI			
			int dependROICount = dependROIs.Length;
			if (dependROICount > 0)
			{
				model.ROIs = new ProgGraphicModel[dependROICount];
				for (int i = 0; i < dependROICount; i++)
				{
					var dependItem = assignValue(dependROIs[i]);
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
	}
}
