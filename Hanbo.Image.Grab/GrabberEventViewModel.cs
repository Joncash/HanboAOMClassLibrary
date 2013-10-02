using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public class GrabberEventViewModel
	{
		public GrabberReportEventHandler ReportHandler;
		public GrabberEventResult Result { get; set; }
	}
}
