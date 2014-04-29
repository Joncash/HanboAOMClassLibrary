using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewROI.Interface
{
	/// <summary>
	/// 製作 ROI 時，會產生 Zoom Window, 等待使用者點擊 N 次後結束 ZoomWindow
	/// </summary>
	public interface IContinueZoom
	{
		/// <summary>
		/// 等待滑鼠點擊 N 次, 取得座標
		/// </summary>
		/// <param name="x">滑鼠座標 x</param>
		/// <param name="y">滑鼠座標 y</param>
		/// <returns></returns>
		bool WaitForClickPoints(double x, double y);

		/// <summary>
		/// 已點擊次數
		/// </summary>
		int ClickedPoints { get; set; }
	}
}
