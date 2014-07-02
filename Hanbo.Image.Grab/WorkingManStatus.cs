using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public enum GrabStage { Initial, Closed, Connecting, Connected, Grabbing, Grabbed, ContinuouslyGrabbing, Stop };
	public enum GrabState { Idle, Busy };
	public enum GrabInstruction { Initialize, Connect, DisConnect, GrabImage, ContinuouslyGrabImage };
	/// <summary>
	/// workingMan 狀態
	/// </summary>
	public class WorkingManStatus
	{
		public bool IsConnection { get; set; }

		public string Message { get; set; }

		/// <summary>
		/// 階段
		/// </summary>
		public GrabStage Stage { get; set; }


		/// <summary>
		/// 狀態
		/// </summary>
		public GrabState State { get; set; }

		/// <summary>
		/// 指令 Input
		/// </summary>
		public GrabInstruction Instruction { get; set; }

	}
}
