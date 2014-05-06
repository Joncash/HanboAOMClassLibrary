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

			init();

		}
		private static void init()
		{
			_lengthUnits = new List<UnitViewModel>() { 
				new UnitViewModel(){ Name = "um", Value = "um"},
				new UnitViewModel(){ Name = "mm", Value = "mm"},
				new UnitViewModel(){ Name = "mil", Value = "mil"},
				new UnitViewModel(){ Name = "inch", Value = "inch"},
			};
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
			};

			_geoTreeviewImageList = new ImageList();
			foreach (var item in _geoImageDictionary)
			{
				_geoTreeviewImageList.Images.Add(item.Key, item.Value);
			}
			/*
			_geoTreeviewImageList.Images.Add("Point", Hanbo.Resources.Resource.draw_point);
			_geoTreeviewImageList.Images.Add("Line", Hanbo.Resources.Resource.draw_line);
			_geoTreeviewImageList.Images.Add("Circle", Hanbo.Resources.Resource.draw_circle);
			_geoTreeviewImageList.Images.Add("Distance", Hanbo.Resources.Resource.distance);
			_geoTreeviewImageList.Images.Add("Angle", Hanbo.Resources.Resource.angle);
			_geoTreeviewImageList.Images.Add("SymmetryLine", Hanbo.Resources.Resource.symmetryLine);
			_geoTreeviewImageList.Images.Add("CrossPoint", Hanbo.Resources.Resource.crossPoint);
			_geoTreeviewImageList.Images.Add("DistanceX", Hanbo.Resources.Resource.distanceX);
			_geoTreeviewImageList.Images.Add("DistanceY", Hanbo.Resources.Resource.distanceY);
			 */
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
	}
}
