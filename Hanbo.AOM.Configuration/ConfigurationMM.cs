using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
//
using Hanbo.AOM.Configuration.ViewModels;

namespace Hanbo.AOM.Configuration
{
	/// <summary>
	/// 系統設定值管理員
	/// <para>**** ConfigurationHelper 內設定要搬到這裡來</para>
	/// </summary>
	public class ConfigurationMM
	{
		#region private variables
		private static List<UnitViewModel> _lengthUnits;
		private static ImageList _geoTreeviewImageList;
		private static Dictionary<string, Bitmap> _geoImageDictionary;
		private static string _systemSettingsRootName = "SystemSettings";
		private static string _systemSettingFile = @"Configuration\Settings.xml";
		private static XDocument _systemSettingsDoc;
		#endregion
		public static XDocument GetConfigurationDoc()
		{
			return _systemSettingsDoc;
		}
		public static int SubpixThreshold
		{
			get
			{
				var threshold = 110;//
				if (_systemSettingsDoc != null)
				{
					var xpath = String.Format("{0}/MeasureSettings/SubpixThreshold", _systemSettingsRootName);
					var elem = getXElementWithXPath(xpath);
					if (elem != null)
					{
						int intValue;
						if (Int32.TryParse(elem.Value, out intValue))
						{
							threshold = intValue;
						}
					}
				}
				return threshold;


			}
		}
		public static string UpperLightChannel
		{
			get
			{
				var channel = "00";
				if (_systemSettingsDoc != null)
				{
					var xpath = String.Format("{0}/LightControl/UpperLight", _systemSettingsRootName);
					var elem = getXElementWithXPath(xpath);
					if (elem != null)
					{
						channel = elem.Value;
					}
				}
				return channel;
			}
		}
		public static string BottomLightChannel
		{
			get
			{
				var channel = "01";
				if (_systemSettingsDoc != null)
				{
					var xpath = String.Format("{0}/LightControl/BottomLight", _systemSettingsRootName);
					var elem = getXElementWithXPath(xpath);
					if (elem != null)
					{
						channel = elem.Value;
					}
				}
				return channel;
			}
		}
		static ConfigurationMM()
		{
			loadConfigFile();
			init();
		}
		private static void loadConfigFile()
		{
			_systemSettingsDoc = null;
			if (File.Exists(_systemSettingFile))
			{
				try
				{
					_systemSettingsDoc = XDocument.Load(_systemSettingFile);
				}
				catch (Exception ex)
				{
					Hanbo.Log.LogManager.Error("Error, 載入系統設定檔錯誤！");
					Hanbo.Log.LogManager.Error(ex);
				}
			}
		}
		private static void init()
		{
			_lengthUnits = new List<UnitViewModel>() { 
				new UnitViewModel(){ Name = "um", Value = "um"},
				new UnitViewModel(){ Name = "mm", Value = "mm"},
				new UnitViewModel(){ Name = "mil", Value = "mil"},
				new UnitViewModel(){ Name = "inch", Value = "inch"},
			};

			//小圖示
			_geoImageDictionary = new Dictionary<string, Bitmap>() { 
				{"Point", Hanbo.Resources.Resource.draw_point},
				{"Line", Hanbo.Resources.Resource.draw_line},
				{"Circle", Hanbo.Resources.Resource.draw_circle},
				{"Distance", Hanbo.Resources.Resource.distance},
				{"PointCircle", Hanbo.Resources.Resource._3pointCircle},
				{"Angle", Hanbo.Resources.Resource.angle},
				{"SymmetryLine", Hanbo.Resources.Resource.symmetryLine},
				{"CrossPoint", Hanbo.Resources.Resource.crossPoint},
				{"DistanceX", Hanbo.Resources.Resource.distanceX},
				{"DistanceY", Hanbo.Resources.Resource.distanceY},
				{"Arc", Hanbo.Resources.Resource.draw_arc},
			};

			_geoTreeviewImageList = new ImageList();
			foreach (var item in _geoImageDictionary)
			{
				_geoTreeviewImageList.Images.Add(item.Key, item.Value);
			}
		}

		/// <summary>
		/// 長度單位
		/// </summary>
		public static List<UnitViewModel> LengthUnits
		{
			get
			{
				return _lengthUnits;
			}
		}

		public static ImageList GeoTreeViewImageList { get { return _geoTreeviewImageList; } }
		public static Dictionary<string, Bitmap> GeoImageDictionary { get { return _geoImageDictionary; } }


		/// <summary>
		/// getXElementWithXPath, Use Default SystemXDoc
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		private static XElement getXElementWithXPath(string xpath)
		{
			XElement xElem = null;
			if (_systemSettingsDoc != null)
			{
				xElem = _systemSettingsDoc.XPathSelectElement(xpath);
			}
			return xElem;
		}

		/// <summary>
		/// NG Color
		/// <para>預設為 Red</para>
		/// </summary>
		public static Color NGColor
		{
			get
			{
				var channel = Color.Red;//Default
				if (_systemSettingsDoc != null)
				{
					var xpath = String.Format("{0}/JudgeColorSettings/NGColor", _systemSettingsRootName);
					var elem = getXElementWithXPath(xpath);
					if (elem != null)
					{
						var settingColor = Color.FromName(elem.Value);
						if (settingColor.IsKnownColor)
						{
							channel = settingColor;
						}
					}
				}
				return channel;
			}
		}
		/// <summary>
		/// OKColor
		/// <para>預設為 Lime</para>
		/// </summary>
		public static Color OKColor
		{
			get
			{
				var channel = Color.Lime;//Default
				if (_systemSettingsDoc != null)
				{
					var xpath = String.Format("{0}/JudgeColorSettings/OKColor", _systemSettingsRootName);
					var elem = getXElementWithXPath(xpath);
					if (elem != null)
					{
						var settingColor = Color.FromName(elem.Value);
						if (settingColor.IsKnownColor)
						{
							channel = settingColor;
						}
					}
				}
				return channel;
			}
		}

		public static bool SaveConfiguration(Dictionary<string, string> settingDict)
		{
			var success = (_systemSettingsDoc != null);
			if (success)
			{
				try
				{
					foreach (var item in settingDict)
					{
						var xpath = _systemSettingsRootName + "/" + item.Key;
						var value = item.Value;
						var elem = _systemSettingsDoc.XPathSelectElement(xpath);
						if (elem != null)
						{
							elem.Value = value;
						}
					}
					_systemSettingsDoc.Save(_systemSettingFile);
				}
				catch (Exception ex)
				{
					success = false;
					Hanbo.Log.LogManager.Error(ex);
					MessageBox.Show(ex.Message);
				}
			}
			return success;
		}
	}
}
