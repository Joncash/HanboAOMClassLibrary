using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Hanbo.Extensions;

namespace Hanbo.Configuration.Settings
{
	public class CameraSettingRepo
	{
		public static Dictionary<string, string> GetCameraSettingDictionary(string settingXmlFilepath)
		{
			var settingDict = new Dictionary<string, string>();
			if (File.Exists(settingXmlFilepath))
			{
				var xDoc = XDocument.Load(settingXmlFilepath);
				settingDict = xDoc.Root.Elements().ToDictionary(p => p.Name.ToString(), p => p.Value.ToString());
			}
			return settingDict;
		}

		public static void SetCameraSetting(Dictionary<string, string> settingDict, string settingXmlFilepath, out bool success)
		{
			success = false;
			if (File.Exists(settingXmlFilepath))
			{
				var xDoc = XDocument.Load(settingXmlFilepath);
				var elements = xDoc.Root.Elements();
				foreach (string key in settingDict.Keys)
				{
					var element = elements.SingleOrDefault(p => p.Name == key);
					if (element != null)
					{
						element.Value = settingDict[key];
					}
				}
				settingDict = xDoc.Root.Elements().ToDictionary(p => p.Name.ToString(), p => p.Value.ToString());
				xDoc.Save(settingXmlFilepath);
				success = true;
			}
		}

		/// <summary>
		/// 取得指定的 cameraSettingName
		/// </summary>
		/// <param name="section"></param>
		/// <param name="cameraSettingName"></param>
		/// <returns></returns>
		public static string GetCameraSettingFilepath(CameraSettingSection section, string cameraSettingName)
		{
			var setting = section.Camera.Cast<CameraSetting>().SingleOrDefault(p => p.Name == cameraSettingName);
			var fpath = (setting != null) ? setting.SettingFilepath.ToAppAbsolutePath() : "";
			return fpath;
		}

		/// <summary>
		/// 取得第一個Camera Settings
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		public static string GetCameraSettingFilepath(CameraSettingSection section)
		{
			var setting = section.Camera.Cast<CameraSetting>().FirstOrDefault();
			var fpath = (setting != null) ? setting.SettingFilepath.ToAppAbsolutePath() : "";
			return fpath;
		}

	}
}
