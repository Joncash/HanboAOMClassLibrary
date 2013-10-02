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
	public partial class LightControlForm : Form
	{
		public enum LightChannel { ChannelOne, ChannelTwo, ChannelThree };
		private PD3LightControlManager _lightManager;
		private string[] _channelList = new string[] { "00", "01" };
		List<ChannelStatusViewModel> _currentViewModel = new List<ChannelStatusViewModel>();
		BackgroundWorker worker = new BackgroundWorker();
		public LightControlForm()
		{
			InitializeComponent();
			initializeLightController();
		}
		private void initializeLightController()
		{
			_lightManager = new PD3LightControlManager() { };
		}
		private void ContinuousModeForm_Load(object sender, EventArgs e)
		{
			connectToLightControl();
		}

		private void connectToLightControl()
		{
			ControlPanel.Enabled = false;
			this.Enabled = false;
			string msg;
			var isConnected = _lightManager.TryConnect(out msg);
			var connectStatusText = (!isConnected) ? "狀態 : 未連線" : "狀態 : 已連線";
			StatusLabel.Text = connectStatusText;
			if (isConnected)
			{
				this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_On_48;
				IPLabel.Text = "IP : " + _lightManager.GetIPAdress();
				PortLabel.Text = "Port :" + _lightManager.GetPort();
				initializeChannelInfo();
			}
			else
			{
				setPowerOff();
			}
			Thread.Sleep(200);
			this.Enabled = true;
		}
		private void setPowerOff()
		{
			this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_Off;
			StatusLabel.Text = "狀態 : 未連線";
			ControlPanel.Enabled = false;
		}
		private void initializeChannelInfo()
		{
			var thisForm = this;
			var bgworker = new BackgroundWorker();
			bgworker.WorkerReportsProgress = true;
			bgworker.DoWork += (caller, args) =>
			{
				var worker = caller as BackgroundWorker;
				var channels = new string[] { "00", "01" };
				var count = 0;
				foreach (var channel in channels)
				{
					var channelViewModel = _lightManager.GetChannelStatus(channel);
					worker.ReportProgress(++count, channelViewModel);
					Thread.Sleep(200);
				}
			};
			bgworker.ProgressChanged += (caller, args) =>
			{
				var model = args.UserState as ChannelStatusViewModel;
				if (model != null)
				{
					setChannelInfo(model);
				}
			};
			bgworker.RunWorkerCompleted += (sender, e) =>
			{
				thisForm.Enabled = true;
				ControlPanel.Enabled = true;
				initializeEvents();
			};
			thisForm.Enabled = false;
			bgworker.RunWorkerAsync();
		}

		private void setChannelInfo(ChannelStatusViewModel channelViewModel)
		{
			var cBox = ControlPanel.Controls.OfType<CheckBox>().SingleOrDefault(p => (string)p.Tag == channelViewModel.Channel);
			if (cBox != null)
			{
				cBox.Checked = channelViewModel.OnOff == LightSwitch.On.ToString();
			}
			var numericBox = ControlPanel.Controls.OfType<NumericUpDown>().SingleOrDefault(p => (string)p.Tag == channelViewModel.Channel);
			if (numericBox != null)
			{
				numericBox.Value = Convert.ToDecimal(channelViewModel.Intensity);
			}
		}

		private bool eventInitialized = false;
		private BackgroundWorker _sendDataWorker;
		private void initializeSendDataWorker()
		{
			_sendDataWorker = new BackgroundWorker();
			_sendDataWorker.DoWork += _sendDataWorker_DoWork;
			_sendDataWorker.RunWorkerCompleted += _sendDataWorker_RunWorkerCompleted;
		}

		void _sendDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			throw new NotImplementedException();
		}

		void _sendDataWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			throw new NotImplementedException();
		}
		private void initializeEvents()
		{
			if (eventInitialized) return;
			L1CheckBox.CheckedChanged += new EventHandler(LightOnSwitch);
			L2CheckBox.CheckedChanged += new EventHandler(LightOnSwitch);
			L3CheckBox.CheckedChanged += new EventHandler(LightOnSwitch);
			numericUpDown1.ValueChanged += new EventHandler(LightIntesity_ValueChanged);
			numericUpDown2.ValueChanged += new EventHandler(LightIntesity_ValueChanged);
			numericUpDown3.ValueChanged += new EventHandler(LightIntesity_ValueChanged);
			eventInitialized = true;
		}

		private void LightOnSwitch(object sender, EventArgs e)
		{
			// L1 On/Off
			var checkbox = sender as CheckBox;
			var onOff = checkbox.Checked ? LightSwitch.On : LightSwitch.OFF;
			var channel = (string)checkbox.Tag;
			setChannelOnOff(onOff, channel);
		}
		private void LightIntesity_ValueChanged(object sender, EventArgs e)
		{
			var upDownElem = sender as NumericUpDown;
			var channel = (string)upDownElem.Tag;
			var setValue = (int)upDownElem.Value;
			setChannelIntensity(setValue, channel);
		}

		private void setChannelOnOff(LightSwitch onOff, string channel)
		{
			var msg = _lightManager.SetLightOnOff(onOff, channel);
			updatePanel(msg);
		}
		private void updatePanel(string msg)
		{
			if (String.IsNullOrEmpty(msg))
			{
				setPowerOff();
			}
			else if (msg.Substring(3, 1) != "O")
			{
				MessageBox.Show("Error");
			}
		}

		private void setChannelIntensity(int setValue, string channel)
		{
			var msg = _lightManager.SetLightIntensity(setValue, channel);
			updatePanel(msg);
		}
		private void ResetButton_Click(object sender, EventArgs e)
		{
			connectToLightControl();
		}
		#region Public APIs

		public void SetLightData(string channel, string intensity, string onOff)
		{
			int intensityValue;
			Int32.TryParse(intensity, out intensityValue);

			var switchOn = (onOff == "On") ? LightSwitch.On : LightSwitch.OFF;
			switch (channel)
			{
				case "00":
					if (intensityValue > 0)
					{
						numericUpDown1.Value = intensityValue;
						//setChannelIntensity(intensityValue, channel);
					}
					L1CheckBox.Checked = (onOff == "On");
					setChannelOnOff(switchOn, channel);

					break;
				case "01":
					if (intensityValue > 0)
					{
						numericUpDown2.Value = intensityValue;
						//setChannelIntensity(intensityValue, channel);
					}
					L2CheckBox.Checked = (onOff == "On");
					setChannelOnOff(switchOn, channel);
					break;
				//case "02":
				//	if (intensityValue > 0)
				//	{
				//		numericUpDown3.Value = intensityValue;
				//		//setChannelIntensity(intensityValue, channel);
				//	}
				//	L3CheckBox.Checked = (onOff == "On");
				//	setChannelOnOff(switchOn, channel);
				//	break;
			}


		}

		public void ResetLightControlMode()
		{
			var replyMsg = _lightManager.SetLightOnOff(LightSwitch.OFF, "FF");
			foreach (var channel in _channelList)
			{
				//set continuous mode
				var lightMode = "00";
				replyMsg = _lightManager.SetLightMode(lightMode, channel);
			}
		}
		#endregion
	}
}
