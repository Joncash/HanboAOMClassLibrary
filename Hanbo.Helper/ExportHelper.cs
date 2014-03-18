using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hanbo.Models;
using System.IO;
namespace Hanbo.Helper
{
	public class ExportHelper
	{
		/// <summary>
		/// Mahr 格式輸出
		/// </summary>
		/// <param name="data">資料</param>
		/// <param name="outputFilepath">輸出檔案路徑</param>
		/// <returns>Success</returns>
		public static bool MahrExport(List<MeasureExportModel> data, string outputFilepath)
		{
			bool success = true;
			try
			{
				var header = GetMahrExportHeader();
				var footer = GetMahrExportFooter();
				var content = header + GetMahrExportContent(data) + footer;
				File.WriteAllText(outputFilepath, content, Encoding.GetEncoding("Big5"));
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
				success = false;
			}
			return success;
		}

		public static string GetMahrExportContent(List<MeasureExportModel> data)
		{
			string tab = "	";
			char space = ' ';
			//string rowTemplate = @"{0}             {1}       {2}     {3}      {4}     {5}     {6}          {7}";
			string rowTemplate = @"{0}{1}{2}{3}{4}{5}{6}{7}";
			StringBuilder sb = new StringBuilder();
			foreach (var model in data)
			{
				double min, max, standard, measure;
				string minText = "", maxText = "", deviationText = "", standardText = "";
				if (Double.TryParse(model.Min, out min)
					&& Double.TryParse(model.Standard, out standard))
				{
					minText = String.Format("-{0:f3}", standard - min, 3);
				}
				if (Double.TryParse(model.Max, out max)
					&& Double.TryParse(model.Standard, out standard))
				{
					maxText = String.Format(" {0:f3}", max - standard, 3);
				}
				if (Double.TryParse(model.MeasureValue, out measure)
					&& Double.TryParse(model.Standard, out standard))
				{
					var diff = Convert.ToDecimal(measure) - Convert.ToDecimal(standard);
					var postiveSymbol = (diff > 0) ? " " : "";
					deviationText = String.Format("{0}{1:f3}", postiveSymbol, diff);
				}
				if (Double.TryParse(model.Standard, out standard))
				{
					standardText = String.Format("{0:f3}", standard);
				}

				var text = String.Format(rowTemplate,
										 model.MeasureName.PadRight(16, space)
										, model.Symbol.PadRight(4, space)
										, model.MeasureValue.PadLeft(10, space)
										, standardText.PadLeft(11, space)
										, maxText.PadLeft(11, space)
										, minText.PadLeft(11, space)
										, deviationText.PadLeft(11, space)
										, GetEvaluation(model));

				sb.AppendLine(text);
				sb.AppendLine("");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 計算 Mahr 評估值
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string GetEvaluation(MeasureExportModel data)
		{
			var evaluation = "";
			char space = ' ';
			double min, max, measure, standard;
			if (Double.TryParse(data.Standard, out standard)
				&& Double.TryParse(data.MeasureValue, out measure)
				&& Double.TryParse(data.Min, out min)
				&& Double.TryParse(data.Max, out max))
			{
				var deviation = measure - standard;
				var isGreater = (deviation) > 0;
				var symbol = (isGreater) ? "+" : "-";
				var diffPercent = (isGreater) ? deviation / (max - standard) : deviation / (standard - min);
				var count = 0;
				var absDiffPercent = Math.Abs(diffPercent);
				if (absDiffPercent <= 0.25) count = 1;
				else if (absDiffPercent > 0.25 && absDiffPercent <= 0.5) count = 2;
				else if (absDiffPercent > 0.5 && absDiffPercent <= 0.75) count = 3;
				else count = 4;
				for (var i = 0; i < count; i++)
					evaluation += symbol;
			}
			return evaluation.PadLeft(11, space);
		}
		public static string GetMahrExportHeader()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("=====================================================================================");
			sb.AppendLine("				OSE MEASURING REPORT");
			sb.AppendLine("=====================================================================================");
			sb.AppendLine("");
			sb.AppendLine("-------------------------------------------------------------------------------------");
			sb.AppendLine("名稱            符號    量測值     設計值   公差上限   公差下限       偏差       評估");
			sb.AppendLine("-------------------------------------------------------------------------------------");
			return sb.ToString();
		}
		public static string GetMahrExportSperateLine()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("========================================================================");
			return sb.ToString();
		}
		public static string GetMahrExportFooter()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("========================================================================");
			return sb.ToString();
		}

		public static string GetHanboCSVExportHeader()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("程式名稱,量測編號,量測項目,量測名稱,MIN,NOM,MAX,量測值,偏差(Deviation),狀態");
			return sb.ToString();
		}

	}
}
