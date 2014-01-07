using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	public class MeasureExportModel
	{
		/// <summary>
		/// 名稱
		/// </summary>
		public string MeasureName { get; set; }

		/// <summary>
		/// 符號
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// 量測值
		/// </summary>
		public string MeasureValue { get; set; }

		/// <summary>
		/// 設計值
		/// </summary>
		public string Standard { get; set; }

		/// <summary>
		/// 公差下限
		/// </summary>
		public string Min { get; set; }

		/// <summary>
		/// 公差上限
		/// </summary>
		public string Max { get; set; }

		/// <summary>
		/// 偏差
		/// </summary>
		public string Deviation { get; set; }

		/// <summary>
		/// 評估
		/// </summary>
		public string Evaluation { get; set; }
	}
}
