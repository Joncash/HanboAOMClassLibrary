using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.ComponentModel;

namespace LightControl
{
	public enum LightSwitch { OFF, On };

	/// <summary>
	/// 光源控制管理員
	/// </summary>
	public class PD3LightControlManager : IDisposable
	{
		/*
		 * 定義 command 標頭, 結束符號, Instruction
		 */
		#region PD3 Instruction Command Define

		/// <summary>
		/// Command 標頭
		/// </summary>
		protected string header = "@";

		/// <summary>
		/// Command 結束符號
		/// </summary>
		protected string delimiter = "\r\n";
		#endregion

		/*
		 * properties
		 */
		public IPAddress ControllerIPAddress { get; set; }
		public int ControllerPort { get; set; }
		public IPEndPoint ControllerEndPoint;
		private Socket _client;


		/*
		 * 事件
		 */
		public delegate void LightControllerInitializedEventHandler(object sender, LightControlInitEventArgs e);

		#region 建構子
		public PD3LightControlManager(IPAddress ipAddress, int port)
		{
			this.ControllerIPAddress = ipAddress;
			this.ControllerPort = port;
			ControllerEndPoint = new IPEndPoint(ipAddress, port);
			initialize();
		}
		/// <summary>
		/// 預設 IPAdress = 192.168.0.2; Port = 40001
		/// </summary>
		public PD3LightControlManager()
		{
			this.ControllerIPAddress = new IPAddress(new byte[] { 192, 168, 0, 2 });
			this.ControllerPort = 40001;
			ControllerEndPoint = new IPEndPoint(this.ControllerIPAddress, this.ControllerPort);
			initialize();
		}
		#endregion

		private void initialize()
		{

		}

