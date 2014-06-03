using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LightControl
{
	public delegate void CCSLightControlReceivedDataHandler(object sender, object receiveMessage);
	public delegate void CCSLightControlProbeConnectionHandler(object sender, object args);
	public delegate void CCSLightControlSafetyCloseHandler(object sender, object args);
	/// <summary>
	/// CCS Light Control Manager, 乙太網路通訊
	/// </summary>
	public class CCSLightControlManager : IDisposable
	{
		/// <summary>
		/// Command 標頭
		/// </summary>
		protected string header = "@";

		/// <summary>
		/// Command 結束符號
		/// </summary>
		protected string delimiter = "\r\n";

		/// <summary>
		/// 是否連線
		/// </summary>
		public bool Connected
		{
			get
			{
				var socketLastConnectionStatus = _client != null ? _client.Connected : false;
				var hardwareLastConnectionStatus = _heartBeatWorker == null ? true : _hardwareConnectionStatus;
				return hardwareLastConnectionStatus && socketLastConnectionStatus;
			}
		}
		public IPAddress ControllerIPAddress { get; set; }
		public int ControllerPort { get; set; }
		public IPEndPoint ControllerEndPoint;
		public bool IsDoSafetyCloseDone { get { return _isSafetyCloseDone; } }
		public bool IsReady()
		{
			return !_bgWorker.IsBusy && Connected;
		}


		private Socket _client;
		private string _ReceiveMessage;
		private string _ExceptionMessage;
		private bool _testConnected = false;
		private bool _SafetyCloseFlag = false;
		private bool _isSafetyCloseDone = false;
		private bool _hardwareConnectionStatus = false;
		private System.Net.NetworkInformation.Ping _ping = new System.Net.NetworkInformation.Ping();
		private int _pingTimeout = 50; //ms


		//
		private BackgroundWorker _bgWorker;
		private BackgroundWorker _heartBeatWorker;
		private int _heartbeat;
		private int _heartbeatCountTimeout = 100;
		private int _heartbeatCount = 0;

		//
		public event CCSLightControlReceivedDataHandler On_ReceivedData;
		public event CCSLightControlProbeConnectionHandler On_ProbeConnectionWorked;
		public event CCSLightControlSafetyCloseHandler On_SafetyClosed;

		public CCSLightControlManager()
		{
			this.ControllerIPAddress = new IPAddress(new byte[] { 192, 168, 0, 2 });
			this.ControllerPort = 40001;
			ControllerEndPoint = new IPEndPoint(this.ControllerIPAddress, this.ControllerPort);
			initialize();
			//
		}

		#region Public API
		/// <summary>
		/// 持續偵測網路是否連續連線
		/// </summary>
		/// <param name="probeInterval">每隔多少時間偵測一次 (單位 ms)</param>
		public void StartProbeConnetion(int? probeInterval)
		{
			_heartbeat = (probeInterval.HasValue) ? 200 : probeInterval.Value;
			if (_heartBeatWorker == null)
				initializeHeartBeatBackgroundWorker();

			if (!_heartBeatWorker.IsBusy)
			{
				_heartBeatWorker.RunWorkerAsync();
			}
		}
		/// <summary>
		/// 停止偵測網路連線
		/// </summary>
		public void StopProbeConnection()
		{
			_heartBeatWorker.CancelAsync();
		}
		private void resetSocket()
		{
			try
			{
				_client.Close();
				initializeSocket();
			}
			catch (Exception ex)
			{

				_ExceptionMessage = ex.Message;
			}
		}
		public string TestConnect(int? timeout)
		{
			_testConnected = false;
			var waitForTimeout = (timeout == null) ? 500 : timeout;

			var connectTestThread = new Thread(() =>
			{
				try
				{
					using (Socket testClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						byte[] bytes = new byte[1024];
						var data = getCheckUnitStatusData();
						testClient.Connect(this.ControllerEndPoint);
						byte[] msg = Encoding.ASCII.GetBytes(data);

						// Send the data through the socket.
						int bytesSent = testClient.Send(msg);

						// Receive the response from the remote device.
						int bytesRec = testClient.Receive(bytes);
						_ReceiveMessage = (Encoding.ASCII.GetString(bytes, 0, bytesRec));
						_testConnected = true;
						testClient.Shutdown(SocketShutdown.Both);
						testClient.Close();
					}
				}
				catch (SocketException ex)
				{
					_ExceptionMessage = ex.Message;
				}
				finally
				{

				}
			});
			connectTestThread.Start();
			connectTestThread.Join(waitForTimeout.Value);
			var message = (_testConnected) ? "OK" : "Timeout";
			connectTestThread.Abort();
			Thread.Sleep(200);
			return message;
		}
		/// <summary>
		/// 連線至光源控制器
		/// </summary>
		/// <param name="timeout">連線逾時 ms, default = 500 ms</param>
		public string Connect(int? timeout)
		{
			var waitForTimeout = (timeout == null) ? 500 : timeout;
			_ExceptionMessage = "";

			var connectTestThread = new Thread(() =>
			{
				try
				{
					if (!_client.Connected)
					{
						//resetSocket();
						byte[] bytes = new byte[1024];
						var data = getCheckUnitStatusData();
						_client.Connect(this.ControllerEndPoint);
						byte[] msg = Encoding.ASCII.GetBytes(data);

						// Send the data through the socket.
						int bytesSent = _client.Send(msg);

						// Receive the response from the remote device.
						int bytesRec = _client.Receive(bytes);
						_ReceiveMessage = (Encoding.ASCII.GetString(bytes, 0, bytesRec));
					}
				}
				catch (SocketException ex)
				{
					_ExceptionMessage = ex.Message;
				}
			});
			connectTestThread.Start();
			connectTestThread.Join(waitForTimeout.Value);
			var message = (_client.Connected) ? "OK" : "Timeout";
			connectTestThread.Abort();
			return message;
		}



		/// <summary>
		/// 取得 Channel 狀態
		/// </summary>
		/// <param name="channel">00, 01, 02</param>
		/// <returns></returns>
		public void GetChannelStatusAsync(string[] channels)
		{
			var instruction = "M";
			var channelsCommand = new Dictionary<string, InstructionModel>();

			foreach (var channel in channels)
			{
				channelsCommand.Add(channel, new InstructionModel() { Instruction = instruction, Data = "" });
			}
			//prepare data
			var model = new CCSCommandModel()
			{
				ChannelCommands = channelsCommand,
			};
			if (!_bgWorker.IsBusy)
			{
				_bgWorker.RunWorkerAsync(model);
			}
		}

		/// <summary>
		/// 設定光源 on/off, 非同步方法
		/// </summary>
		/// <param name="onOff"></param>
		/// <param name="channel"></param>
		public void SetLightOnOffAsync(LightSwitch onOff, string channel)
		{
			var instruction = "L";
			var model = new CCSCommandModel()
			{
				ChannelCommands = new Dictionary<string, InstructionModel>()
				{
					{channel, new InstructionModel(){Instruction = instruction, Data = ((int)onOff).ToString()}}
				},
			};

			if (!_bgWorker.IsBusy)
			{
				_bgWorker.RunWorkerAsync(model);
			}
		}



		/// <summary>
		/// 設定光源亮度, 非同步方法
		/// </summary>
		/// <param name="intensity"></param>
		/// <param name="channel"></param>
		public void SetLightIntensityAsync(int intensity, string channel)
		{
			var instruction = "F";
			if (intensity < 0 || intensity > 255) return;

			var data = intensity.ToString().PadLeft(3, '0');
			var model = new CCSCommandModel()
			{
				ChannelCommands = new Dictionary<string, InstructionModel>()
				{
					{channel, new InstructionModel(){Instruction = instruction, Data = data}}
				},
			};

			if (!_bgWorker.IsBusy)
			{
				_bgWorker.RunWorkerAsync(model);
			}
		}
		#endregion

		//=================================== Private Functions ===========================================
		private void initialize()
		{
			initializeSocket();
			initializeBackgroundWorker();
		}

		private void initializeHeartBeatBackgroundWorker()
		{
			_heartBeatWorker = new BackgroundWorker();
			_heartBeatWorker.WorkerSupportsCancellation = true;
			_heartBeatWorker.WorkerReportsProgress = true;
			_heartBeatWorker.DoWork += _heartBeatWorker_DoWork;
			_heartBeatWorker.ProgressChanged += _heartBeatWorker_ProgressChanged;
			_heartBeatWorker.RunWorkerCompleted += _heartBeatWorker_RunWorkerCompleted;
		}

		void _heartBeatWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_SafetyCloseFlag)
			{
				_isSafetyCloseDone = true;
				if (On_SafetyClosed != null)
				{
					On_SafetyClosed(null, true);
				}
			}
		}

		void _heartBeatWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var pingState = (bool)e.UserState;
			if (On_ProbeConnectionWorked != null)
			{
				On_ProbeConnectionWorked(null, pingState);
			}
		}

		void _heartBeatWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			while (true)
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				else
				{
					try
					{
						var pingStatus = _ping.Send(ControllerIPAddress.ToString(), _pingTimeout).Status;
						_hardwareConnectionStatus = (pingStatus == System.Net.NetworkInformation.IPStatus.Success);
					}
					catch (System.Net.NetworkInformation.PingException ex)
					{
						_hardwareConnectionStatus = false;
						_ExceptionMessage = ex.Message;
						_heartbeatCount++;
						if (_heartbeatCount >= _heartbeatCountTimeout)
						{
							_heartbeatCount = 0;
							e.Cancel = true;
							break;
						}
					}
					finally
					{
						if (_hardwareConnectionStatus) _heartbeatCount = 0;
						worker.ReportProgress(1, _hardwareConnectionStatus);
						Thread.Sleep(_heartbeat);
					}
				}
			}
		}

		private void initializeBackgroundWorker()
		{
			_bgWorker = new BackgroundWorker();
			_bgWorker.WorkerSupportsCancellation = true;
			_bgWorker.WorkerReportsProgress = true;

			_bgWorker.DoWork += _bgWorker_DoWork;
			_bgWorker.ProgressChanged += _bgWorker_ProgressChanged;
			_bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
		}

		private void initializeSocket()
		{
			_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (On_ReceivedData != null)
			{
				On_ReceivedData(sender, e.UserState);
			}
		}

		void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var model = e.Argument as CCSCommandModel;
			var worker = sender as BackgroundWorker;
			if (model != null)
			{
				int count = 0;
				foreach (var channelCommand in model.ChannelCommands)
				{
					if (worker.CancellationPending)
					{
						e.Cancel = true;
						break;
					}
					else
					{
						var channel = channelCommand.Key;
						var sentCommand = channelCommand.Value;
						var data = getSendData(channel, sentCommand);
						var replyMsg = sendData(data);
						worker.ReportProgress(++count, replyMsg);
					}
				}
			}
		}
		private string getSendData(string channel, InstructionModel sentCommand)
		{
			var command = this.header + channel + sentCommand.Instruction + sentCommand.Data;
			return command + getCheckSum(command) + this.delimiter;
		}

		private string sendData(string data)
		{
			var replyMsg = "";
			try
			{
				if (_client.Connected)
				{
					byte[] bytes = new byte[1024];
					byte[] msg = Encoding.ASCII.GetBytes(data);

					// Send the data through the socket.
					int bytesSent = _client.Send(msg);

					// Receive the response from the remote device.
					int bytesRec = _client.Receive(bytes);
					replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));
				}
			}
			catch (SocketException ex)
			{
				_ExceptionMessage = ex.Message;
			}
			return replyMsg;
		}
		#region .....
		/// <summary>
		/// 檢查狀態 Data
		/// </summary>
		/// <returns></returns>
		private string getCheckUnitStatusData()
		{
			var command = "00CE3";
			return header + command + delimiter;
		}
		private string getCheckSum(string command)
		{
			//取低位元 2 碼
			var checkSumCharArray = Encoding.ASCII.GetBytes(command)
									.Select(p => (int)p).Sum()
									.ToString("X").ToCharArray().Reverse().Take(2).Reverse().ToArray();
			return new string(checkSumCharArray);
		}
		#endregion

		public void Dispose()
		{
			if (_client != null)
				_client.Close();
		}
		public void ReleaseBK()
		{
			if (_bgWorker != null)
				_bgWorker.CancelAsync();
			if (_heartBeatWorker != null)
				_heartBeatWorker.CancelAsync();
		}

		public void RemoveAllRegisterEvent()
		{
			On_ReceivedData = null;
		}

		/// <summary>
		/// 確認所有執行緒都停止工作後才進行關閉
		/// </summary>
		public void DoSafetyClose()
		{
			if (_isSafetyCloseDone) return;

			var isSafeCloseNow = (_heartBeatWorker == null) ? true : (!_heartBeatWorker.IsBusy);

			//
			if (isSafeCloseNow)
			{
				_isSafetyCloseDone = true;
				if (On_SafetyClosed != null)
				{
					On_SafetyClosed(null, true);
				}
			}
			else
			{
				_SafetyCloseFlag = true;
				ReleaseBK();
			}
			Dispose();
		}

		public void SendMessage(object model)
		{
			if (!_bgWorker.IsBusy)
			{
				_bgWorker.RunWorkerAsync(model);
			}
		}

		public CCSCommandModel GetIntesnityModel(int intensity, string channel)
		{
			var instruction = "F";
			if (intensity < 0) intensity = 0;
			if (intensity > 255) intensity = 255;

			var data = intensity.ToString().PadLeft(3, '0');
			var model = new CCSCommandModel()
			{
				ChannelCommands = new Dictionary<string, InstructionModel>()
				{
					{channel, new InstructionModel(){Instruction = instruction, Data = data}}
				},
			};
			return model;
		}

		public CCSCommandModel GetLightOnOffModel(LightSwitch onOff, string channel)
		{
			var instruction = "L";
			var model = new CCSCommandModel()
			{
				ChannelCommands = new Dictionary<string, InstructionModel>()
				{
					{channel, new InstructionModel(){Instruction = instruction, Data = ((int)onOff).ToString()}}
				},
			};
			return model;
		}

		public CCSCommandModel GetChannelStatusModel(string[] channels)
		{
			var instruction = "M";
			var channelsCommand = new Dictionary<string, InstructionModel>();

			foreach (var channel in channels)
			{
				channelsCommand.Add(channel, new InstructionModel() { Instruction = instruction, Data = "" });
			}
			//prepare data
			var model = new CCSCommandModel()
			{
				ChannelCommands = channelsCommand,
			};
			return model;
		}
	}
}
