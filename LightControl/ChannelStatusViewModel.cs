using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightControl
{
	public class ChannelStatusViewModel
	{
		public string CommandStatus { get; set; }

		public string Channel { get; set; }

		public string Intensity { get; set; }

		public string Mode { get; set; }

		public string StrobeTime { get; set; }

		public string OnOff { get; set; }
	}
}
