using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI;

namespace MeasureModule
{
	/// <summary>
	/// 定義幾何量測資料模型
	/// </summary>
	public interface IMeasureGeoModel
	{

		/// <summary>
		/// 點1 Y or 圓心座標 Y
		/// </summary>
		HTuple Row1 { get; set; }

		/// <summary>
		/// 點1 X or 圓心座標 X
		/// </summary>
		HTuple Col1 { get; set; }

		/// <summary>
		/// 點 2 Y
		/// </summary>
		HTuple Row2 { get; set; }

		/// <summary>
		/// 點 2 X
		/// </summary>
		HTuple Col2 { get; set; }

		/// <summary>
		/// 半徑 or 距離
		/// </summary>
		HTuple Distance { get; set; }

		/// <summary>
		/// 幾何類型
		/// </summary>
		MeasureType GeoType { get; set; }

	}
}
