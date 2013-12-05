using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViewROI;
using Xunit;
using Xunit.Extensions;

namespace MeasureModule.Test
{
	/// <summary>
	/// 測試距離計算功能
	/// </summary>
	public class DistanceHelperTest
	{
		[Theory]
		[InlineData(@"Data/TestCaculateDistance.xml")]
		public void TestCaculateDistance(string testData)
		{
			//Assign
			var xDoc = XDocument.Load(testData);
			var testCases = xDoc.Root.Elements("Case")
							.Select(p => new
							{
								Models = p.Elements("MeasureModel").Select(model => new MeasureViewModel()
								{
									Row1 = model.Element("Row1").Value,
									Col1 = model.Element("Col1").Value,
									Row2 = model.Element("Row2").Value,
									Col2 = model.Element("Col2").Value,
									GeoType = (MeasureType)Enum.Parse(typeof(MeasureType), model.Element("GeoType").Value),
								}).ToList(),
								ExpectedValue = p.Element("Expected").Value
							}).ToList();

			foreach (var testCase in testCases)
			{
				var f1 = translateToDoubleValue(testCase.Models[0]);
				var f2 = translateToDoubleValue(testCase.Models[1]);

				//Act
				var result = DistanceHelper.CaculateDistance(f1, f2);

				//Assert
				Assert.Equal(testCase.ExpectedValue, result.Distance.ToString());
			}
		}

		private MeasureViewModel translateToDoubleValue(MeasureViewModel model)
		{
			var newModel = new MeasureViewModel();
			double rowBegin, colBegin, rowEnd, colEnd;
			if (Double.TryParse(model.Row1, out rowBegin))
				newModel.Row1 = new HTuple(rowBegin);

			if (Double.TryParse(model.Row2, out rowEnd))
				newModel.Row2 = new HTuple(rowEnd);

			if (Double.TryParse(model.Col1, out colBegin))
				newModel.Col1 = new HTuple(colBegin);

			if (Double.TryParse(model.Col2, out colEnd))
				newModel.Col2 = new HTuple(colEnd);

			newModel.GeoType = model.GeoType;
			return newModel;
		}

		[Theory]
		[InlineData(1, 2, 3, 4, 0)]
		[InlineData(1.0, 2.0, 3.0, 4.0, 0)]
		[InlineData(null, 2.0, 3.0, 4.0, -1)]
		[InlineData(1.0, null, 3.0, 4.0, -1)]
		[InlineData(1.0, 2.0, null, 4.0, -1)]
		[InlineData(1.0, 2.0, 3.0, null, -1)]
		[InlineData(-1, 2.0, 3.0, 4.0, -1)]
		[InlineData(1.0, -1, 3.0, 4.0, -1)]
		[InlineData(1.0, 2.0, -1, 4.0, -1)]
		[InlineData(1.0, 2.0, 3.0, -1, -1)]
		public void TestPointToPoint(object row1, object col1, object row2, object col2, double expectedValue)
		{
			//assign
			var pointA = new MeasureViewModel();
			if (row1 != null) pointA.Row1 = new HTuple(row1);
			if (col1 != null) pointA.Col1 = new HTuple(col1);
			pointA.GeoType = MeasureType.Point;

			var pointB = new MeasureViewModel();
			if (row2 != null) pointB.Row1 = new HTuple(row2);
			if (col2 != null) pointB.Col1 = new HTuple(col2);
			pointB.GeoType = MeasureType.Point;

			//act
			double distance = DistanceHelper.PointToPoint(pointA, pointB);

			//assert
			Assert.True(distance >= expectedValue);
		}

