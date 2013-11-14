using Hanbo.ACS;
using Hanbo.Configuration.Settings;
using Hanbo.Image.Grab;
using LightControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Hanbo.System.SingleInstance
{
	public static class DeviceController
	{
		private static CameraSettingSection _section = ConfigurationManager.GetSection("CameraSettingSection") as CameraSettingSection;
		private static FrameGrabberArgs _fgArgs;
		private static GrabImageWorkingMan _grabImageWorkingMan;
		private static LineScanGrabImageWorkingMan _linescan;
		private static MotionController _motionController;
		private static bool _grabImageManInstanceUsed = false;

		private static CCSLightControlManager _lightControlManager;
		private static bool _lightControlInstanceUsed = false;

		//public static bool GrabImageManUsed
		public static bool GrabImageManUsed { get { return _grabImageManInstanceUsed; } }

		public static bool LightControlManagerUsed { get { return _lightControlInstanceUsed; } }

		public static MotionController GetMotionControllerInstance()
		{
			if (_motionController == null)
			{
				try
				{
					_motionController = new MotionController();
					_motionController.Initialize();
					_motionController.EnableAxis(Axis.X);
					_motionController.EnableAxis(Axis.Y);
					_motionController.EnableAxis(Axis.Z);
				}
				catch (Exception ex)
				{
				}
				finally
				{

				}
			}
			else
			{
				_motionController.RemoveAllRegisterEvent();
			}
			return _motionController;
			//
		}

		/// <summary>
		/// 釋放 CCD 資源
		/// </summary>
		public static void ReleaseGrabImageWorkingManInstance()
		{
			//_grabImageWorkingMan.Cancel();
			_grabImageManInstanceUsed = false;

			_linescan.Cancel();
		}
		public static LineScanGrabImageWorkingMan GetLineScanInstance()
		{
			if (_linescan == null)
			{
				_linescan = new LineScanGrabImageWorkingMan();
			}
			else
			{
				_linescan.RemoveAllRegisterEvent();
			}
			_grabImageManInstanceUsed = (_linescan != null);
			return _linescan;
		}
		public static GrabImageWorkingMan GetGrabImageWorkingManInstance()
		{
			if (_fgArgs == null)
			{
				initGrabImageHandle();
			}
			var isArgsReady = _fgArgs != null;
			if (_grabImageWorkingMan == null && isArgsReady)
			{
				_grabImageWorkingMan = new GrabImageWorkingMan(_fgArgs);
			}
			else
			{
				_grabImageWorkingMan.RemoveAllRegisterEvent();
			}
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
			var fpath = CameraSettingRepo.GetCameraSettingFilepath(_section);
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
		#endregion

	}
}
