using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Hanbo.Helper.Test
{
	public class ShapeFinderTest
	{
		[Theory]
		[InlineData(4)]
		public void GetOrderdShapeModelTest(int modelCount)
		{
			//assign
			var finder = new ShapeFinder();
			var random = new Random();
			double[] row = new double[modelCount];
			double[] col = new double[modelCount];
			double[] angle = new double[modelCount];
			double[] score = new double[modelCount];
			row = row.Select(p => random.NextDouble()).ToArray();
			col = col.Select(p => random.NextDouble()).ToArray();
			angle = angle.Select(p => random.NextDouble()).ToArray();
			score = score.Select(p => random.NextDouble()).ToArray();

			var model = new ShapeModel()
			{
				ModelId = new HalconDotNet.HTuple(),
				Row = new HalconDotNet.HTuple(row),
				Col = new HalconDotNet.HTuple(col),
				Angle = new HalconDotNet.HTuple(angle),
				Score = new HalconDotNet.HTuple(score),
			};
			var colMin = model.Col.DArr.Min();

			//act
			var orderedModel = finder.GetOrderdShapeModel(model);

			//assert
			Assert.True(colMin == orderedModel.Col[0].D);
		}

	}
}