		/// <summary>
		/// 連線測試
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool TryConnect(out string message)
		{
			bool connected = false;
			message = "";
			var exceptionMessage = "";

			//連線後若不送資料，下次再開啟程式時，PD3-3024 -3  控制器會發生異常，無法再次正確工作
			var connectTestThread = new Thread(() =>
			{
				try
				{
					using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						if (!_client.Connected)
						{
							byte[] bytes = new byte[1024];
							var replyMsg = "";
							var data = getCheckUnitStatusData();
							_client.Connect(this.ControllerEndPoint);
							byte[] msg = Encoding.ASCII.GetBytes(data);

							// Send the data through the socket.
							int bytesSent = _client.Send(msg);

							// Receive the response from the remote device.
							int bytesRec = _client.Receive(bytes);
							replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));
							connected = _client.Connected;
						}
						_client.Shutdown(SocketShutdown.Both);
						_client.Close();
					}
				}
				catch (SocketException ex)
				{
					exceptionMessage = ex.Message;
				}
			});
			connectTestThread.Start();
			connectTestThread.Join(35000);
			if (!connected)
			{
				if (exceptionMessage != "") message = exceptionMessage;
				else message = "連線逾時";
			}
			else
			{
				message = "已連線";
			}
			connectTestThread.Abort();
			return connected;
		}

		public string SendingData(string data)
		{
			byte[] bytes = new byte[1024];
			var replyMsg = "";
			try
			{
				using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					_client.Connect(this.ControllerEndPoint);
					byte[] msg = Encoding.ASCII.GetBytes(data);

					// Send the data through the socket.
					int bytesSent = _client.Send(msg);

					// Receive the response from the remote device.
					int bytesRec = _client.Receive(bytes);
					replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));

					// Release the socket.
					_client.Shutdown(SocketShutdown.Both);
					_client.Close();
				}
			}
			catch (Exception se)
			{
				replyMsg = ("錯誤 : " + se.ToString());
			}
			return replyMsg;
		}

		/// <summary>
		/// 送資料
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private string sendData(string data)
		{
			byte[] bytes = new byte[1024];
			var replyMsg = "";
			var requestCompleted = false;
			var sendRequestThread = new Thread(() =>
			{
				try
				{
					using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						_client.Connect(this.ControllerEndPoint);
						byte[] msg = Encoding.ASCII.GetBytes(data);

						// Send the data through the socket.
						int bytesSent = _client.Send(msg);

						// Receive the response from the remote device.
						int bytesRec = _client.Receive(bytes);
						replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));

						// Release the socket.
						_client.Shutdown(SocketShutdown.Both);
						_client.Close();
						requestCompleted = true;
					}
				}
				catch (Exception se)
				{
					replyMsg = ("錯誤 : " + se.ToString());
				}
			});
			sendRequestThread.Start();
			sendRequestThread.Join(2000);

			if (!requestCompleted)
			{
				sendRequestThread.Abort();
			}
			return replyMsg;
		}

		public object GetSettingStatus()
		{
			return "";
		}

		/// <summary>
		/// 檢查狀態
		/// </summary>
		/// <returns></returns>
		public string CheckUnitStatus()
		{
			var command = "00CE3";
			return sendData(header + command + delimiter);
		}
		private string getCheckUnitStatusData()
		{
			var command = "00CE3";
			return header + command + delimiter;
		}

		/// <summary>
		/// 送出訊息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public string SendMessage(string data)
		{
			return sendData(data);
		}

		/// <summary>
		/// Reset
		/// </summary>
		/// <returns></returns>
		public string ResetAllChannel()
		{
			var command = "00RF2";
			return sendData(header + command + delimiter);
		}

		public string SetIpAddress(string ipaddress)
		{
			var instruction = "E01";
			var channel = "00";
			var command = header + channel + instruction + ipaddress;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		public string SetSubMask(string submask)
		{
			var instruction = "E02";
			var channel = "00";
			var command = header + channel + instruction + submask;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}
		public string SetDefaultGatway(string gatway)
		{
			var instruction = "E03";
			var channel = "00";
			var command = header + channel + instruction + gatway;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}
		public string SetReceptionPort(int port)
		{
			var instruction = "E04";
			var channel = "00";
			var command = header + channel + instruction + port;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		public string SetReplyIPAddress(string ipaddress)
		{
			var instruction = "E05";
			var channel = "00";
			var command = header + channel + instruction + ipaddress;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}
		public string SetReplyPort(int port)
		{
			var instruction = "E06";
			var channel = "00";
			var command = header + channel + instruction + port;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		/// <summary>
		/// 設定光源大小
		/// </summary>
		/// <param name="intensity"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public string SetLightIntensity(string intensity, string channel)
		{
			var instruction = "F";
			int value;
			Int32.TryParse(intensity, out value);
			if (value < 0 || value > 255) return "";

			var command = header + channel + instruction + intensity.PadLeft(3, '0');
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}
		public string SetLightIntensity(int intensity, string channel)
		{
			var instruction = "F";
			if (intensity < 0 || intensity > 255) return "";

			var command = header + channel + instruction + intensity.ToString().PadLeft(3, '0');
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		/// <summary>
		/// 設定光源模式
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public string SetLightMode(string mode, string channel)
		{
			/*
			 * mode range 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10
			 */
			var instruction = "S";
			var command = header + channel + instruction + mode;
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		/// <summary>
		/// 設定光源開關
		/// </summary>
		/// <param name="onOff"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public string SetLightOnOff(LightSwitch onOff, string channel)
		{
			/*
			 * On/Off : 1/0
			 */
			var instruction = "L";
			var command = header + channel + instruction + ((int)onOff).ToString();
			var checksum = getCheckSum(command);
			return sendData(command + checksum + delimiter);
		}

		/// <summary>
		/// 取得 Channel 狀態
		/// </summary>
		/// <param name="channel">00, 01, 02</param>
		/// <returns></returns>
		public ChannelStatusViewModel GetChannelStatus(string channel)
		{
			var instruction = "M";
			var command = header + channel + instruction;
			var checksum = getCheckSum(command);
			var receviceMsg = sendData(command + checksum + delimiter);
			var parts = receviceMsg.Split('.');
			if (parts.Length < 3) return null;
			var info = parts[0];
			var modeInfo = parts[1];
			var onOffInfo = parts[2];
			var commandStatus = info.Substring(3, 1);
			if (commandStatus == "O") commandStatus = "OK";
			else commandStatus = "NG";

			var intensity = info.Substring(5, 3);
			var onOff = onOffInfo.Substring(1, 1);

			return new ChannelStatusViewModel()
			{
				CommandStatus = commandStatus,
				Channel = channel,
				Intensity = intensity,
				Mode = modeInfo,
				StrobeTime = "",
				OnOff = (onOff == "1") ? LightSwitch.On.ToString() : LightSwitch.OFF.ToString(),
			};
		}

		#region private
		private string getCheckSum(string command)
		{
			//取低位元 2 碼
			var checkSumCharArray = Encoding.ASCII.GetBytes(command)
									.Select(p => (int)p).Sum()
									.ToString("X").ToCharArray().Reverse().Take(2).Reverse().ToArray();
			return new string(checkSumCharArray);
		}

		//private string sendData(string data)
		//{
		//	byte[] bytes = new byte[1024];
		//	var replyMsg = "";

		//	try
		//	{
		//		/* sync Call */
		//		using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
		//		{
		//			_client.Connect(this.ControllerEndPoint);
		//			byte[] msg = Encoding.ASCII.GetBytes(data);

		//			// Send the data through the socket.
		//			int bytesSent = _client.Send(msg);

		//			// Receive the response from the remote device.
		//			int bytesRec = _client.Receive(bytes);
		//			replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));

		//			// Release the socket.
		//			_client.Shutdown(SocketShutdown.Both);
		//			_client.Close();
		//		}
		//	}
		//	catch (ArgumentNullException ae)
		//	{
		//		replyMsg = ("ArgumentNullException : " + ae.ToString());
		//	}
		//	catch (SocketException se)
		//	{
		//		replyMsg = ("SocketException : " + se.ToString());
		//	}
		//	catch (Exception ex)
		//	{
		//		replyMsg = ("Unexpected exception : " + ex.ToString());
		//	}
		//	return replyMsg;
		//}

		#region LightController Background Worker
		/*
		 * initialize Background Worker
		 */

		public delegate void LightControllerResponseEventHandler(object sender, LightControllerResponseEventArgs e);
		public delegate void LightControllerConnectionEventHandler(object sender, ConnectionEventArgs e);


		public event LightControllerResponseEventHandler On_LightControllerResponsed;

		private BackgroundWorker bgWorker;
		private void initialBackgroundWorker()
		{
			bgWorker = new BackgroundWorker();
			bgWorker.WorkerSupportsCancellation = true;
			bgWorker.WorkerReportsProgress = true;
			bgWorker.DoWork += bgWorker_DoWork;
			bgWorker.ProgressChanged += bgWorker_ProgressChanged;
			bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
		}

		void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (On_LightControllerResponsed != null)
			{
				On_LightControllerResponsed(sender, new LightControllerResponseEventArgs()
				{
					IsConnected = _client.Connected

				});
			}
		}

		void bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var data = (string[])e.Argument;
			var worker = sender as BackgroundWorker;
			foreach (var sendData in data)
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				else
				{
					var receiveMsg = sendDataAsync(sendData);
					worker.ReportProgress(0, receiveMsg);
				}
			}
		}

		#endregion
		#region AsyncMethods
		/*
		 * Async Methods
		 */

		public void SetLightIntensityAsync(string intensity, string channel)
		{
			var instruction = "F";
			int value;
			Int32.TryParse(intensity, out value);
			var invalid = (value < 0 || value > 255);
			if (!invalid)
			{
				var command = header + channel + instruction + intensity.PadLeft(3, '0');
				var checksum = getCheckSum(command);
				var sendData = command + checksum + delimiter;
				bgWorker.RunWorkerAsync(sendData);
			}
		}

		public void GetLightControllerStatusAsync(string[] channels)
		{
			List<string> commandList = new List<string>();
			foreach (var channel in channels)
			{
				commandList.Add(getChannelStatusCommand(channel));
			}
			bgWorker.RunWorkerAsync(commandList.ToArray());
		}
		#endregion

		private string getChannelStatusCommand(string channel)
		{
			var instruction = "M";
			var command = header + channel + instruction;
			var checksum = getCheckSum(command);
			return command + checksum + delimiter;
		}
		private string sendDataAsync(string data)
		{
			byte[] bytes = new byte[1024];
			var replyMsg = "";
			if (_client == null)
			{
				_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
			try
			{
				/* sync Call */
				//using (_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				//{
				if (!_client.Connected) _client.Connect(this.ControllerEndPoint);


				byte[] msg = Encoding.ASCII.GetBytes(data);

				// Send the data through the socket.
				int bytesSent = _client.Send(msg);

				// Receive the response from the remote device.
				int bytesRec = _client.Receive(bytes);
				replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));

				// Release the socket.
				_client.Shutdown(SocketShutdown.Both);
				_client.Close();
				//}
			}
			catch (ArgumentNullException ae)
			{
				replyMsg = ("ArgumentNullException : " + ae.ToString());
			}
			catch (SocketException se)
			{
				replyMsg = ("SocketException : " + se.ToString());
			}
			catch (Exception ex)
			{
				replyMsg = ("Unexpected exception : " + ex.ToString());
			}
			return replyMsg;
		}


		#endregion

		public void Dispose()
		{
			_client.Close();
			_client.Dispose();
			//throw new NotImplementedException();
		}

		public string GetIPAdress()
		{
			return this.ControllerIPAddress.ToString();
		}

		public string GetPort()
		{
			return this.ControllerPort.ToString();
		}
	}

}
