using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Hanbo.Helper
{
	public static class CameraSystem
	{
		private static int _RoundDigit;
		private static double _Resolution;
		private static XDocument _settingDoc;
		private static string _settingDocFilepath = @"Configuration\Mahr.xml";
		static CameraSystem()
		{
			init();
		}
		private static void init()
		{
			try
			{
				_settingDoc = XDocument.Load(_settingDocFilepath);
				_RoundDigit = GetRoundDigit();
				_Resolution = GetCurrentResolution();
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(String.Format("載入 「{0}」 發生錯誤", _settingDocFilepath));
				Hanbo.Log.LogManager.Error(ex);
			}
		}

		/// <summary>
		/// 取得系統校正值, 單位: pixel/um
		/// </summary>
		/// <returns></returns>
		public static double GetCurrentResolution()
		{
			double value = 0.0;
			if (_settingDoc != null)
			{
				var avgElement = _settingDoc.Root.Elements("Avg").SingleOrDefault();
				if (avgElement != null)
				{
					value = Convert.ToDouble(avgElement.Attribute("value").Value);
					_Resolution = value;
				}
			}
			return value;
		}

		public static bool ApplyResolution(double value)
		{
			var success = false;
			try
			{
				if (_settingDoc != null)
				{
					var avgElement = _settingDoc.Root.Elements("Avg").SingleOrDefault();
					if (avgElement != null)
					{
						avgElement.Attribute("value").SetValue(value);
						_settingDoc.Save(_settingDocFilepath);
						success = true;
					}
				}
			}
			catch (IOException ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
			return success;
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
