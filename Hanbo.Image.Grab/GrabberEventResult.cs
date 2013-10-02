using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public class GrabberEventResult
	{
		public GrabberEventState State { get; set; }

		public Exception Error { get; set; }

		public object Model { get; set; }
	}
}
