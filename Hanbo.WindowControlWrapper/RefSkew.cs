using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.WindowControlWrapper
{
	/// <summary>
	/// 參考軸擺正
	/// </summary>
	public class RefSkew
	{
		public RefSkew()
		{
			ID = "";
			Name = "Default (0)";
			Desc = "傾斜 0 度";
		}
		/// <summary>
		/// 參考元素 ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 顯示名稱
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Desc { get; set; }
	}
}
