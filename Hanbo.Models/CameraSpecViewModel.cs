using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	/// <summary>
	/// Camera 規格 ViewModel
	/// </summary>
	public class CameraSpecViewModel
	{
		/// <summary>
		/// 水平方向解析度 (pixel)
		/// </summary>
		public int HorizontalResolution { get; set; }

		/// <summary>
		/// 水平方向 Pixel size
		/// </summary>
		public double HorizontalPixelSize { get; set; }

		/// <summary>
		/// 垂直方向解析度 (pixel)
		/// </summary>
		public int VerticalResolution { get; set; }

		/// <summary>
		/// 垂直方向 Pixel size
		/// </summary>
		public double VerticalPixelSize { get; set; }


	}
}
