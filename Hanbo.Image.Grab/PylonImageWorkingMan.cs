using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public class PylonImageWorkingMan : IDisposable
	{		
		private BackgroundWorker _bgworker;
		private PylonImageProvider _imageProvider;

		public event GrabImageChangedEventHandler GrabImageChanged;
		public event GrabImageStoppedEventHandler GrabImageStopped;
		public event GrabImageStartEventHandler GrabImageStart;
		public event GrabImageExceptionEventHandler GrabImageException;

		public event GrabImageStatusNotifyEventHandler On_GrabImageStatusChanged;
		public event GrabImageConnectedEventHandler On_GrabImageConnected;
		public event GrabImageConnectingEventHandler On_GrabImageConnecting;
		public WorkingManStatus Status { get; set; }

		private int _gevSCPSPacketSize = 1500; //預設值
		public PylonImageWorkingMan(int gevSCPSPacketSize)
		{
			_gevSCPSPacketSize = gevSCPSPacketSize;
			init();
		}
		public PylonImageWorkingMan()
		{
			init();
		}

		#region public menthod (公開方法)
		public void Connect()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 是否忙碌
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return _bgworker.IsBusy;
			}
		}

		/// <summary>
		/// 連續擷取
		/// </summary>
		public void Start()
		{
			if (!Status.IsConnection)
			{
				connection();
			}
			else
			{
				setStatus(GrabInstruction.GrabImage, GrabStage.Grabbing, GrabState.Busy);
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
			}

			if (!_bgworker.IsBusy && Status.IsConnection)
			{
				_bgworker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// 單張擷取
		/// </summary>
		public void SnapShot()
		{
			//開始通知
			setStatus(GrabInstruction.GrabImage, GrabStage.Grabbing, GrabState.Busy);
			statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
			try
			{
				if (_bgworker.IsBusy)
				{
					_bgworker.CancelAsync();
				}
				if (!Status.IsConnection)
				{
					connection();
				}
				if (Status.IsConnection)
				{
					_imageProvider.GrabImage();
					setStatus(Status.Instruction, GrabStage.Grabbed, GrabState.Idle);
				}
				else
				{
					setStatus(Status.Instruction, GrabStage.Closed, GrabState.Idle);
				}
			}
			catch (HalconDotNet.HOperatorException ex)
			{
				if (GrabImageException != null)
				{
					GrabImageException(ex);
				}
				Status.Message = ex.Message;
				Status.Stage = GrabStage.Closed;
				Hanbo.Log.LogManager.Debug("[GrabImageWorkingMan.SnapShot()] HOperatorException:" + ex.Message + " [StackTrack]" + ex.StackTrace);
			}
			finally
			{
				//結束通知
				setStatus(Status.Instruction, Status.Stage, GrabState.Idle);
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
			}
		}



		/// <summary>
		/// NotImplementedException
		/// </summary>
		public void Pause()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 停止擷取
		/// </summary>
		public void Cancel()
		{
			_bgworker.CancelAsync();
		}

		/// <summary>
		/// 中斷連線
		/// </summary>
		public void DisConnection()
		{
			_bgworker.CancelAsync();
			_imageProvider.Stop();

			Status.IsConnection = false;
			statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = new WorkingManStatus() { Instruction = GrabInstruction.DisConnect, Stage = GrabStage.Closed, State = GrabState.Idle } });
		}
		public void Dispose()
		{
			try
			{
				_bgworker.CancelAsync();
				_bgworker.Dispose();
				if (_imageProvider.Connected)
				{
					_imageProvider.DisConnect();
					_imageProvider.Dispose();
				}
			}
			catch (Exception ex)
			{
                Hanbo.Log.LogManager.Debug("[Dispose] Exception: " + ex.Message);
			}
		}
		public HObject GetCurrentImage()
		{
			return _imageProvider.Image;
		}
		#endregion

		#region 私有方法
		private void init()
		{
            Hanbo.Log.LogManager.Debug("Init PylonImageProvider");
			_imageProvider = new PylonImageProvider();
			_imageProvider.GevSCPSPacketSize = _gevSCPSPacketSize;
			Status = new WorkingManStatus()
			{
				IsConnection = false,
				Message = "",
				Instruction = GrabInstruction.Initialize,
				Stage = GrabStage.Initial,
				State = GrabState.Idle
			};
            Hanbo.Log.LogManager.Debug("Init Worker");
			initializeBackgroundWorker();
            Hanbo.Log.LogManager.Debug("Init Done");
		}

		private void initializeBackgroundWorker()
		{
			_bgworker = new BackgroundWorker();
			_bgworker.WorkerReportsProgress = true;
			_bgworker.WorkerSupportsCancellation = true;
			_bgworker.DoWork += new DoWorkEventHandler(_bgworker_DoWork);
			_bgworker.ProgressChanged += new ProgressChangedEventHandler(_bgworker_ProgressChanged);
			_bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgworker_RunWorkerCompleted);
		}

		private void _bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (GrabImageStopped != null)
				GrabImageStopped(sender, e);
		}

		private void _bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (GrabImageChanged != null)
			{
				GrabImageChanged(this, e);
			}
		}

		private void _bgworker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			try
			{
				if (GrabImageStart != null) GrabImageStart(sender, e);
				var i = 0;
				while (true)
				{
					if (worker.CancellationPending)
					{
						e.Cancel = true;
						break;
					}
					else
					{
						_imageProvider.GrabImage();
						var himage = new HImage(_imageProvider.Image);
						worker.ReportProgress(i++, himage);
					}
				}
			}
			catch (Exception ex)
			{
                Hanbo.Log.LogManager.Error(ex);
				if (GrabImageException != null)
				{
					GrabImageException(ex);
				}
			}
		}

		/// <summary>
		/// 狀態改變通知
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void statusChangedNotify(object sender, GrabImageStatusChangedEventArgs e)
		{
			if (On_GrabImageStatusChanged != null)
			{
				On_GrabImageStatusChanged(sender, e);
			}
		}

		private void connection()
		{
			//設定狀態
            Hanbo.Log.LogManager.Debug("CCD Connection");
			setStatus(GrabInstruction.Connect, GrabStage.Connecting, GrabState.Busy);
			try
			{
				if (On_GrabImageConnecting != null)
				{
					On_GrabImageConnecting();
				}
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
				_imageProvider.Connect();
				var msg = _imageProvider.GetLatestMessage();
				if (!String.IsNullOrEmpty(msg))
				{
					Status.Message = msg;
				}

				Status.IsConnection = _imageProvider.Connected;
				if (Status.IsConnection)
					Status.Stage = GrabStage.Connected;
			}
			catch (Exception ex)
			{
				setStatus(GrabInstruction.Connect, GrabStage.Closed, GrabState.Idle);
				if (GrabImageException != null)
				{
					GrabImageException(ex);
				}
				Status.Message = ex.Message;
				Status.Stage = GrabStage.Closed;
                Hanbo.Log.LogManager.Debug("[GrabImageWorkingMan.connection()] HOperatorException:" + ex.Message + " [StackTrack]" + ex.StackTrace);
			}
			finally
			{
				if (On_GrabImageConnected != null)
				{
					On_GrabImageConnected();
				}
				setStatus(GrabInstruction.Connect, Status.Stage, GrabState.Idle);
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
			}
		}

		private void setStatus(GrabInstruction instruction, GrabStage stage, GrabState state)
		{
			Status.Instruction = instruction;
			Status.Stage = stage;
			Status.State = state;
		}
		#endregion


	}
}
