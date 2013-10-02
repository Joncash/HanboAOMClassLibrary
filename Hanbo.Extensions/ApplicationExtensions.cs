using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Extensions
{
	public static class ApplicationExtensions
	{
		public static string ToAppAbsolutePath(this String str)
		{
			var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(baseDir, str);
		}
	}
}
