using System;
using HalconDotNet;

namespace MeasureModule
{
	public enum LineDirection { Horizontal, Vertical };

	/// <summary>
	/// Base class to have a more abstract definition of a measure result.
	/// </summary>
	public class MeasureResult
	{
		public MeasureResult() { }

		/// <summary>
		/// 顯示結果的外觀圖形
		/// </summary>
		/// <returns></returns>
		public virtual ResultDisplayViewModel CreateDisplayViewModel()
		{
			return null;
		}

		protected HXLDCont determineEdgeLine(double row, double col, double phi, double width)
		{
			double row1, row2, col1, col2;

			row1 = row;// -width * Math.Sin(phi + 0.5 * Math.PI);
			col1 = col;// +width * Math.Cos(phi + 0.5 * Math.PI);
			row2 = row - width * Math.Sin(phi + 1.5 * Math.PI);
			col2 = col + width * Math.Cos(phi + 1.5 * Math.PI);
			return determineLine(row1, col1, row2, col2);
		}
		protected void getLinePoints(double row, double col, double phi, double width, out double[] rows, out double[] cols)
		{
			double row1, row2, col1, col2;

			row1 = row;// -width * Math.Sin(phi + 0.5 * Math.PI);
			col1 = col;// +width * Math.Cos(phi + 0.5 * Math.PI);
			row2 = row - width * Math.Sin(phi + 1.5 * Math.PI);
			col2 = col + width * Math.Cos(phi + 1.5 * Math.PI);
			rows = new double[2] { row1, row2 };
			cols = new double[2] { col1, col2 };
		}

		protected HXLDCont determineEdgeLine(double row, double col, double phi, double width, out double centerX, out double centerY)
		{
			double row1, row2, col1, col2;


			row1 = row;// -width * Math.Sin(phi + 0.5 * Math.PI);
			col1 = col;// +width * Math.Cos(phi + 0.5 * Math.PI);
			row2 = row - width * Math.Sin(phi + 1.5 * Math.PI);
			col2 = col + width * Math.Cos(phi + 1.5 * Math.PI);

			centerX = (col1 + col2) / 2.0;
			centerY = (row1 + row2) / 2.0;
			return determineLine(row1, col1, row2, col2);
		}
		protected HXLDCont determineLine(double row1, double col1, double row2, double col2)
		{
			HXLDCont edge = new HXLDCont();
			HTuple rows, cols;

			rows = new HTuple(new double[] { row1, row2 });
			cols = new HTuple(new double[] { col1, col2 });

			edge.GenContourPolygonXld(rows, cols);
			return edge;
		}
	}

	/****************************************************************/
	/****************************************************************/

	/// <summary>
	/// Measure result class containing data obtained from the HALCON measure
	/// operator for single-edge measurement
	/// </summary>
	public class EdgeResult : MeasureResult
	{
		/// <summary>Row coordinate of extracted edges.</summary>
		public HTuple rowEdge;

		/// <summary>Column coordinate of extracted edges.</summary>
		public HTuple colEdge;

		/// <summary>Amplitude of the extracted edges (with sign).</summary>
		public HTuple amplitude;

		/// <summary>Distance between consecutive edges.</summary>
		public HTuple distance;

		/// <summary>Creates empty instance.</summary>
		public EdgeResult() { }

		/// <summary>
		/// Creates an edge result instance containing data from
		/// the provided result value (deep copy).
		/// </summary>
		public EdgeResult(EdgeResult result)
			: this(result.rowEdge, result.colEdge,
				  result.amplitude, result.distance)
		{
		}

		/// <summary>
		/// Creates an edge result instance using the passed values.
		/// </summary>
		public EdgeResult(HTuple Nrow, HTuple Ncol,
						  HTuple Nampl, HTuple Ndist)
		{
			rowEdge = new HTuple(Nrow);
			colEdge = new HTuple(Ncol);
			amplitude = new HTuple(Nampl);
			distance = new HTuple(Ndist);
		}

		/// <summary>
		/// Creates an edge result instance using the passed values.
		/// </summary>
		public EdgeResult(double Nrow, double Ncol,
						  double Nampl, double Ndist)
		{
			rowEdge = new HTuple(Nrow);
			colEdge = new HTuple(Ncol);
			amplitude = new HTuple(Nampl);
			distance = new HTuple(Ndist);
		}

	}//end of class EdgeResult


	/****************************************************************/
	/****************************************************************/

	/// <summary>
	/// Measure result class containing data obtained from the HALCON measure
	/// operator for edge pair measurement
	/// </summary>
	public class PairResult : MeasureResult
	{

		/// <summary>
		/// Row coordinate of first extracted edges of a pair.
		/// </summary>
		public HTuple rowEdgeFirst;
		/// <summary>
		/// Column coordinate of first extracted edges of a pair.
		/// </summary>
		public HTuple colEdgeFirst;
		/// <summary>
		/// Row coordinate of second extracted edges of a pair.
		/// </summary>
		public HTuple rowEdgeSecond;
		/// <summary>
		/// Column coordinate of second extracted edges of a pair.
		/// </summary>
		public HTuple colEdgeSecond;
		/// <summary>Amplitude of the first extracted edges of a pair (with sign).</summary>
		public HTuple amplitudeFirst;
		/// <summary>Amplitude of the second extracted edges of a pair (with sign).</summary>
		public HTuple amplitudeSecond;
		/// <summary>Distance between edges of a pair.</summary>
		public HTuple intraDistance;
		/// <summary>Distance between consecutive edge pairs</summary>
		public HTuple interDistance;


		/// <summary>Creates empty instance.</summary>
		public PairResult() { }

		/// <summary>
		/// Creates an edge result instance containing data from
		/// the provided result value (deep copy).
		/// </summary>
		public PairResult(PairResult result)
			: this(result.rowEdgeFirst, result.colEdgeFirst,
				  result.rowEdgeSecond, result.colEdgeSecond,
				  result.amplitudeFirst, result.amplitudeSecond,
				  result.intraDistance, result.interDistance)
		{
		}

		/// <summary>
		/// Creates an edge result instance using the passed values.
		/// </summary>
		public PairResult(HTuple Nrow1, HTuple Ncol1,
						  HTuple Nrow2, HTuple Ncol2,
						  HTuple Nampl1, HTuple Nampl2,
						  HTuple Ndist, HTuple Nwidth)
		{
			rowEdgeFirst = new HTuple(Nrow1);
			colEdgeFirst = new HTuple(Ncol1);
			rowEdgeSecond = new HTuple(Nrow2);
			colEdgeSecond = new HTuple(Ncol2);
			amplitudeFirst = new HTuple(Nampl1);
			amplitudeSecond = new HTuple(Nampl2);
			intraDistance = new HTuple(Ndist);
			interDistance = new HTuple(Nwidth);
		}

		/// <summary>
		/// Creates an edge result instance using the passed values.
		/// </summary>
		public PairResult(double Nrow1, double Ncol1,
						  double Nrow2, double Ncol2,
						  double Nampl1, double Nampl2,
						  double Ndist, double Nwidth)
		{
			rowEdgeFirst = new HTuple(Nrow1);
			colEdgeFirst = new HTuple(Ncol1);
			rowEdgeSecond = new HTuple(Nrow2);
			colEdgeSecond = new HTuple(Ncol2);
			amplitudeFirst = new HTuple(Nampl1);
			amplitudeSecond = new HTuple(Nampl2);
			intraDistance = new HTuple(Ndist);
			interDistance = new HTuple(Nwidth);
		}

	}//end of class PairResult
}//end of namespace
