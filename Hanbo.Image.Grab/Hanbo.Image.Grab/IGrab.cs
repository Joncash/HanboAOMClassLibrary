using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public delegate GrabberEventViewModel GrabberEventHandler();
	public delegate void GrabberReportEventHandler(object sender, ProgressChangedEventArgs e);
	public enum GrabberEventState { Busy, Done, Fail }
	public interface IGrabber
	{
		GrabberEventHandler ConnectHandler { get; set; }
		GrabberEventHandler SnapShotHandler { get; set; }
		RunWorkerCompletedEventHandler ConnectCompletedHandler { get; set; }
		//GrabberEventHandler DisConnectHandler { get; set; }
	}
}
