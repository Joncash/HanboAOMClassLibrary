using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightControl
{
	public delegate void LightControlResponseHandler(object sender, object eventArgs);
	public class CCSLightControlAssistant
	{
		private System.Windows.Forms.Timer _timer;
		private CCSLightControlManager _manager;
		private List<object> _messageStack;
		private int _interval = 20; //ms

		/// <summary>
		/// CCSLightControlAssistant
		/// </summary>
		/// <param name="manager">CCSLightControlManager</param>
		public CCSLightControlAssistant(CCSLightControlManager manager)
		{
			init(manager, _interval);
		}

		/// <summary>
		/// CCSLightControlAssistant
		/// </summary>
		/// <param name="manager">CCSLightControlManager</param>
		/// <param name="checkInterval">ms</param>
		public CCSLightControlAssistant(CCSLightControlManager manager, int checkInterval)
		{
			init(manager, checkInterval);
		}

		public void SendMessage(CCSCommandModel message)
		{
			_messageStack.Add(message);
			if (_messageStack.Count > 0) _timer.Start();
		}

		private void init(CCSLightControlManager manager, int interval)
		{
			_manager = manager;
			_manager.On_ReceivedData += _manager_On_ReceivedData;

			_timer = new System.Windows.Forms.Timer();
			_timer.Interval = interval;
			_timer.Tick += _timer_Tick;

			_messageStack = new List<object>();
		}

		private void _manager_On_ReceivedData(object sender, object receiveMessage)
		{
			if (_messageStack.Count > 0)
				_messageStack.RemoveAt(0);
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			if (_messageStack.Count == 0)
			{
				stop();
			}
			else if (_manager.IsReady())
			{
				var model = _messageStack[0];
				_manager.SendMessage(model);
			}
		}

		private void stop()
		{
			_timer.Stop();
		}
	}
}
