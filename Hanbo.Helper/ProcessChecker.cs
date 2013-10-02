using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	/// <summary>
	/// 應用程式執行檢查範圍
	/// </summary>
	public enum ProcessCheckRange { Local, Global };

	/// <summary>
	/// 應用程式檢查類別
	/// </summary>
	public class ProcessChecker
	{
		/// <summary>
		/// 檢查應用程式是否已執行
		/// </summary>
		/// <param name="checkRange">檢查範圍 : Global (同一主機所有使用者), Local (同一主機相同使用者)</param>
		/// <returns>是否已執行</returns>
		public static bool IsProcessExists(ProcessCheckRange checkRange)
		{
			var isCreated = false;
			var muteRange = "Global\\";
			switch (checkRange)
			{
				case ProcessCheckRange.Global:
					muteRange = "Global\\";
					break;
				case ProcessCheckRange.Local:
					muteRange = "Local\\";
					break;
			}
			var mutexName = Process.GetCurrentProcess().ProcessName;
			System.Threading.Mutex mutex = new System.Threading.Mutex(true, muteRange + mutexName, out isCreated);
			return isCreated;
		}

		/// <summary>
		/// 檢查應用程式是否已執行
		/// </summary>
		/// <returns></returns>
		public static bool IsProcessExists()
		{
			var isCreated = true;
			Process curr = Process.GetCurrentProcess();
			Process[] procs = Process.GetProcessesByName(curr.ProcessName);
			foreach (Process p in procs)
			{
				if ((p.Id != curr.Id) &&
					(p.MainModule.FileName == curr.MainModule.FileName))
				{
					isCreated = false;
					break;
				}
			}
			return isCreated;
		}

	}
}
