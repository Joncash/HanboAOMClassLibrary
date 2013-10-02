using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PD3_Ethernet_LightControl
{
	public partial class AsyncConnectionForm : Form
	{
		private ManualResetEvent _connectDone = new ManualResetEvent(false);
		private Socket _client;
		public AsyncConnectionForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			var ControllerIPAddress = new IPAddress(new byte[] { 192, 168, 0, 2 });
			var ControllerPort = 40001;
			var ControllerEndPoint = new IPEndPoint(ControllerIPAddress, ControllerPort);
			_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			var header = "@";
			var command = "00C";
			var checksum = "E3";
			var end = "\r\n";
			var data = header + command + checksum + end;
			byte[] bytes = new byte[1024];

			//Start Connect
			_connectDone.Reset();
			watch.Start();
			_client.BeginConnect(ControllerIPAddress, ControllerPort, new AsyncCallback(ConnectCallback), _client);
			//wait 2s
			_connectDone.WaitOne(2000, false);

			var text = (_client.Connected) ? "ok" : "ng";
			richTextBox1.AppendText(text + "\r\n");
			watch.Stop();
			richTextBox1.AppendText("Consumer time: " + watch.ElapsedMilliseconds + "\r\n");

		}

		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				Socket client = (Socket)ar.AsyncState;

				client.EndConnect(ar);
			}
			catch (Exception ex)
			{
			}
			finally
			{
				_connectDone.Set();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (_client != null)
			{

				_client.Close();
			}
		}


	}
}
