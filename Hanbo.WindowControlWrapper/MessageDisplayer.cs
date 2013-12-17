using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hanbo.WindowControlWrapper
{
	public class MessageDisplayer
	{
		private System.Windows.Forms.Timer _timer;
		private System.Windows.Forms.Label _label;
		private int _count;
		private int _hold;
		private int _interval;

		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="displayLabel">顯示的 Label </param>
		/// <param name="interval">顯示的秒數</param>
		public MessageDisplayer(Label displayLabel, int hold)
		{
			_label = displayLabel;
			_interval = 1000;
			_hold = hold;
			init();
		}
		private void init()
		{
			_timer = new Timer();
			_timer.Interval = _interval;
			_timer.Tick += _timer_Tick;
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			_count++;
			if (_count >= _hold)
			{
				Stop();
			}
			else
			{
				_label.BackColor = Color.FromArgb(100 / _count, System.Drawing.Color.Red);
			}
		}

		/// <summary>
		/// 顯示訊息
		/// </summary>
		/// <param name="message"></param>
		public void SetMessage(string message)
		{
			_timer.Stop();
			_timer.Enabled = true;
			_label.Text = message;
			_label.BackColor = System.Drawing.Color.Red;
			_count = 0;
			_timer.Start();
		}

		/// <summary>
		/// 停止訊息
		/// </summary>
		public void Stop()
		{
			_timer.Stop();
			_label.Text = "";
			_label.BackColor = System.Drawing.SystemColors.Control;
		}
	}
}
