using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
//
using HalconDotNet;

namespace Hanbo.Image.Grab
{
	public class HalconGrabber : IGrabber
	{
		public event GrabberReportEventHandler On_ConnectReport;
		public event RunWorkerCompletedEventHandler On_ConnectCompleted;

		#region 私有成員
		//
		private HFramegrabber _hFrameGrabber;
		private FrameGrabberArgs _FGArgs;
		private int _delay = 58; // FPS = 17
		//
		#endregion

		#region 公有成員
		//
		/// <summary>
		/// Frame per second
		/// </summary>
		public int FPS { get { return this.FPS; } set { setFPS(FPS); } }
		//
		#endregion
		private void setFPS(int? FPS)
		{
			if (FPS.HasValue && FPS.Value > 0)
				_delay = (int)Math.Floor((1.0 / FPS.Value) * 1000);
		}

		public HalconGrabber(FrameGrabberArgs framegrabberArgs)
		{
			_FGArgs = framegrabberArgs;
		}
		public GrabberEventViewModel Connect()
		{
			var viewModel = new GrabberEventViewModel()
			{
				Result = new GrabberEventResult()
				{
					State = GrabberEventState.Busy,
					Error = new Exception(),
					Model = null
				},
				ReportHandler = On_ConnectReport
			};
			try
			{
				setHFramegrabber();

				viewModel.Result.State = GrabberEventState.Done;
			}
			catch (Exception ex)
			{
				viewModel.Result.Error = ex;
				Hanbo.Log.LogManager.Debug("Connect Exception:" + ex.Message);
                Hanbo.Log.LogManager.Debug("Connect StackTrace:" + ex.StackTrace);
			}
			finally
			{
				if (_hFrameGrabber != null)
				{
					_hFrameGrabber.Dispose();
				}
			}
			return viewModel;
		}
		//
		public GrabberEventViewModel Connect2()
		{
			var viewModel = new GrabberEventViewModel()
			{
				Result = new GrabberEventResult()
				{
					State = GrabberEventState.Busy
				},
				ReportHandler = On_ConnectReport
			};
			HTuple acqHandle = null;
			try
			{
				acqHandle = getAcqHandle();
				viewModel.Result.State = GrabberEventState.Done;
			}
			catch (Exception ex)
			{
				viewModel.Result.Error = ex;
                Hanbo.Log.LogManager.Debug("Connect 2 Exception:" + ex.Message);
                Hanbo.Log.LogManager.Debug("Connect 2 StackTrace:" + ex.StackTrace);
			}
			finally
			{
				if (acqHandle != null)
					HOperatorSet.CloseFramegrabber(acqHandle);
			}
			return viewModel;
		}

		private HTuple getAcqHandle()
		{
			HTuple acqHandle;
			HOperatorSet.OpenFramegrabber(_FGArgs.Name,
														_FGArgs.HorizontalResolution,
														_FGArgs.VerticalResolution,
														_FGArgs.ImageWidth,
														_FGArgs.ImageHeight,
														_FGArgs.StartRow,
														_FGArgs.StartColumn,
														_FGArgs.Field,
														_FGArgs.BitsPerChannel,
														_FGArgs.ColorSpace,
														_FGArgs.Generic,
														_FGArgs.ExternalTrigger,
														_FGArgs.CameraType,
														_FGArgs.Device,
														_FGArgs.Port,
														_FGArgs.LineIn, out acqHandle);
			return acqHandle;
		}

		private void ConnectCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (On_ConnectCompleted != null)
				On_ConnectCompleted(sender, e);
		}

		public GrabberEventViewModel SnapShot()
		{
			var viewModel = new GrabberEventViewModel()
			{
				Result = new GrabberEventResult()
				{
					State = GrabberEventState.Busy
				},
				ReportHandler = On_ConnectReport
			};
			try
			{
				setHFramegrabber();

				_hFrameGrabber.GrabImageStart(_delay);
				var himage = _hFrameGrabber.GrabImage();
				viewModel.Result.State = GrabberEventState.Done;
			}
			catch (Exception ex)
			{
				viewModel.Result.Error = ex;
                Hanbo.Log.LogManager.Debug("SnapShot Exception:" + ex.Message);
                Hanbo.Log.LogManager.Debug("SnapShot StackTrace:" + ex.StackTrace);
			}
			finally
			{
				//c中斷
				if (_hFrameGrabber != null)
				{
					_hFrameGrabber.Dispose();
				}
			}
			return viewModel;
		}
		
		/// <summary>
		/// HOperatorSet.GrabImage
		/// </summary>
		/// <returns></returns>
		public GrabberEventViewModel SnapShot2()
		{
			var viewModel = new GrabberEventViewModel()
			{
				Result = new GrabberEventResult()
				{
					State = GrabberEventState.Busy
				},
				ReportHandler = On_ConnectReport
			};
			var acqHandle = getAcqHandle();
			HObject himage;
			try
			{
				System.Console.WriteLine("Before HOperatorSet.GrabImageStart........");
				System.Console.WriteLine("Press anykey to Continue...." + DateTime.Now.ToString());
                Hanbo.Log.LogManager.Debug("Before GrabImage");
				System.Console.ReadKey();
				
				HOperatorSet.GrabImage(out himage, acqHandle);
                Hanbo.Log.LogManager.Debug("After GrabImage");
				System.Console.WriteLine("HOperatorSet.GrabImageAsync Done........");
				System.Console.WriteLine("Press anykey to Continue....");
				System.Console.ReadKey();
				viewModel.Result.State = GrabberEventState.Done;
			}
			catch (Exception ex)
			{
				viewModel.Result.Error = ex;
                Hanbo.Log.LogManager.Debug("SnapShot 2 Exception:" + ex.Message);
                Hanbo.Log.LogManager.Debug("SnapShot 2 StackTrace:" + ex.StackTrace);
			}
			finally
			{
                Hanbo.Log.LogManager.Debug("Finally Start ");
				if (acqHandle != null)
					HOperatorSet.CloseFramegrabber(acqHandle);
                Hanbo.Log.LogManager.Debug("Finally End");
			}
			return viewModel;
		}
		private void setHFramegrabber()
		{
			if (_hFrameGrabber == null)
			{
				_hFrameGrabber = new HFramegrabber(_FGArgs.Name,
													_FGArgs.HorizontalResolution,
													_FGArgs.VerticalResolution,
													_FGArgs.ImageWidth,
													_FGArgs.ImageHeight,
													_FGArgs.StartRow,
													_FGArgs.StartColumn,
													_FGArgs.Field,
													_FGArgs.BitsPerChannel,
													_FGArgs.ColorSpace,
													_FGArgs.Generic,
													_FGArgs.ExternalTrigger,
													_FGArgs.CameraType,
													_FGArgs.Device,
													_FGArgs.Port,
													_FGArgs.LineIn);
			}
		}
		#region DelegateHandler
		//
		public GrabberEventHandler ConnectHandler
		{
			get
			{
				return Connect;
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public GrabberEventHandler SnapShotHandler
		{
			get
			{
				return SnapShot;
			}
			set
			{
				this.SnapShotHandler = value;
			}
		}

		public RunWorkerCompletedEventHandler ConnectCompletedHandler
		{
			get
			{
				return ConnectCompleted;
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		//
		#endregion



	}
}
