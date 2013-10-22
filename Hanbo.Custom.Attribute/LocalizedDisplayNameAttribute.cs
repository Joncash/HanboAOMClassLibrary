using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Custom.Attribute
{
	public class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		/// <summary>
		/// LocalizedDisplayNameAttribute
		/// </summary>
		/// <param name="resourceID">Resource ID</param>
		/// <param name="defaultDisplayName">預設的顯示名稱，如果找不到 Resouce, 將用此替代</param>
		public LocalizedDisplayNameAttribute(string resourceID, string defaultDisplayName)
			: base(GetMessageFromResource(resourceID, defaultDisplayName))
		{

		}
		private static string GetMessageFromResource(string resourceId, string defaultDisplayName)
		{
			return Hanbo.Resources.Resource.ResourceManager.GetString(resourceId) ?? defaultDisplayName;
		}
	}
}
