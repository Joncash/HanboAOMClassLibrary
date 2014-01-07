using Hanbo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Hanbo.Helper.Test
{
	public class ExportHelperTest
	{
		[Theory]
		[InlineData("12.480", "12.500", "12.600", "12.400", " -")]
		[InlineData("12.454", "12.500", "12.600", "12.400", " --")]
		[InlineData("15.029", "15.000", "15.100", "14.900", " ++")]
		[InlineData("15.029", "15.000", "15.100", "", "")]
		[InlineData("15.029", "15.000", "", "14.900", "")]
		[InlineData("15.029", "", "15.100", "14.900", "")]
		public void GetEvaluationTest(string measure, string standard, string max, string min, string expected)
		{
			//assign
			var model = new MeasureExportModel()
			{
				MeasureName = DateTime.Now.ToString(@"mmss"),
				MeasureValue = measure,
				Max = max,
				Min = min,
				Standard = standard,
				Deviation = "",
				Symbol = "",
			};

			//act
			var evaluation = ExportHelper.GetEvaluation(model);

			//assert
			Assert.True(evaluation == expected);
		}

		[Theory]
		[InlineData("12.480", "12.500", "12.600", "12.400", " -")]
		public void MahrExportTest(string measure, string standard, string max, string min, string expected)
		{
			//assign
			var fpath = @"D:\2.txt";
			var data = new List<MeasureExportModel>();
			var model = new MeasureExportModel()
			{
				MeasureName = DateTime.Now.ToString(@"mmss"),
				MeasureValue = measure,
				Max = max,
				Min = min,
				Standard = standard,
				Deviation = "",
				Symbol = "",
			};
			var model2 = new MeasureExportModel()
			{
				//"15.029", "15.000", "15.100", "14.900
				MeasureName = DateTime.Now.ToString(@"mmss") + "-1",
				MeasureValue = "15.029",
				Max = "15.1",
				Min = "14.9",
				Standard = "15",
				Deviation = "",
				Symbol = "",
			};
			data.Add(model);
			data.Add(model2);
			//act
			ExportHelper.MahrExport(data, fpath);

			//assert
			Assert.True(1 == 1);
		}
	}
}
