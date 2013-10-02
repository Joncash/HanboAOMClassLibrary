using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	[Serializable]
	public class ExportMacroPlanViewModel
	{
		public string MacroName { get; set; }

		public string MacroGuid { get; set; }

		public string Note { get; set; }

		public System.Data.Linq.Binary ShapeModelBinary { get; set; }

		public System.Data.Linq.Binary TrainingImageBinary { get; set; }

		public System.Data.Linq.Binary MatchingParamBinaryData { get; set; }

		public System.Data.Linq.Binary MeasureBinaryData { get; set; }

		public System.Data.Linq.Binary MeasureAssistantBinaryData { get; set; }

		public double? ModelRow { get; set; }

		public double? ModelCol { get; set; }

		public double? ModelAngle { get; set; }

		public string ExportUnit { get; set; }

		public string UpperLightValue { get; set; }

		public string BottomLigthValue { get; set; }

		public bool? UpperLightSwitch { get; set; }

		public bool? BottomLightSiwtch { get; set; }

		public string CreateBy { get; set; }

		public string ModifiedBy { get; set; }

		public DateTime? CreateOn { get; set; }

		public DateTime? ModifiedOn { get; set; }

		public bool? IsDeleted { get; set; }
	}
}
