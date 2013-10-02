using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models.Argument
{
	public enum AutoExportArgumentPostfixType { AutoNumber, DateTime, Mixed };
	public enum AutoExportArgumentPrefixType { Default, Custom };
	public class AutoExportArgument
	{
		public string OutputDirectory { get; set; }

		public string Prefix { get; set; }

		public string PostfixFormat { get; set; }

		public string ExtensionName { get; set; }

		public AutoExportArgumentPostfixType PostfixType;
		public AutoExportArgumentPrefixType PrefixType;
	}
}
