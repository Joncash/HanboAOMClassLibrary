using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.WindowControlWrapper
{
	public class RefCoordinate
	{
		public RefCoordinate()
		{
			ID = "";
			Name = "Default";
			Desc = "影像座標";
		}
		/// <summary>
		/// 參考座標 ID
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
