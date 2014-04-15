using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.WindowsFormsControlLibrary.Enum
{
	/// <summary>
	/// 狀態列
	/// </summary>
	public enum SystemStatusType
	{
		/// <summary>
		/// 連線狀態
		/// </summary>
		ConnectionStatus,
		/// <summary>
		/// 操作模式
		/// </summary>
		ControlMode,
		/// <summary>
		/// 影像座標
		/// </summary>
		Coordinate,
		/// <summary>
		/// 歸零座標
		/// </summary>
		CustomCoordinate,
		/// <summary>
		/// 灰階
		/// </summary>
		GrayLevel,
		/// <summary>
		/// 系統訊息
		/// </summary>
		SystemMsg,
		/// <summary>
		/// 縮放比例
		/// </summary>
		ZoomFactor,
	}
}
