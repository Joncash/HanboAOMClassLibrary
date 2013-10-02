using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace LightControl
{
	/// <summary>
	/// PD3-3024-3-EI 通訊測試
	/// </summary>
	public class PD3Test
	{
		public PD3Test()
		{
			_controllerIPAddress = new IPAddress(new byte[] { 192, 168, 0, 2 });
			_controllerPort = 40001;
			_controllerEndPoint = new IPEndPoint(_controllerIPAddress, _controllerPort);
		}

		/// <summary>
		/// 測試 Checking the Unit Status 指令
		/// </summary>
		/// <param name="data">header + instruction + checksum + delimiter</param>
		[Theory]
		[InlineData("@00CE3\r\n", "@00O004F\r\n")]
		public void TestCheckingUnitState(string data, string expectedReplyData)
		{
			var replyMsg = "";
			using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				if (!_client.Connected)
				{
					byte[] bytes = new byte[1024];

					_client.Connect(this._controllerEndPoint);
					byte[] msg = Encoding.ASCII.GetBytes(data);

					// Send the data through the socket.
					int bytesSent = _client.Send(msg);

					// Receive the response from the remote device.
					int bytesRec = _client.Receive(bytes);
					replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));
				}
				_client.Shutdown(SocketShutdown.Both);
				_client.Close();
			}
			Assert.True(replyMsg == expectedReplyData);
		}

		private Socket _client { get; set; }

		private System.Net.EndPoint _controllerEndPoint { get; set; }

		private IPAddress _controllerIPAddress { get; set; }

		private int _controllerPort { get; set; }
	}
}
