using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Hanbo.Helper
{
	public class HanboSecurityHelper
	{
		public static string GetEncrypt(string rawText)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(rawText, "SHA1");
		}
	}
}
