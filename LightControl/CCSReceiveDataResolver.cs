using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightControl
{
	public class CCSReceiveDataResolver
	{
		public static CCSReceiveMessageViewModel Resolve(string message)
		{
			/* Message 組成
			 * header (1bit) + channel (2bit) + receiveCommand + checksum + Delimiter
			 * ------------------------------  OK: |  OF999.S99.L9 or O00 or O01 or ""  |------------------------
			 * ------------------------------  NG: |  N01 or N02 or N03 |
			 */
			var channel = (message.Length > 3) ? message.Substring(1, 2) : "";
			var receiveCommand = (message.Length > 4) ? message.Substring(3, message.Length - 3) : "";
			var status = receiveCommand.Length > 1 ? receiveCommand.Substring(0, 1) : "NG";
			var commandStatus = status == "O" ? "OK" : "NG";

			var modelParts = receiveCommand.Split('.');
			var hasLightModeInfo = modelParts.Length > 2;
			var intensity = hasLightModeInfo ? modelParts[0].Substring(2, 3) : "";
			var lightMode = hasLightModeInfo ? modelParts[1].Substring(1, 2) : "";
			var onOff = hasLightModeInfo ? modelParts[2].Substring(1, 1) : "";

			return new CCSReceiveMessageViewModel()
			{
				Channel = channel,
				Status = commandStatus,
				Intensity = intensity,
				LightMode = lightMode,
				OnOff = (onOff == "1") ? LightSwitch.On : LightSwitch.OFF
			};
		}
	}
}
