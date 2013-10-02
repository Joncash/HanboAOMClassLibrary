using Hanbo.System.SingleInstance;
using LightControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PD3_Ethernet_LightControl
{
	public partial class ProbeConnectionForm : Form
	{
		CCSLightControlManager _lightControlManager;
		public ProbeConnectionForm()
		{
			InitializeComponent();
			_lightControlManager = new CCSLightControlManager();//DeviceController.GetCCSLightControlManagerInstance();
			_lightControlManager.On_ProbeConnectionWorked += _lightControlManager_On_Disconnected;
			_lightControlManager.On_SafetyClosed += _lightControlManager_On_SafetyClosed;
		}

		void _lightControlManager_On_SafetyClosed(object sender, object args)
		{
			this.Close();
		}
		
		private void ProbeConnectionForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_lightControlManager.DoSafetyClose();
			if (!_lightControlManager.IsDoSafetyCloseDone)
			{
				e.Cancel = true;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//連線
			richTextBox1.AppendText("Current Connected:" + _lightControlManager.Connected + "\r\n");
			var msg = _lightControlManager.Connect(null);
			richTextBox1.AppendText("Current Connected:" + msg + "\r\n");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//偵測
			_lightControlManager.StartProbeConnetion(200);

		}

		void _lightControlManager_On_Disconnected(object sender, object args)
		{
			var connected = (bool)args;
			richTextBox2.AppendText("目前連線 : " + connected.ToString() + " \r\n");
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			var upDown = sender as NumericUpDown;
			if (_lightControlManager.Connected)
			{
				var channel = "00";
				var intensity = (int)upDown.Value;
				_lightControlManager.SetLightIntensityAsync(intensity, channel);
			}
		}

		
	}
}
