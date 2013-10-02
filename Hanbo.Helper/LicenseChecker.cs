using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Hanbo.Helper
{
	public class LicenseChecker
	{
		public static bool IsLicenseValid()
		{
#if DEBUG
			return true;
#endif
			var isValid = false;
			var macCode = "00500805F4E4";

			foreach (var item in NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
			{
				if (item.GetPhysicalAddress().ToString() == macCode)
				{
					isValid = true;
				}
			}
			return isValid;
		}
	}
}
