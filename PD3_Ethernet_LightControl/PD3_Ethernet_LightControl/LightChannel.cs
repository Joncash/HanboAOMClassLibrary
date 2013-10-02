using LightControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PD3_Ethernet_LightControl
{
	/// <summary>
	/// 光源頻道資訊
	/// </summary>
	public class LightChannel
	{
		public string Channel { get; set; }
		public int Intensity { get; set; }
		public LightSwitch OnOff { get; set; }
	}
}
