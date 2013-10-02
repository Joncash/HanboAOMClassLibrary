using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightControl
{
	public class CCSReceiveMessageViewModel
	{
		public string Channel { get; set; }
		public string Intensity { get; set; }
		public LightSwitch OnOff { get; set; }
		public string Status { get; set; }

		public string LightMode { get; set; }
	}
}
