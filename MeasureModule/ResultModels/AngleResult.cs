using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	/// <summary>
	/// 角度的量測結果
	/// </summary>
	public class AngleResult : MeasureResult
	{
		/// <summary>
		/// 徑度角
		/// </summary>
		public HTuple Angle { get; set; }

		/// <summary>
		/// 開始徑度
		/// </summary>
		public HTuple StartPhi { get; set; }

		/// <summary>
		/// 結束徑度
		/// </summary>
		public HTuple EndPhi { get; set; }

		/// <summary>
		/// 交點 Row
		/// </summary>
		public HTuple Row;

		/// <summary>
		/// 交點 Col
		/// </summary>
		public HTuple Col;


		/// <summary>
		/// 角度
		/// </summary>
		public HTuple Distance
		{
			get
			{
				HTuple value = null;
				if (this.Angle != null && this.Angle.TupleLength() > 0)
				{
					//徑度轉角度  * 180 / PI
					value = (this.Angle.D < 0) ? (this.Angle.D + Math.PI) * (180.0 / Math.PI)
						: (this.Angle.D) * (180.0 / Math.PI);
				}
				return value;
			}
			set { this.Angle = value; }
		}

		public AngleResult()
		{

		}
		public AngleResult(AngleResult result)
			: this(result.Angle, result.StartPhi, result.EndPhi)
		{
		}
		public AngleResult(HTuple angle, HTuple startPhi, HTuple endPhi)
		{
			this.Angle = new HTuple(angle);
			this.StartPhi = new HTuple(startPhi);
			this.EndPhi = new HTuple(endPhi);
		}
		public AngleResult(double angle, double startPhi, double endPhi)
		{
			this.Angle = new HTuple(angle);
			this.StartPhi = new HTuple(startPhi);
			this.EndPhi = new HTuple(endPhi);
		}
		/// <summary>
		/// 圓的顯示模式，文字顯示在圓心
		/// </summary>
		/// <returns></returns>
		public override ResultDisplayViewModel CreateDisplayViewModel()
		{
			//var dispXLD = new HXLDCont();
			//dispXLD.Dispose();
			//dispXLD.GenEmptyObj();

			//var row = 0.0;
			//var col = 0.0;
			//for (int i = 0; i < this.Row.Length; i++)
			//{
			//	dispXLD.GenCircleContourXld(this.Row[i].D, this.Col[i].D, this.Radius[i].D, 0.0, 6.28318, "positive", 1.0);
			//	row = this.Row[0].D;
			//	col = this.Col[0].D;
			//}
			//return new ResultDisplayViewModel()
			//{
			//	ImageXLD = dispXLD,
			//	PositionX = col,
			//	PositionY = row,
			//};
			return null;
		}
	}
}
