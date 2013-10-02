using HalconDotNet;
using MeasureModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule.Resolver
{
	/// <summary>
	/// 將 不同類型 ViewModel 轉換為 IMeasureGeoModel 介面
	/// </summary>
	public class MeasureViewModelResolver
	{

		public static IMeasureGeoModel Resolve(GeoDataGridViewModel rawModel)
		{
			MeasureViewModel model = null;
			if (rawModel != null)
			{
				model = new MeasureViewModel()
				{
					Row1 = new HTuple(rawModel.Row1),
					Row2 = new HTuple(rawModel.Row2),
					Col1 = new HTuple(rawModel.Col1),
					Col2 = new HTuple(rawModel.Col2),
					Distance = new HTuple(rawModel.Distance),
					GeoType = rawModel.GeoType
				};
			}
			return model;
		}
	}
}
