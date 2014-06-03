using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Extensions
{
	/// <summary>
	/// 提供數學計算功能
	/// </summary>
	public static class MathExtensions
	{
		/// <summary>
		/// <para>***************</para>
		/// 把角度轉為 Halcon 徑度
		/// <para>角度, 順時針方向遞增 0 ~360</para>
		/// <para>Halcon Phi, 第一，二象限為正值，逆時針方向遞增 0 ~ PI</para>
		/// <para>第三，四象限為負值, 順時針方向遞增 0 ~ -PI</para>
		/// <para>***************</para>
		/// </summary>
		/// <param name="degree"></param>
		/// <returns></returns>
		public static double ToHalconPhi(this double degree)
		{
			double phi;
			if (degree <= 180.0 || degree == 0.0)
			{
				phi = -1 * degree * (Math.PI / 180.0);
			}
			else
			{
				phi = 2 * Math.PI - degree * (Math.PI / 180.0);
			}
			return phi;
		}

		/// <summary>
		/// 把 Halcon 徑度轉為角度
		/// <para>順時針方向計算角度</para>
		/// </summary>
		/// <param name="halconPhi"></param>
		/// <returns></returns>
		public static double HalconPhiToDegree(this double halconPhi)
		{
			var halconDegree = Math.Abs(halconPhi * 180.0 / Math.PI);
			var degree = (halconPhi <= 0) ? halconDegree : 360.0 - halconDegree;
			return degree;
		}

		/// <summary>
		/// 角度轉徑度
		/// </summary>
		/// <param name="degree"></param>
		/// <returns></returns>
		public static double DegreeToRad(this double degree)
		{
			// 1 pi rad = 180 degree, 1 rad = pi /180 degree
			return degree * (Math.PI / 180.0);
		}
	}
}
