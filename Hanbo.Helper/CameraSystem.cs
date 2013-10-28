using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Hanbo.Helper
{
	public static class CameraSystem
	{
		private static int _RoundDigit = GetRoundDigit();
		private static double _Resolution = GetCurrentResolution();

		/// <summary>
		/// 取得系統校正值, 單位: pixel/um
		/// </summary>
		/// <returns></returns>
		public static double GetCurrentResolution()
		{
			var fpath = @"Configuration\Mahr.xml";
			var xDoc = XDocument.Load(fpath);
			double value = 0.0;
			var avgElement = xDoc.Root.Elements("Avg").SingleOrDefault();
			if (avgElement != null)
			{
				value = Convert.ToDouble(avgElement.Attribute("value").Value);
			}
			return value;
		}

		/// <summary>
		/// 取得系統設定的進位數
		/// Default = 5
		/// </summary>
		/// <returns></returns>
		public static int GetRoundDigit()
		{
			var defaultValue = "5";
			var roundDigit = ConfigurationManager.AppSettings["RoundDigit"] ?? defaultValue;
			int roundDigitValue;
			Int32.TryParse(roundDigit, out roundDigitValue);
			return roundDigitValue;
		}
		public static double ToRealWorldValue(double value)
		{
			var Resolution = GetCurrentResolution();
			var RoundDigit = GetRoundDigit();
			return Math.Round((value * Resolution) / 1000.0, RoundDigit);
		}

		/// <summary>
		/// 像素轉換成實際值
		/// </summary>
		/// <param name="value"></param>
		/// <param name="exportUnit"></param>
		/// <returns></returns>
		public static double PixelToRealWorld(double value, string exportUnit)
		{
			return UnitConverter.PixelToRealWorldValue(value, exportUnit, _Resolution, _RoundDigit);
		}

	}
}
