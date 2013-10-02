using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.System.SingleInstance
{
	public class DeviceCheckViewModel
	{

		[DisplayName("裝置名稱")]
		public string Name { get; set; }

		[DisplayName("描述")]
		public string Description { get; set; }

		[DisplayName("檢查結果")]
		public string IsPass { get; set; }

		[DisplayName("系統訊息")]
		public string Message { get; set; }

		public string StaticClassName { get; set; }

		public string InvokeMethod { get; set; }

		public string Params { get; set; }
	}
}
