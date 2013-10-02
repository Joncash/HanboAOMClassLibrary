using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	public class SpecDTO
	{
		public string MeasureType;
		public string MeasureName { get; set; }

		public string Min { get; set; }

		public string Normal { get; set; }

		public string Max { get; set; }

		public string AssemblyFullName { get; set; }

		public string Unit { get; set; }
	}
}
