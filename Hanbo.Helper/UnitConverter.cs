using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	/// <summary>
	/// 單位種類
	/// 公制 (um, mm)
	/// 英制 (mil, inch)
	/// </summary>
	public enum UnitType { um, mm, mil, inch };



	public static class UnitConverter
	{
		private static double _umToMil = 0.0393700787;
		private static double _milToInch = 0.001;
		private static double _umToMm = 0.001;
		/// <summary>
		/// PixelToRealWorldValue ( Pixel 轉換為實際尺寸)
		/// </summary>
		/// <param name="pixelValue">Pixel Value</param>
		/// <param name="toUnit">轉換到什麼單位</param>
		/// <param name="resolution">解析度 ( pixels / um )</param>
		/// <param name="roundDigit">取到小數第幾位</param>
		/// <returns></returns>
		public static double PixelToRealWorldValue(double pixelValue, UnitType toUnit, double resolution, int roundDigit)
		{
			double realWorldValue = 0.0;
			double umValue = (pixelValue * resolution);
			switch (toUnit)
			{
				case UnitType.um:
					realWorldValue = Math.Round(umValue, roundDigit);
					break;
				case UnitType.mm:
					realWorldValue = Math.Round(umValue * _umToMm, roundDigit);
					break;
				case UnitType.mil:
					// 1um = 0.0393700787 mil
					realWorldValue = Math.Round(umValue * _umToMil, roundDigit);
					break;
				case UnitType.inch:
					//1 mil = 0.001 inches
					realWorldValue = Math.Round(umValue * _umToMil * _milToInch, roundDigit);
					break;
			}
			return realWorldValue;
		}

		/// <summary>
		/// PixelToRealWorldValue ( Pixel 轉換為實際尺寸)
		/// </summary>
		/// <param name="pixelValue">Pixel Value</param>
		/// <param name="toUnit">轉換到什麼單位</param>
		/// <param name="resolution">解析度 ( pixels / um )</param>
		/// <param name="roundDigit">取到小數第幾位</param>
		/// <returns></returns>
		public static double PixelToRealWorldValue(double pixelValue, string toUnit, double resolution, int roundDigit)
		{
			UnitType unitType;
			if (!Enum.TryParse(toUnit, out unitType))
			{
				unitType = UnitType.um;
			}
			return PixelToRealWorldValue(pixelValue, unitType, resolution, roundDigit);
		}

		/// <summary>
		/// 取得可以轉換的單位s
		/// </summary>
		/// <returns></returns>
		public static List<UnitViewModel> GetUnits()
		{
			return new List<UnitViewModel>() { 
				new UnitViewModel(){ Name = "um", Value = "um"},
				new UnitViewModel(){ Name = "mm", Value = "mm"},
				new UnitViewModel(){ Name = "mil", Value = "mil"},
				new UnitViewModel(){ Name = "inch", Value = "inch"},
			};
		}

		public class UnitViewModel
		{
			/// <summary>
			/// 顯示名稱
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 值
			/// </summary>
			public string Value { get; set; }
		}

	}
}
