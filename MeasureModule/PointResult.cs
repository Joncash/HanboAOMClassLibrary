using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeasureModule
{
	public class PointResult : MeasureResult
	{
		/// <summary>
		/// 點1 Row
		/// </summary>
		public HTuple Row1;

		/// <summary>
		/// 點1 Col
		/// </summary>
		public HTuple Col1;

		public bool IsParallel;

		public PointResult()
		{

		}
		public PointResult(PointResult result)
			: this(result.Row1, result.Col1, result.IsParallel)
		{

		}

		public PointResult(HTuple row1, HTuple col1, bool isParallel)
		{
			this.Row1 = new HTuple(row1);
			this.Col1 = new HTuple(col1);
			this.IsParallel = isParallel;
		}
		public PointResult(double row1, double col1, bool isParallel)
		{
			this.Row1 = new HTuple(row1);
			this.Col1 = new HTuple(col1);
			this.IsParallel = isParallel;
		}
		/// <summary>
		/// 線段的顯示 viewModel
		/// </summary>
		/// <returns></returns>
		public override ResultDisplayViewModel CreateDisplayViewModel()
		{
			HXLDCont cross = new HXLDCont();
			var size = 15;
			var angle = 0.785398;
			cross.GenCrossContourXld(this.Row1, this.Col1, size, angle);

			return new ResultDisplayViewModel()
			{
				PositionX = this.Col1,
				PositionY = this.Row1,
				ImageXLD = cross,
			};
		}
	}
}
