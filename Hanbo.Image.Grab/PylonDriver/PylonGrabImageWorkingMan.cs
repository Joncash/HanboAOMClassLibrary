using HalconDotNet;
using PylonC.NETSupportLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Hanbo.Image.Grab.PylonDriver
{
	/// <summary>
	/// 使用 PylonC.Net 取得影像
	/// </summary>
	public class PylonGrabImageWorkingMan : GrabImageWorkingMan
	{
		#region 覆寫 Base Class 事件
		public override event GrabImageChangedEventHandler GrabImageChanged;
		public override event GrabImageExceptionEventHandler GrabImageException;
		public override event GrabImageStatusNotifyEventHandler On_GrabImageStatusChanged;
		#endregion

		#region 覆寫 Base Class 公開變數
		public override bool IsBusy
		{
			get
			{
				return _isBusy;
			}
		}
		#endregion

		#region priavete variables
		private ImageProvider m_imageProvider;
		private bool _isBusy = false;
		#endregion

		#region 建構子 及 初始化 ImageProvider
		public PylonGrabImageWorkingMan()
		{
			Status = new WorkingManStatus();
			initPylonDriver();
		}

		private void initPylonDriver()
		{
			m_imageProvider = new ImageProvider();

			/* Register for the events of the image provider needed for proper operation. */
			m_imageProvider.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
			m_imageProvider.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
			m_imageProvider.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
			m_imageProvider.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
			m_imageProvider.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
			m_imageProvider.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
			m_imageProvider.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);
		}

		/// <summary>
		/// 影像擷取完成通知
		/// </summary>
		private void OnImageReadyEventCallback()
		{
			try
			{
				/* Acquire the image from the image provider. Only show the latest image. The camera may acquire images faster than images can be displayed*/
				ImageProvider.Image image = m_imageProvider.GetLatestImage();

				/* Check if the image has been removed in the meantime. */
				if (image != null)
				{
					//利用 GC Handle , 釘住記憶體不讓釋放，取得該記憶體位址
					GCHandle pinnedArray = GCHandle.Alloc(image.Buffer, GCHandleType.Pinned);
					IntPtr pointer = pinnedArray.AddrOfPinnedObject();
					try
					{
						//'byte', 'complex', 'cyclic', 'direction', 'int1', 'int2', 'int4', 'int8', 'real', 'uint2'
						var imgType = "byte";
						HImage img = new HImage(imgType, image.Width, image.Height, pointer);

						if (GrabImageChanged != null)
						{
							GrabImageChanged(m_imageProvider
							, new System.ComponentModel.ProgressChangedEventArgs(1, img));
						}
					}
					catch (Exception ex)
					{
						errorNotify(ex);
					}
					finally
					{
						//用完要釋放，否則該記憶體不會被回收
						pinnedArray.Free();
					}

					/* The processing of the image is done. Release the image buffer. */
					m_imageProvider.ReleaseImage();
					/* The buffer can be used for the next image grabs. */
				}
			}
			catch (Exception e)
			{
				OnGrabErrorEventCallback(e, m_imageProvider.GetLastErrorMessage());
			}
		}

		/// <summary>
		/// 開始擷取通知
		/// </summary>
		private void OnGrabbingStartedEventCallback()
		{
			_isBusy = true;
			setStatus(GrabInstruction.Connect, GrabStage.Grabbing, GrabState.Busy);
			statusChangedNotify();
		}

		/// <summary>
		/// 停止擷取通知
		/// </summary>
		private void OnGrabbingStoppedEventCallback()
		{
			_isBusy = false;
			setStatus(GrabInstruction.Connect, GrabStage.Stop, GrabState.Idle);
			statusChangedNotify();
		}

		/// <summary>
		/// 裝置開啟通知
		/// </summary>
		private void OnDeviceOpenedEventCallback()
		{
			_isBusy = false;
			setStatus(GrabInstruction.Connect, GrabStage.Stop, GrabState.Idle);
			statusChangedNotify();
		}

		/// <summary>
		/// 裝置關閉通知
		/// </summary>
		private void OnDeviceClosedEventCallback()
		{
			setStatus(GrabInstruction.DisConnect, GrabStage.Closed, GrabState.Idle);
			statusChangedNotify();
		}

		/// <summary>
		/// 裝置移除通知
		/// </summary>
		private void OnDeviceRemovedEventCallback()
		{
			setStatus(GrabInstruction.DisConnect, GrabStage.Closed, GrabState.Idle);
			statusChangedNotify();
		}

		/// <summary>
		/// 錯誤通知
		/// </summary>
		/// <param name="grabException"></param>
		/// <param name="additionalErrorMessage"></param>
		private void OnGrabErrorEventCallback(Exception grabException, string additionalErrorMessage)
		{
			errorNotify(grabException);
			Hanbo.Log.LogManager.Error("Additional Error Message : " + additionalErrorMessage);
		}

		/// <summary>
		/// 狀態通知
		/// </summary>
		private void statusChangedNotify()
		{
			if (On_GrabImageStatusChanged != null)
			{
				On_GrabImageStatusChanged(null, new GrabImageStatusChangedEventArgs() { Status = this.Status, });
			}
		}

		/// <summary>
		/// 錯誤通知
		/// </summary>
		/// <param name="ex"></param>
		private void errorNotify(Exception ex)
		{
			if (GrabImageException != null)
			{
				Hanbo.Log.LogManager.Error(ex);
				GrabImageException(ex);
			}
		}
		#endregion

		#region 覆寫 Base Class 方法
		public override void Start()
		{
			m_imageProvider.ContinuousShot();
		}

		public override void Cancel()
		{
			m_imageProvider.Stop();
		}

		public override void Connect()
		{
			//Stop,and Close 
			m_imageProvider.Stop();
			m_imageProvider.Close();
			setStatus(GrabInstruction.Connect, GrabStage.Closed, GrabState.Idle);
			Status.IsConnection = false;


			List<DeviceEnumerator.Device> list = DeviceEnumerator.EnumerateDevices();
			var device = list.FirstOrDefault();
			if (device != null)
			{
				m_imageProvider.Open(device.Index);
				setStatus(GrabInstruction.Connect, GrabStage.Connected, GrabState.Idle);
				Status.IsConnection = true;
			}
		}
		#endregion
	}
}
