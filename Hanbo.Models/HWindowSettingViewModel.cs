using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	/// <summary>
	/// HWCtrl 設定值 ViewModel
	/// </summary>
	public class HWindowSettingViewModel
	{
		public HWindowSettingViewModel()
		{
			HLines = 2;
			VLines = 2;
			GridLineColor = "red";
			ShowGridLine = false;
			ShowAccuracyAreaGridLine = false;
			AccuracyAreaGridLineColor = "lime green";
		}
		public int HLines { get; set; }
		public int VLines { get; set; }
		public string GridLineColor { get; set; }
		public bool ShowGridLine { get; set; }

		public bool ShowAccuracyAreaGridLine { get; set; }
		public string AccuracyAreaGridLineColor { get; set; }
	}
}
