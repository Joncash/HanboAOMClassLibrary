using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	/// <summary>
	/// Position Event Generation (PEG) 移動用 ViewModel
	/// </summary>
	public class PEGMoveViewModel
	{
		/// <summary>
		/// X 方向移動 Pixel
		/// </summary>
		public int XMovePixel { get; set; }

		/// <summary>
		/// X 方向移動 Loop
		/// </summary>
		public int xMoveLoop { get; set; }

		/// <summary>
		/// Y 方向移動 Pixel
		/// </summary>
		public int YMovePixel { get; set; }

		/// <summary>
		/// Y 方向移動 Loop
		/// </summary>
		public int YMoveLoop { get; set; }
	}
}
