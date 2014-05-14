using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	/// <summary>
	/// 兩個線段，計算距離的結果
	/// </summary>
	public class DistanceResult : MeasureResult
	{
		public HTuple FirstRowBegin;
		public HTuple FirstColBegin;
		public HTuple FirstRowEnd;
		public HTuple FirstColEnd;

		public HTuple SecondRowBegin;
		public HTuple SecondColBegin;
		public HTuple SecondRowEnd;
		public HTuple SecondColEnd;

		public HTuple Angle;
		public HTuple Distance;

		public LineDirection Direction = LineDirection.Horizontal;

		#region 建構子
		public DistanceResult()
		{
		}
		public DistanceResult(DistanceResult result)
			: this(result.FirstRowBegin, result.FirstColBegin, result.FirstRowEnd, result.FirstColEnd, result.SecondRowBegin, result.SecondColBegin, result.SecondRowEnd, result.SecondColEnd, result.Angle, result.Distance)
		{
		}
		public DistanceResult(double rowBegin1, double colBegin1, double rowEnd1, double colEnd1, double rowBegin2, double colBegin2, double rowEnd2, double colEnd2, double angle, double distance)
		{
			this.FirstRowBegin = new HTuple(rowBegin1);
			this.FirstColBegin = new HTuple(colBegin1);
			this.FirstRowEnd = new HTuple(rowEnd1);
			this.FirstColEnd = new HTuple(colEnd1);
			this.SecondRowBegin = new HTuple(rowBegin2);
			this.SecondColBegin = new HTuple(colBegin2);
			this.SecondRowEnd = new HTuple(rowEnd2);
			this.SecondColEnd = new HTuple(colEnd2);
			this.Angle = new HTuple(angle);
			this.Distance = new HTuple(distance);
		}
		public DistanceResult(HTuple rowBegin1, HTuple colBegin1, HTuple rowEnd1, HTuple colEnd1, HTuple rowBegin2, HTuple colBegin2, HTuple rowEnd2, HTuple colEnd2, HTuple angle, HTuple distance)
		{
			this.FirstRowBegin = new HTuple(rowBegin1);
			this.FirstColBegin = new HTuple(colBegin1);
			this.FirstRowEnd = new HTuple(rowEnd1);
			this.FirstColEnd = new HTuple(colEnd1);
			this.SecondRowBegin = new HTuple(rowBegin2);
			this.SecondColBegin = new HTuple(colBegin2);
			this.SecondRowEnd = new HTuple(rowEnd2);
			this.SecondColEnd = new HTuple(colEnd2);
			this.Angle = new HTuple(angle);
			this.Distance = new HTuple(distance);
		}
		#endregion


		public override ResultDisplayViewModel CreateDisplayViewModel()
		{
			double row = FirstRowEnd.D;
			double col = FirstColEnd.D;
			double centerX = 0.0;
			double centerY = 0.0;
			double[] rows;
			double[] cols;

			var phi = (Direction == LineDirection.Horizontal) ? this.Angle.D + 1.57 : this.Angle.D;
			getLinePoints(row, col, phi, this.Distance, out rows, out cols);
			var displayXLD = determineEdgeLine(row, col, phi, this.Distance, out centerX, out centerY);

			return new ResultDisplayViewModel()
			{
				PositionX = centerX,
				PositionY = centerY,
				ImageXLD = displayXLD,
				FirstArrowX = cols[0],
				FirstArrowY = rows[0],
				SecArrowX = cols[1],
				SecArrowY = rows[1],
			};
		}

		[Obsolete]
		private HXLDCont getDispXLD()
		{
			double row = 0.0;
			double col = 0.0;

			#region Testing
			//縱向 or 橫向
			var dx = Math.Abs(FirstColBegin.D - FirstColEnd.D);
			var dy = Math.Abs(FirstRowBegin.D - FirstRowEnd.D);
			if (dx < dy)
			{
				////橫向
				//if (FirstColBegin.D <= SecondColBegin.D && FirstColEnd.D >= SecondColEnd.D)
				//{
				//	row = SecondRowBegin.D;
				//	col = SecondColBegin.D;
				//}
				//else if (FirstColBegin.D >= SecondColBegin.D && FirstColEnd.D <= SecondColEnd.D)
				//{
				//	row = FirstRowBegin.D;
				//	col = FirstColBegin.D;
				//}
				//else if (FirstColBegin.D > SecondColBegin.D && FirstColEnd.D > SecondColEnd.D)
				//{
				//	row = SecondRowEnd.D;
				//	col = SecondColEnd.D;
				//}
				//else if (FirstColBegin.D < SecondColBegin.D && FirstColEnd.D < SecondColEnd.D)
				//{
				//	row = SecondRowBegin.D;
				//	col = SecondColBegin.D;
				//}
				//else if (FirstColEnd.D < SecondColBegin.D)
				//{
				//	row = FirstRowEnd.D;
				//	col = FirstColEnd.D;
				//}
				//else if (SecondColEnd.D < FirstColBegin.D)
				//{
				//	row = FirstRowBegin.D;
				//	col = FirstColBegin.D;
				//}

			}
			else
			{
				//縱向
			}
			#endregion

			row = FirstRowEnd.D;
			col = FirstColEnd.D;
			var phi = (Direction == LineDirection.Horizontal) ? this.Angle.D + 1.57 : this.Angle.D;
			return determineEdgeLine(row, col, phi, this.Distance);
			//return determineEdgeLine(row, col, this.Angle.D + 1.57, this.Distance);
		}

	}

}
