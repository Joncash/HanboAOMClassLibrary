using Hanbo.Configuration.Settings;
using Hanbo.Image.Grab;
using Hanbo.Image.Grab.PylonDriver;
using LightControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Hanbo.Device.SingleInstance
{
	public static class DeviceController
	{
		private static FrameGrabberArgs _fgArgs;
		private static GrabImageWorkingMan _grabImageWorkingMan;
		private static bool _grabImageManInstanceUsed = false;

		private static CCSLightControlManager _lightControlManager;
		private static bool _lightControlInstanceUsed = false;

		public static bool GrabImageManUsed { get { return _grabImageManInstanceUsed; } }

		public static bool LightControlManagerUsed { get { return _lightControlInstanceUsed; } }

		//建構子
		static DeviceController()
		{
			try
			{
				initGrabImageHandle();
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
		}

		/// <summary>
		/// 釋放 CCD 資源
		/// </summary>
		public static void ReleaseGrabImageWorkingManInstance()
		{
			_grabImageManInstanceUsed = false;
			if (_grabImageWorkingMan is PylonGrabImageWorkingMan)
			{
				_grabImageWorkingMan.Cancel();
			}
		}

		public static GrabImageWorkingMan GetGrabImageWorkingManInstance()
		{
			_grabImageWorkingMan.RemoveAllRegisterEvent();
			_grabImageManInstanceUsed = (_grabImageWorkingMan != null);
			return _grabImageWorkingMan;
		}

		public static CCSLightControlManager GetCCSLightControlManagerInstance()
		{
			if (_lightControlManager == null)
			{
				_lightControlManager = new CCSLightControlManager();
			}
			else
			{
				_lightControlManager.RemoveAllRegisterEvent();
			}
			_lightControlInstanceUsed = (_lightControlManager != null);
			return _lightControlManager;
		}
		public static void ReleaseCCSLightControlManagerInstance()
		{
			_lightControlInstanceUsed = false;
			_lightControlManager = null;
		}

		#region private functions
		private static void initGrabImageHandle()
		{
			initGrabArguments();
			initGrabImageWorkingMan();
		}

		/// <summary>
		/// 初始化 Camera 參數
		/// </summary>
		private static void initGrabArguments()
		{
			var section = ConfigurationManager.GetSection("CameraSettingSection") as CameraSettingSection;
			var fpath = CameraSettingRepo.GetCameraSettingFilepath(section);
			var dict = CameraSettingRepo.GetCameraSettingDictionary(fpath);
			_fgArgs = new FrameGrabberArgs()
			{
				Name = dict["Name"],
				HorizontalResolution = Convert.ToInt32(dict["HorizontalResolution"]),
				VerticalResolution = Convert.ToInt32(dict["VerticalResolution"]),
				ImageWidth = Convert.ToInt32(dict["ImageWidth"]),
				ImageHeight = Convert.ToInt32(dict["ImageHeight"]),
				StartRow = Convert.ToInt32(dict["StartRow"]),
				StartColumn = Convert.ToInt32(dict["StartColumn"]),
				Field = dict["Field"],
				BitsPerChannel = Convert.ToInt32(dict["BitsPerChannel"]),
				ColorSpace = dict["ColorSpace"],
				Generic = dict["Generic"],
				ExternalTrigger = dict["ExternalTrigger"],
				CameraType = dict["CameraType"],
				Device = dict["Device"],
				Port = Convert.ToInt32(dict["Port"]),
				LineIn = Convert.ToInt32(dict["LineIn"])
			};
		}

		/// <summary>
		/// 初始化 GrabImageWorkingMan
		/// </summary>
		private static void initGrabImageWorkingMan()
		{
			var setting = ConfigurationManager.AppSettings["UsePylonDriver"] ?? "false";
			bool usePylonDriver;
			Boolean.TryParse(setting, out usePylonDriver);

			if (usePylonDriver)
			{
				_grabImageWorkingMan = new PylonGrabImageWorkingMan();
			}
			else
			{
				_grabImageWorkingMan = new GrabImageWorkingMan(_fgArgs);
			}
			
		}
		#endregion

	}
}
