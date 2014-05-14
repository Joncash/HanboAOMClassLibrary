using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	/// <summary>
	/// FitCircle 圓的結果
	/// </summary>
	public class CircleResult : MeasureResult
	{
		#region 公開屬性 (Public Properties)
		/// <summary>
		/// Row coordinate of the center of the circle
		/// </summary>
		public HTuple Row;

		/// <summary>
		/// Column coordinate of the center of the circle
		/// </summary>
		public HTuple Col;

		/// <summary>
		/// Radius of circle
		/// </summary>
		public HTuple Radius;

		/// <summary>
		/// Angle of the start point [rad]. 
		/// </summary>
		public HTuple StartPhi;

		/// <summary>
		/// Angle of the end point [rad]. 
		/// </summary>
		public HTuple EndPhi;

		/// <summary>
		/// Point order along the boundary.
		/// List of values: 'positive', 'negative
		/// </summary>
		public HTuple PointOrder;
		#endregion

		#region 建構子
		public CircleResult()
		{

		}
		public CircleResult(CircleResult result)
			: this(result.Row, result.Col, result.Radius, result.StartPhi, result.EndPhi, result.PointOrder)
		{

		}
		public CircleResult(HTuple row, HTuple col, HTuple radius, HTuple startPhi, HTuple EndPhi, HTuple pointOrder)
		{
			this.Row = new HTuple(row);
			this.Col = new HTuple(col);
			this.Radius = new HTuple(radius);
			this.StartPhi = new HTuple(startPhi);
			this.EndPhi = new HTuple(EndPhi);
			this.PointOrder = new HTuple(pointOrder);
		}
		public CircleResult(double row, double col, double radius, double startPhi, double EndPhi, string pointOrder)
		{
			this.Row = new HTuple(row);
			this.Col = new HTuple(col);
			this.Radius = new HTuple(radius);
			this.StartPhi = new HTuple(startPhi);
			this.EndPhi = new HTuple(EndPhi);
			this.PointOrder = new HTuple(pointOrder);
		}
		#endregion

		/// <summary>
		/// 圓的顯示模式，文字顯示在圓心
		/// </summary>
		/// <returns></returns>
		public override ResultDisplayViewModel CreateDisplayViewModel()
		{
			var dispXLD = new HXLDCont();
			dispXLD.Dispose();
			dispXLD.GenEmptyObj();

			var row = 0.0;
			var col = 0.0;
			for (int i = 0; i < this.Row.Length; i++)
			{
				var radius = this.Radius[i].D;
				dispXLD.GenCircleContourXld(this.Row[i].D, this.Col[i].D, radius, 0.0, 6.28318, "positive", 1.0);
				row = this.Row[0].D;
				col = this.Col[0].D;
			}
			return new ResultDisplayViewModel()
			{
				ImageXLD = dispXLD,
				PositionX = col,
				PositionY = row,
			};
		}
	}
}
