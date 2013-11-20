using Hanbo.Helper;
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
	public partial class CCSLightControlForm : Form
	{
		CCSLightControlManager _lightControlManager;

		private bool _lock = false;
		private bool _isConnected = false;
		private int _maxTry = 3;	//最大嘗試次數
		private int _curTry = 0;	//目前嘗試次數
		private int _heartbeatInterval = ConfigurationHelper.GetHeartbeatInterval();
		private bool _latestHardwareConnectedStatus;
		public CCSLightControlForm()
		{
			InitializeComponent();
			LightConnectTimer.Tick += LightConnectTimer_Tick;
		}
		private void CCSLightControlForm_Load(object sender, EventArgs e)
		{
			CCSLightControlForm_LocationChanged(sender, e);
			_lightControlManager = DeviceController.GetCCSLightControlManagerInstance();
			_lightControlManager.On_SafetyClosed += _lightControlManager_On_SafetyClosed;
			_lightControlManager.StartProbeConnetion(_heartbeatInterval);
			initializeLightControl();
		}

		void _lightControlManager_On_SafetyClosed(object sender, object args)
		{
			this.Close();
		}
		private void CCSLightControlForm_Shown(object sender, EventArgs e)
		{
			LightConnectTimer.Interval = 1500;
			LightConnectTimer.Enabled = true;
			LightConnectTimer.Start();
		}
		private void CCSLightControlForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_lightControlManager != null)
			{
				DeviceController.ReleaseCCSLightControlManagerInstance();
				_lightControlManager.DoSafetyClose();
				if (!_lightControlManager.IsDoSafetyCloseDone)
				{
					e.Cancel = true;
				}
			}
		}

		void LightConnectTimer_Tick(object sender, EventArgs e)
		{
			var timer = sender as System.Windows.Forms.Timer;
			_curTry++;
			bool isOverRetryCont = _curTry > _maxTry;
			if (isOverRetryCont || _isConnected)
			{
				timer.Stop();
				timer.Enabled = false;
			}
			else if (!_isConnected)
			{
				initializeLightControl();
			}

			if (isOverRetryCont && !_isConnected)
			{
				MessageBox.Show(Hanbo.Resources.Resource.Message_LightControllerDisconnectedNotice);
			}
		}

		private void initializeLightControl()
		{
			ResetButton.Enabled = false;
			this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_Off;
			ControlPanel.Enabled = false;
			if (!_lightControlManager.Connected)
			{
				var msg = _lightControlManager.Connect(null);
				if (msg == "Timeout")
				{
					StatusLabel.Text = "狀態: 未連線";
				}
			}
			IPLabel.Text = "IP :" + _lightControlManager.ControllerIPAddress.ToString();
			PortLabel.Text = "Port :" + _lightControlManager.ControllerPort.ToString();
			_isConnected = _lightControlManager.Connected;

			if (_isConnected)
			{
				this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_On_48;
				ControlPanel.Enabled = true;
				StatusLabel.Text = "狀態: 已連線";

				//binding Event
				_lightControlManager.On_ReceivedData += _lightControlManager_On_ReceivedData;
				L1CheckBox.CheckedChanged += new EventHandler(switchOnOff);
				L2CheckBox.CheckedChanged += new EventHandler(switchOnOff);
				L3CheckBox.CheckedChanged += new EventHandler(switchOnOff);

				numericUpDown1.ValueChanged += new EventHandler(intensityChanged);
				numericUpDown2.ValueChanged += new EventHandler(intensityChanged);
				numericUpDown3.ValueChanged += new EventHandler(intensityChanged);

				//init panel value
				_lightControlManager.GetChannelStatusAsync(new string[] { "00", "01", "02" });
				_lightControlManager.On_ProbeConnectionWorked += _lightControlManager_On_ProbeConnectionWorked;
				//_lightControlManager.StartProbeConnetion(null);
			}
			ResetButton.Enabled = true;
		}

		void _lightControlManager_On_ProbeConnectionWorked(object sender, object args)
		{
			var hardwareConnectedStatus = (bool)args;
			if (_latestHardwareConnectedStatus ^ hardwareConnectedStatus)
			{
				_latestHardwareConnectedStatus = ControlPanel.Enabled = hardwareConnectedStatus;
				this.ResetButton.Image = (!hardwareConnectedStatus) ?
					global::PD3_Ethernet_LightControl.Properties.Resources.power_Off :
					global::PD3_Ethernet_LightControl.Properties.Resources.power_On_48;

				var statusText = (hardwareConnectedStatus) ? "狀態: 已連線" : "狀態: 未連線";
				StatusLabel.Text = statusText;
			}
		}

		private void intensityChanged(object sender, EventArgs e)
		{
			var upDown = sender as NumericUpDown;
			if (_lightControlManager.Connected)
			{
				if (!_lock)
				{
					var channel = (string)upDown.Tag;
					var intensity = (int)upDown.Value;
					_lightControlManager.SetLightIntensityAsync(intensity, channel);
				}
			}
		}

		private void switchOnOff(object sender, EventArgs e)
		{
			var ck = sender as CheckBox;
			if (_lightControlManager.Connected)
			{
				if (!_lock)
				{
					var channel = (string)ck.Tag;
					var onOff = ck.Checked ? LightSwitch.On : LightSwitch.OFF;
					_lightControlManager.SetLightOnOffAsync(onOff, channel);
				}
			}
		}

		void _lightControlManager_On_ReceivedData(object sender, object receiveMessage)
		{
			//解析 receiveMessage
			var model = CCSReceiveDataResolver.Resolve((string)receiveMessage);
			if (model.Intensity != "")
			{
				var onOff = model.OnOff == LightSwitch.On ? true : false;
				int intensity;
				Int32.TryParse(model.Intensity, out intensity);
				_lock = true;
				switch (model.Channel)
				{
					case "00":
						L1CheckBox.Checked = onOff;
						numericUpDown1.Value = intensity;
						break;
					case "01":
						L2CheckBox.Checked = onOff;
						numericUpDown2.Value = intensity;
						break;
					case "02":
						L3CheckBox.Checked = onOff;
						numericUpDown3.Value = intensity;
						break;
				}
				_lock = false;
			}

		}

		private void ResetButton_Click(object sender, EventArgs e)
		{
			initializeLightControl();
		}

		#region Public APIs

		public void SetLightData(string channel, string intensity, string onOff)
		{
			var switchOn = (onOff == "On");
			int intensityValue;
			Int32.TryParse(intensity, out intensityValue);
			switch (channel)
			{
				case "00":
					numericUpDown1.Value = intensityValue;
					L1CheckBox.Checked = switchOn;
					break;
				case "01":
					numericUpDown2.Value = intensityValue;
					L2CheckBox.Checked = switchOn;
					break;
			}
		}

		public void ResetLightControlMode()
		{
			//var replyMsg = _lightManager.SetLightOnOff(LightSwitch.OFF, "FF");
			//foreach (var channel in _channelList)
			//{
			//	//set continuous mode
			//	var lightMode = "00";
			//	replyMsg = _lightManager.SetLightMode(lightMode, channel);
			//}
		}
		public void GetChannelsInfo()
		{

		}
		#endregion




		private void button1_Click(object sender, EventArgs e)
		{
			SetLightData("00", "100", "On");
		}

		private void TraceConnectTimer_Tick(object sender, EventArgs e)
		{
			//偵測連線是否中斷

			//this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_Off;
			//ControlPanel.Enabled = false;
		}

		private void CCSLightControlForm_LocationChanged(object sender, EventArgs e)
		{
			var win = sender as Form;
			LocationLabel.Text = String.Format("x: {0} y: {1}", win.Location.X, win.Location.Y);
		}
	}
}
