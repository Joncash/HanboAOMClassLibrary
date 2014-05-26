using Hanbo.Geometry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hanbo.Extensions;

namespace Hanbo.Geometry
{
	/// <summary>
	/// 歐幾里德-幾何運算
	/// </summary>
	public static class GeoOperatorSet
	{
		/// <summary>
		/// 資料轉為幾何座標點
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <returns></returns>
		public static List<GeoPoint> ToGeoPoints(double[] rows, double[] cols)
		{
			return rows.Zip(cols, (row, col) => new GeoPoint() { Row = row, Col = col }).ToList();
		}
		/// <summary>
		/// <para>*************************</para>
		/// 給定 3 點，找圓心座標
		/// <para>*************************</para>
		/// </summary>
		/// <param name="rows">點 y 方向座標</param>
		/// <param name="cols">點 x 方向座標</param>
		/// <param name="centerRow">圓心 y 座標</param>
		/// <param name="centerCol">圓心 x 座標</param>
		public static void CalculateCircleCenter(double[] rows, double[] cols
			, out double centerRow, out double centerCol)
		{
			centerRow = centerCol = -1.0;
			var geoPoints = ToGeoPoints(rows, cols);
			if (geoPoints.Count > 2)
			{
				var pA = geoPoints[0];
				var pB = geoPoints[1];
				var pC = geoPoints[2];
				CalculateCircleCenter(pA, pB, pC, out centerRow, out centerCol);
			}
		}

		/// <summary>
		/// <para>**************</para>
		/// 給定 3 點，找圓心座標及半徑
		/// <para>**************</para>
		/// </summary>
		/// <param name="rows">點 y 方向座標</param>
		/// <param name="cols">點 x 方向座標</param>
		/// <param name="centerRow">圓心 y 座標</param>
		/// <param name="centerCol">圓心 x 座標</param>
		/// <param name="radius">半徑</param>
		public static void CalculateCircleCenter(double[] rows, double[] cols
			, out double centerRow, out double centerCol, out double radius)
		{
			radius = -1;
			CalculateCircleCenter(rows, cols, out centerRow, out centerCol);
			if (centerRow > 0 && centerCol > 0 && rows.Length > 0 && cols.Length > 0)
			{
				radius = CalculateDistance(centerRow, centerCol, rows[0], cols[0]);
			}
		}

		/// <summary>
		/// 給定 3 點，找圓心座標
		/// </summary>
		/// <param name="pA"></param>
		/// <param name="pB"></param>
		/// <param name="pC"></param>
		/// <param name="centerRow"></param>
		/// <param name="centerCol"></param>
		public static void CalculateCircleCenter(GeoPoint pA, GeoPoint pB, GeoPoint pC
			, out double centerRow, out double centerCol)
		{
			centerRow = centerCol = -1;
			if (pA != null && pB != null && pC != null)
			{
				var a1 = pA.Col;
				var b1 = pA.Row;
				var a2 = pB.Col;
				var b2 = pB.Row;
				var a3 = pC.Col;
				var b3 = pC.Row;
				var a1s = Math.Pow(a1, 2);
				var a2s = Math.Pow(a2, 2);
				var a3s = Math.Pow(a3, 2);
				var b1s = Math.Pow(b1, 2);
				var b2s = Math.Pow(b2, 2);
				var b3s = Math.Pow(b3, 2);

				centerCol = (a1s * b2
						- a1s * b3
						+ b1 * b3s
						- b1 * a2s
						+ b3 * a2s
						- b3s * b2
						+ b3 * b2s
						- b1 * b2s
						+ b1 * a3s
						- b1s * b3
						- a3s * b2
						+ b1s * b2) /
						(2 * (a1 * b2 + a3 * b1 - a3 * b2 - a1 * b3 - a2 * b1 + a2 * b3));

				centerRow = -0.5 * (-1 * a1 * a2s
			   + a2 * b1s
			   - a1 * b2s
			   - a3 * a1s
			   - a2 * b3s
			   - a3 * b1s
			   + a3 * a2s
			   + a1 * b3s
			   + a3 * b2s
			   + a1 * a3s
			   - a2 * a3s
			   + a2 * a1s) /
			   (a1 * b2 + a3 * b1 - a3 * b2 - a1 * b3 - a2 * b1 + a2 * b3);
			}
		}

		/// <summary>
		/// 給定 3 點，找圓心座標及半徑
		/// </summary>
		/// <param name="pA"></param>
		/// <param name="pB"></param>
		/// <param name="pC"></param>
		/// <param name="centerRow"></param>
		/// <param name="centerCol"></param>
		/// <param name="radius"></param>
		public static void CalculateCircleCenter(GeoPoint pA, GeoPoint pB, GeoPoint pC
			, out double centerRow, out double centerCol, out double radius)
		{
			radius = -1;
			CalculateCircleCenter(pA, pB, pC, out centerRow, out centerCol);
			if (pA != null && pA.Row > 0 && pA.Col > 0 && centerRow > 0 && centerCol > 0)
			{
				radius = CalculateDistance(centerRow, centerCol, pA.Row, pA.Col);
			}
		}


		/// <summary>
		/// 計算 2 點距離
		/// </summary>
		/// <param name="rowBegin"></param>
		/// <param name="colBegin"></param>
		/// <param name="rowEnd"></param>
		/// <param name="colEnd"></param>
		/// <returns></returns>
		public static double CalculateDistance(double rowBegin, double colBegin
			, double rowEnd, double colEnd)
		{
			/*
			 distance = sqrt(dx^2 + dy^2)
			 */
			var dx = colBegin - colEnd;
			var dy = rowBegin - rowEnd;
			return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
		}

		/// <summary>
		/// 取得旋轉後的位置 (笛卡爾座標)
		/// <para></para>
		/// </summary>
		/// <param name="x">點座標 x</param>
		/// <param name="y">點座標 y</param>
		/// <param name="degree">旋轉角度</param>
		/// <returns></returns>
		public static GeoPoint GetRotationPosition(double x, double y, double degree)
		{
			var rad = degree.DegreeToRad();
			var newX = x * Math.Cos(rad) - y * Math.Sin(rad);
			var newY = y * Math.Cos(rad) + x * Math.Sin(rad);
			return new GeoPoint() { Row = newY, Col = newX };
		}
		/// <summary>
		/// 取得旋轉後的位置 (笛卡爾座標)
		/// </summary>
		/// <param name="x">點座標 x</param>
		/// <param name="y">點座標 y</param>
		/// <param name="degree">旋轉角度</param>
		/// <param name="newX">旋轉後的點座標 x</param>
		/// <param name="newY">旋轉後的點座標 y</param>
		public static void GetRotationPosition(double x, double y, double degree, out double newX, out double newY)
		{
			var nP = GetRotationPosition(x, y, degree);
			newX = nP.Col;
			newY = nP.Row;
		}

	}
}
