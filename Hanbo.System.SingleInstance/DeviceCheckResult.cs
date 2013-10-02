using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.System.SingleInstance
{
	public class DeviceCheckResult
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public Exception ExceptionDetail { get; set; }

		public string Name { get; set; }
	}
}
