using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	/// <summary>
	/// 線段結果
	/// 兩點距離
	/// </summary>
	public class LineResult : MeasureResult
	{
		#region 公開屬性

		/// <summary>
		/// 點1 Row
		/// </summary>
		public HTuple Row1;

		/// <summary>
		/// 點1 Col
		/// </summary>
		public HTuple Col1;
		public HTuple Row2;
		public HTuple Col2;
		public HTuple Distance;

		#endregion

		#region 建構子

		public LineResult()
		{

		}
		public LineResult(LineResult result)
			: this(result.Row1, result.Col1, result.Row2, result.Col2, result.Distance)
		{
		}
		public LineResult(HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple distance)
		{
			this.Row1 = new HTuple(row1);
			this.Col1 = new HTuple(col1);
			this.Row2 = new HTuple(row2);
			this.Col2 = new HTuple(col2);
			this.Distance = new HTuple(distance);
		}
		public LineResult(double row1, double col1, double row2, double col2, double distance)
		{
			this.Row1 = new HTuple(row1);
			this.Col1 = new HTuple(col1);
			this.Row2 = new HTuple(row2);
			this.Col2 = new HTuple(col2);
			this.Distance = new HTuple(distance);
		}
		#endregion

		/// <summary>
		/// 線段的顯示 viewModel
		/// </summary>
		/// <returns></returns>
		public override ResultDisplayViewModel CreateDisplayViewModel()
		{
			HXLDCont edge = new HXLDCont();
			HTuple rows, cols;
			double centerRow = 0.0, centerCol = 0.0;
			double f_ArrowX = 0.0, f_ArrowY = 0.0, s_ArrowX = 0.0, s_ArrowY = 0.0;
			if (Row1.TupleLength() > 0 && Row2.TupleLength() > 0)
			{
				rows = new HTuple(new double[] { Row1, Row2 });
				cols = new HTuple(new double[] { Col1, Col2 });
				centerRow = (Row1.D + Row2.D) / 2.0;
				centerCol = (Col1.D + Col2.D) / 2.0;
				f_ArrowX = Col1;
				f_ArrowY = Row1;
				s_ArrowX = Col2;
				s_ArrowY = Row2;
				edge.GenContourPolygonXld(rows, cols);
			}
			return new ResultDisplayViewModel()
			{
				PositionX = centerCol,
				PositionY = centerRow,
				ImageXLD = edge,
				FirstArrowX = f_ArrowX,
				FirstArrowY = f_ArrowY,
				SecArrowX = s_ArrowX,
				SecArrowY = s_ArrowY,
			};
		}
	}
}
