using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	//class ResultDisplayViewModel
	//{
	//}

	/// <summary>
	/// 顯示圖形與文字的 ViewModel
	/// </summary>
	public class ResultDisplayViewModel
	{
		/// <summary>
		/// 圖形
		/// </summary>
		public HObject ImageXLD { get; set; }

		/// <summary>
		/// 顯示文字
		/// </summary>
		public string DisplayText { get; set; }

		/// <summary>
		/// 顯示位置 X
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// 顯示位置 Y
		/// </summary>
		public double PositionY { get; set; }

		public double FirstArrowX { get; set; }
		public double FirstArrowY { get; set; }
		public double SecArrowX { get; set; }
		public double SecArrowY { get; set; }
	}
}
