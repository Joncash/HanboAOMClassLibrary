using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Hanbo.AOM.Configuration
{
	/// <summary>
	/// 顏色轉換管理
	/// </summary>
	public static class ColorMM
	{
		/// <summary>
		/// 將顏色字串解析為顏色 (System.Drawing.Color)
		/// </summary>
		/// <param name="fromNameOrHtmlString">顏色名稱 or 16 進位顏色字串</param>
		/// <param name="parseColor">解析後的顏色</param>
		/// <returns>成功 or 失敗</returns>
		public static bool TryParseColor(string fromNameOrHtmlString, out Color parseColor)
		{
			bool parseSuccess = true;
			parseColor = Color.Empty;
			var fromNameColor = Color.FromName(fromNameOrHtmlString);
			if (fromNameColor.IsKnownColor)
			{
				parseColor = fromNameColor;
			}
			else
			{
				try
				{
					parseColor = ColorTranslator.FromHtml("#" + fromNameOrHtmlString);
				}
				catch
				{
					parseSuccess = false;
				}
			}
			return parseSuccess;
		}
	}
}
