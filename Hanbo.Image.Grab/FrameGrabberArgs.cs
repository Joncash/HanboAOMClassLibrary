using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public class FrameGrabberArgs
	{
		public int BitsPerChannel { get; set; }

		public string ColorSpace { get; set; }

		public string Generic { get; set; }

		public string CameraType { get; set; }

		public string Device { get; set; }

		public int Port { get; set; }

		public int LineIn { get; set; }

		public string ExternalTrigger { get; set; }

		public string Field { get; set; }

		public int StartColumn { get; set; }

		public int StartRow { get; set; }

		public int ImageHeight { get; set; }

		public int ImageWidth { get; set; }

		public int VerticalResolution { get; set; }

		public int HorizontalResolution { get; set; }

		public string Name { get; set; }
	}
}
