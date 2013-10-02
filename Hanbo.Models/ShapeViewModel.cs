using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	/// <summary>
	/// 訓練模型的資訊
	/// </summary>
	public class ShapeViewModel
	{
		/// <summary>
		/// 中心點 Row
		/// </summary>
		public double? Row { get; set; }

		/// <summary>
		/// 中心點 Col
		/// </summary>
		public double? Col { get; set; }

		/// <summary>
		/// 角度
		/// </summary>
		public double? Angle { get; set; }
	}
}
