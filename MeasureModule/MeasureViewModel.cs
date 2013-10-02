using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using ViewROI;

namespace MeasureModule
{

	/// <summary>
	/// 量測的呈現資料模形
	/// </summary>
	public class MeasureViewModel : IMeasureGeoModel
	{
		/// <summary>
		/// 點1 Y or 圓心座標 Y
		/// </summary>
		public HTuple Row1 { get; set; }

		/// <summary>
		/// 點1 X or 圓心座標 X
		/// </summary>
		public HTuple Col1 { get; set; }

		/// <summary>
		/// 點 2 Y
		/// </summary>
		public HTuple Row2 { get; set; }

		/// <summary>
		/// 點 2 X
		/// </summary>
		public HTuple Col2 { get; set; }

		/// <summary>
		/// 半徑 or 距離
		/// </summary>
		public HTuple Distance { get; set; }

		/// <summary>
		/// 起始角度
		/// </summary>
		public HTuple StartPhi { get; set; }

		/// <summary>
		/// 結束角度
		/// </summary>
		public HTuple EndPhi { get; set; }

		public HTuple PointOrder { get; set; }

		/// <summary>
		/// 幾何類型
		/// </summary>
		public MeasureType GeoType { get; set; }

		/// <summary>
		/// 角度
		/// </summary>
		public HTuple Angle { get; set; }
	}
}
