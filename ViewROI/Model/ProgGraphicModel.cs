using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI
{
	/// <summary>
	/// 用來描述工程圖形
	/// </summary>
	public class ProgGraphicModel
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public bool IsExportItem { get; set; }

		public double StartPhi { get; set; }
		public double EndPhi { get; set; }
		public string PointerOrder { get; set; }

		public double RowBegin { get; set; }
		public double ColBegin { get; set; }
		public double RowEnd { get; set; }
		public double ColEnd { get; set; }
		public double Distance { get; set; }//pixels

		public MeasureType GeoType { get; set; }

		//相依的ROI資訊
		public ProgGraphicModel[] ROIs { get; set; }
	}
}
