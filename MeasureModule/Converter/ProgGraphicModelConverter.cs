using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI;
namespace MeasureModule
{
	/// <summary>
	/// 工程模形轉換器
	/// </summary>
	public class ProgGraphicModelConverter
	{
		public ROIProgDistance ConvertToROIProgDistance(ProgGraphicModel raw)
		{
			ROIProgDistance model = new ROIProgDistance();
			return model;
		}

		public ROIProgCircle ConvertToROIProgCircle(ProgGraphicModel raw)
		{
			ROIProgCircle model = new ROIProgCircle();
			return model;
		}
	}
}
