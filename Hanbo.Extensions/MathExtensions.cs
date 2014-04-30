using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Extensions
{
	public static class MathExtensions
	{
		/// <summary>
		/// 把角度轉為 Halcon 徑度
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
		/// </summary>
		/// <param name="phi"></param>
		/// <returns></returns>
		public static double HalconPhiToDegree(this double phi)
		{
			var halconDegree = Math.Abs(phi * 180.0 / Math.PI);
			var degree = (phi <= 0) ? halconDegree : 360.0 - halconDegree;
			return degree;
		}
	}
}