		[Theory]
		[InlineData(1, 2, 0, 0, 2, 2, 0)]
		[InlineData(null, 2, 0, 0, 2, 2, -1)]
		[InlineData(1, null, 0, 0, 2, 2, -1)]
		[InlineData(1, 2, null, 0, 2, 2, -1)]
		[InlineData(1, 2, 0, null, 2, 2, -1)]
		[InlineData(1, 2, 0, 0, null, 2, -1)]
		[InlineData(1, 2, 0, 0, 2, null, -1)]
		[InlineData(-1, 2, 0, 0, 2, 2, -1)]
		[InlineData(1, -1, 0, 0, 2, 2, -1)]
		[InlineData(1, 2, -1, 0, 2, 2, -1)]
		[InlineData(1, 2, 0, -1, 2, 2, -1)]
		[InlineData(1, 2, 0, 0, -1, 2, -1)]
		public void TestPointToLine(object pointRow, object pointCol
									, object lineRowBegin, object lineColBegin, object lineRowEnd, object lineColEnd
									, double expectedValue)
		{
			//assign
			var pointModel = new MeasureViewModel();
			if (pointRow != null) pointModel.Row1 = new HTuple(pointRow);
			if (pointCol != null) pointModel.Col1 = new HTuple(pointCol);
			pointModel.GeoType = MeasureType.Point;

			var lineModel = new MeasureViewModel();
			if (lineRowBegin != null) lineModel.Row1 = new HTuple(lineRowBegin);
			if (lineColBegin != null) lineModel.Col1 = new HTuple(lineColBegin);
			if (lineRowEnd != null) lineModel.Row2 = new HTuple(lineRowEnd);
			if (lineColEnd != null) lineModel.Col2 = new HTuple(lineColEnd);
			lineModel.GeoType = MeasureType.FitLine;

			//act
			var distance = DistanceHelper.PointToLine(pointModel, lineModel);

			//assert
			Assert.True(distance >= expectedValue);
		}

		[Theory]
		[InlineData(1, 1, 2, 2, 3, 3, 4, 4, 0)]
		[InlineData(null, 1, 2, 2, 3, 3, 4, 4, -1)]
		[InlineData(1, null, 2, 2, 3, 3, 4, 4, -1)]
		[InlineData(1, 1, null, 2, 3, 3, 4, 4, -1)]
		[InlineData(1, 1, 2, null, 3, 3, 4, 4, -1)]
		[InlineData(1, 1, 2, 2, null, 3, 4, 4, -1)]
		[InlineData(1, 1, 2, 2, 3, null, 4, 4, -1)]
		[InlineData(1, 1, 2, 2, 3, 3, null, 4, -1)]
		[InlineData(1, 1, 2, 2, 3, 3, 4, null, -1)]
		[InlineData(-1, 1, 2, 2, 3, 3, 4, 4, -1)]
		[InlineData(1, 1, -1, 2, 3, 3, 4, 4, -1)]
		[InlineData(1, 1, 2, 2, -1, 3, 4, 4, -1)]
		[InlineData(1, 1, 2, 2, 3, 3, -1, 4, -1)]
		public void TestLineToLine(object line1RowBegin, object line1ColBegin, object line1RowEnd, object line1ColEnd
								, object line2RowBegin, object line2ColBegin, object line2RowEnd, object line2ColEnd
								, double expectedValue)
		{
			//assign
			var line1Model = new MeasureViewModel();
			if (line1RowBegin != null) line1Model.Row1 = new HTuple(line1RowBegin);
			if (line1ColBegin != null) line1Model.Col1 = new HTuple(line1ColBegin);
			if (line1RowEnd != null) line1Model.Row2 = new HTuple(line1RowEnd);
			if (line1ColEnd != null) line1Model.Col2 = new HTuple(line1ColEnd);
			line1Model.GeoType = MeasureType.Line;

			var line2Model = new MeasureViewModel();
			if (line2RowBegin != null) line2Model.Row1 = new HTuple(line2RowBegin);
			if (line2ColBegin != null) line2Model.Col1 = new HTuple(line2ColBegin);
			if (line2RowEnd != null) line2Model.Row2 = new HTuple(line2RowEnd);
			if (line2ColEnd != null) line2Model.Col2 = new HTuple(line2ColEnd);
			line2Model.GeoType = MeasureType.Line;

			//act
			var distance = DistanceHelper.LineToLine(line1Model, line2Model);

			//assert
			Assert.True(distance >= expectedValue);
		}
	}
}
