using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using LightControl;
using System.Threading;

namespace PD3_Ethernet_LightControl
{
	public partial class DemoForm : Form
	{
		PD3LightControlManager LightManager = new PD3LightControlManager() { };
		public DemoForm()
		{
			InitializeComponent();
			//LightManager.On_LightControllerResponsed += LightManager_On_LightControllerResponsed;
			//LightManager.On_LightControllerConnected += LightManager_On_LightControllerConnected;
		}





		private void button1_Click(object sender, EventArgs e)
		{
			var lighting = (textBox1.Text != "") ? textBox1.Text : "000";
			var channel = (textBox2.Text != "") ? textBox2.Text : "00";
			var replyMsg = this.LightManager.SetLightIntensity(lighting, channel);
			richTextBox2.AppendText(replyMsg);
		}
		private void button2_Click(object sender, EventArgs e)
		{
			var lightMode = (textBox5.Text != "") ? textBox5.Text : "F00";
			var channel = (textBox2.Text != "") ? textBox2.Text : "00";
			var replyMsg = this.LightManager.SetLightMode(lightMode, channel);

			richTextBox2.AppendText(replyMsg);
		}
		private void button3_Click(object sender, EventArgs e)
		{
			var onOff = (textBox4.Text != "") ? LightSwitch.OFF : LightSwitch.On;
			var channel = (textBox2.Text != "") ? textBox2.Text : "00";
			var replyMsg = this.LightManager.SetLightOnOff(onOff, channel);

			richTextBox2.AppendText(replyMsg);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var command = textBox13.Text;
			string data = command + "\r\n";

			var replyMsg = this.LightManager.SendMessage(data);
			richTextBox2.AppendText(replyMsg);
		}

		private void button5_Click(object sender, EventArgs e)
		{

			int i = 0;
			var replyMsg = this.LightManager.GetChannelStatus("0" + i);
			richTextBox2.AppendText(replyMsg.Channel);
			//@00OF001.S00.L152


		}

		private void L1CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			// L1 On/Off
			var checkbox = sender as CheckBox;
			//checkbox.Checked
			var onOff = checkbox.Checked ? LightSwitch.On : LightSwitch.OFF;
			var replyMsg = this.LightManager.SetLightOnOff(onOff, "00");
			richTextBox2.AppendText(replyMsg);
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			var upDownElement = sender as NumericUpDown;
			//upDownElement.Value
			var onOff = false;
			var replyMsg = "";
			if (onOff)
			{
				var lightMode = "F00";
				var channel = "00";
				replyMsg = this.LightManager.SetLightMode(lightMode, channel);
			}
			replyMsg = this.LightManager.SetLightIntensity(upDownElement.Value.ToString(), "00");
			richTextBox2.AppendText(replyMsg);
			//Thread.Sleep(100);
		}

		private void button6_Click(object sender, EventArgs e)
		{
			var lighting = (textBox1.Text != "") ? textBox1.Text : "100";
			var channel = (textBox2.Text != "") ? textBox2.Text : "00";
			//var replyMsg = this.LightManager.SetLightIntensity(lighting, channel);


			//richTextBox2.AppendText(replyMsg);
			this.LightManager.SetLightIntensityAsync(lighting, channel);
		}
		void LightManager_On_LightControllerResponsed(object sender, ProgressChangedEventArgs e)
		{
			var responseMsg = (string)e.UserState;
			richTextBox2.AppendText(responseMsg);
		}
		void LightManager_On_LightControllerConnected(object sender, ConnectionEventArgs e)
		{
			label18.Text = "Connection:" + e.IsConnected;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			//送出狀態檢查
			var header = "@";
			var command = "CE3";
			var end = "\r\n";
			var data = header + command + end;
			var replymsg = this.LightManager.SendingData(data);
			richTextBox1.Text += replymsg;

		}

	}
}
