using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PD3_Ethernet_LightControl
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new DemoForm());
			//Application.Run(new LightControlForm());
			//Application.Run(new ProbeConnectionForm());
			Application.Run(new AsyncConnectionForm());
			//Application.Run(new CCSLightControlForm());
		}
	}
}
