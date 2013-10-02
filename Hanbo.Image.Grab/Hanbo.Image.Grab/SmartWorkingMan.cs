using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//
//
namespace Hanbo.Image.Grab
{
	/*
	 * 定義 WorkingMan 的架構與流程
	 * 操作方法由外部控制
	 */
	public class SmartWorkingMan
	{
		#region 私有成員
		//
		private IGrabber _imageGrabber;
		private BackgroundWorker _worker;
		private RunWorkerCompletedEventHandler _RunWorkerCompletedEventHandler;
		//
		#endregion

		#region 公開成員
		//
		/// <summary>
		/// 是否忙碌
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return _worker.IsBusy;
			}
		}
		//
		#endregion

		#region public methods (公開方法)
		//
		public void Connect()
		{
			if (!_worker.IsBusy)
			{
				this._RunWorkerCompletedEventHandler = _imageGrabber.ConnectCompletedHandler;
				_worker.RunWorkerAsync(_imageGrabber.ConnectHandler);
			}
		}

		public void DisConnect()
		{
		}

		public void SnapShot()
		{
		}

		public void SnapContinuous()
		{
		}

		public void Stop()
		{
			_worker.CancelAsync();
		}
		//
		#endregion

		public SmartWorkingMan(IGrabber imageGrabber)
		{
			_imageGrabber = imageGrabber;
			_worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
			_worker.DoWork += worker_DoWork;
			_worker.ProgressChanged += worker_ProgressChanged;
			_worker.RunWorkerCompleted += worker_RunWorkerCompleted;
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_RunWorkerCompletedEventHandler != null)
			{
				_RunWorkerCompletedEventHandler(sender, e);
			}
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			var viewModel = e.UserState as GrabberEventViewModel;
			if (viewModel != null)
			{
				var result = viewModel.Result;
				var handler = viewModel.ReportHandler;
				if (handler != null)
				{
					handler(sender, e);
				}
				if (result != null)
				{
					if (result.State == GrabberEventState.Done)
					{
						worker.CancelAsync();
					}
				}
			}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			var handler = e.Argument as GrabberEventHandler;
			var progressCout = 0;
			while (true)
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				else
				{
					if (handler != null)
					{
						var viewModel = handler();
						worker.ReportProgress(progressCout++, viewModel);
					}
				}
			}
		}
	}
}
