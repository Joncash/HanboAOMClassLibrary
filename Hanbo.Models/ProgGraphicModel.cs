using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	/// <summary>
	/// 用來描述工程圖形
	/// </summary>
	public class ProgGraphicModel
	{
		public string ID { get; set; }
		public string Nmae { get; set; }
		public bool IsExportItem { get; set; }

		public double StartPhi { get; set; }
		public double EndPhi { get; set; }
		public string PointerOrder { get; set; }

		public double RowBegin { get; set; }
		public double ColBegin { get; set; }
		public double RowEnd { get; set; }
		public double ColEnd { get; set; }
		public double Distance { get; set; }//pixels

		public string MeasureType { get; set; }

		//相依的ROI資訊
		public ProgGraphicModel[] ROIs { get; set; }
	}
}
