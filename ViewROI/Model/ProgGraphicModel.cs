using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI.Model
{
	/// <summary>
	/// 用來描述工程圖形
	/// </summary>
	public class ProgGraphicModel
	{
		/// <summary>
		/// 工程圖 ID = Record ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 工程圖顯示的文字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Y 起始點
		/// </summary>
		public double RowBegin { get; set; }

		/// <summary>
		/// X 起始點
		/// </summary>
		public double ColBegin { get; set; }

		/// <summary>
		/// Y 終點
		/// </summary>
		public double RowEnd { get; set; }

		/// <summary>
		/// X 終點
		/// </summary>
		public double ColEnd { get; set; }

		/// <summary>
		/// 兩元素距離 (pixels)
		/// </summary>
		public double Distance { get; set; }//pixels

		/// <summary>
		/// 量測類型
		/// </summary>
		public MeasureType GeoType { get; set; }

		public bool IsExportItem { get; set; }
		public double StartPhi { get; set; }
		public double EndPhi { get; set; }
		public string PointerOrder { get; set; }

		//相依的ROI資訊
		public ProgGraphicModel[] ROIs { get; set; }

		/// <summary>
		/// 使用者定義的 工程圖中心點位置 x 座標
		/// </summary>
		public Double? UserDefineCenterCol { get; set; }

		/// <summary>
		/// 使用者定義的 工程圖中心點位置 y 座標
		/// </summary>
		public Double? UserDefineCenterRow { get; set; }
	}
}
