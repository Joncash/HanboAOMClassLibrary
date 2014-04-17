using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
namespace Hanbo.WindowsFormsControlLibrary
{
	public class CustomHalconControlDesigner : ControlDesigner
	{
		public override void Initialize(IComponent comp)
		{
			base.Initialize(comp);
			var uc = (CustomHalconControl)comp;
			EnableDesignMode(uc.HalconContainer, "Panel_Halcon");
			//EnableDesignMode(uc.HalconWin, "HalconWin");
		}
	}
}
