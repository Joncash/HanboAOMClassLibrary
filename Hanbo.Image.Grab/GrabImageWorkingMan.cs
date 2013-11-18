using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hanbo.Image.Grab
{
	public delegate void GrabImageExceptionEventHandler(Exception ex);
	public delegate void GrabImageConnectedEventHandler();
	public delegate void GrabImageConnectingEventHandler();
	public delegate void GrabImageChangedEventHandler(object sender, ProgressChangedEventArgs e);
	public delegate void GrabImageStartEventHandler(object sender, DoWorkEventArgs e);
	public delegate void GrabImageStoppedEventHandler(object sender, RunWorkerCompletedEventArgs e);


	public delegate void GrabImageStatusNotifyEventHandler(object sender, GrabImageStatusChangedEventArgs e);

	public class GrabImageWorkingMan
	{
		#region Events (事件)
		//
		public event GrabImageStatusNotifyEventHandler On_GrabImageStatusChanged;
		//
		public event GrabImageConnectingEventHandler On_GrabImageConnecting;
		public event GrabImageConnectedEventHandler On_GrabImageConnected;
		public event GrabImageExceptionEventHandler GrabImageException;

		/// <summary>
		/// GrabImageStart, Start()
		/// </summary>
		public event GrabImageStartEventHandler GrabImageStart;

		/// <summary>
		/// GrabImageChanged
		/// </summary>
		public event GrabImageChangedEventHandler GrabImageChanged;

		/// <summary>
		/// GrabImageStopped, Stop()
		/// </summary>
		public event GrabImageStoppedEventHandler GrabImageStopped;
		//
		#endregion

		#region Private property member (私有屬性成員)
		//
		private BackgroundWorker _bgworker;
		private HImage _currentImage;
		private int _delay = 58; // FPS = 17
		private FrameGrabberArgs _FGArgs;
		private HFramegrabber _hFrameGrabber;
		
		//
		#endregion

		#region Public member (公有成員)
		//

		/// <summary>
		/// 是否連線
		/// </summary>
		public bool Connected { get { return Status.IsConnection; } }
		public WorkingManStatus Status { get; set; }

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

		public string Message { get; set; }

		/// <summary>
		/// Frame per second
		/// </summary>
		public int FPS { get; set; }
		//
		#endregion

		#region 建構子

		/// <summary>
		/// 設定鏡頭參數
		/// </summary>
		/// <param name="fgArgs">FrameGrabberArgs</param>
		public GrabImageWorkingMan(FrameGrabberArgs fgArgs)
		{
			initialize(fgArgs, null);
		}
		//
		/// <summary>
		/// 設定鏡頭參數
		/// </summary>
		/// <param name="fgArgs">FrameGrabberArgs</param>
		/// <param name="FPS">Frame per second, default = 17</param>
		public GrabImageWorkingMan(FrameGrabberArgs fgArgs, int? FPS)
		{
			initialize(fgArgs, FPS);
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="fgArgs"></param>
		/// <param name="FPS"></param>
		private void initialize(FrameGrabberArgs fgArgs, int? FPS)
		{
			Status = new WorkingManStatus()
			{
				IsConnection = false,
				Message = "",
				Instruction = GrabInstruction.Initialize,
				Stage = GrabStage.Initial,
				State = GrabState.Idle
			};
			setFPS(FPS);
			_FGArgs = fgArgs;
			initializeBackgroundWorker();
		}
		#endregion

		private void setFPS(int? FPS)
		{
			if (FPS.HasValue && FPS.Value > 0)
				_delay = (int)Math.Floor((1.0 / FPS.Value) * 1000);
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
		#region Notification
		protected void OnGrabImageChanged(ProgressChangedEventArgs e)
		{
			if (GrabImageChanged != null)
			{
				GrabImageChanged(this, e);
			}
		}
		#endregion

		#region Event Binding
		private void _bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (GrabImageStopped != null)
				GrabImageStopped(sender, e);
		}

		private void _bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			OnGrabImageChanged(e);
		}

		private void _bgworker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			try
			{
				_hFrameGrabber.GrabImageStart(_delay);
				if (GrabImageStart != null) GrabImageStart(sender, e);
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
							var himage = _hFrameGrabber.GrabImageAsync(_delay);
							worker.ReportProgress(1, himage);
							_currentImage = himage;
						}
						catch (HalconDotNet.HOperatorException ex)
						{
                            Hanbo.Log.LogManager.Error(ex);
							if (GrabImageException != null)
							{
								GrabImageException(ex);
							}
						}
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
		#endregion

		#region public menthod (公開方法)
		public void Connect()
		{
			if (!Status.IsConnection)
			{
				connection();
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
					_hFrameGrabber.GrabImageStart(_delay);
					_currentImage = _hFrameGrabber.GrabImage();
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
			setStatus(GrabInstruction.Connect, GrabStage.Connecting, GrabState.Busy);
			try
			{
				if (On_GrabImageConnecting != null)
				{
					On_GrabImageConnecting();
				}
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = this.Status });
				setHFramegrabber();

				Status.IsConnection = true;
				Status.Stage = GrabStage.Connected;
			}
			catch (HalconDotNet.HOperatorException ex)
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
			if (_bgworker.IsBusy)
				_bgworker.CancelAsync();
		}

		/// <summary>
		/// 中斷連線
		/// </summary>
		public void DisConnection()
		{
			_bgworker.CancelAsync();
			if (_hFrameGrabber != null)
			{
				_hFrameGrabber.Dispose();
				Status.IsConnection = false;
				statusChangedNotify(null, new GrabImageStatusChangedEventArgs() { Status = new WorkingManStatus() { Instruction = GrabInstruction.DisConnect, Stage = GrabStage.Closed, State = GrabState.Idle } });
			}
		}
		public void Dispose()
		{
			try
			{
				_bgworker.CancelAsync();
				_hFrameGrabber.Dispose();
				_bgworker.Dispose();
			}
			catch (Exception ex)
			{
                Hanbo.Log.LogManager.Debug("[Dispose] Exception: " + ex.Message);
			}
		}
		public HObject GetCurrentImage()
		{
			return _currentImage;
		}
		#endregion

		#region private methods
		//

		/// <summary>
		/// 設定 HFramegrabber 參數
		/// </summary>
		private void setHFramegrabber()
		{
			if (_hFrameGrabber == null || !_hFrameGrabber.IsInitialized())
			{
				_hFrameGrabber = new HFramegrabber(_FGArgs.Name,
													_FGArgs.HorizontalResolution,
													_FGArgs.VerticalResolution,
													_FGArgs.ImageWidth,
													_FGArgs.ImageHeight,
													_FGArgs.StartRow,
													_FGArgs.StartColumn,
													_FGArgs.Field,
													new HTuple(_FGArgs.BitsPerChannel),
													new HTuple(_FGArgs.ColorSpace),
													new HTuple(_FGArgs.Generic),
													_FGArgs.ExternalTrigger,
													new HTuple(_FGArgs.CameraType),
													new HTuple(_FGArgs.Device),
													new HTuple(_FGArgs.Port),
													new HTuple(_FGArgs.LineIn));
			}
		}
		#endregion

		/// <summary>
		/// Celar all Register evetns
		/// </summary>
		public void RemoveAllRegisterEvent()
		{
			On_GrabImageStatusChanged = null;
			On_GrabImageConnecting = null;
			On_GrabImageConnected = null;
			GrabImageException = null;
			GrabImageStart = null;
			GrabImageChanged = null;
			GrabImageStopped = null;
		}
	}

}
