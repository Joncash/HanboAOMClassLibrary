using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Hanbo.Configuration.Settings
{
	/// <summary>
	/// CameraSettingSection, ConfigurationSection
	/// </summary>
	public class CameraSettingSection : ConfigurationSection
	{

		[ConfigurationProperty("Camera")]
		public CameraSettingCollection Camera
		{
			get
			{
				CameraSettingCollection collection = (CameraSettingCollection)base["Camera"];
				return collection;
			}
		}
	}


	/// <summary>
	/// CameraSettingCollection, ConfigurationElementCollection
	/// </summary>
	public class CameraSettingCollection : ConfigurationElementCollection
	{
		public void Add(CameraSetting elem)
		{
			BaseAdd(elem);
		}

		//index
		public CameraSetting this[string key]
		{
			get { return (CameraSetting)BaseGet(key); }

			set
			{

				if (BaseGet(key) != null)
				{

					BaseRemove(key);

					BaseAdd(value);

				}
			}
		}

		public CameraSetting this[int key]
		{
			get { return (CameraSetting)BaseGet(key); }

			set
			{

				if (BaseGet(key) != null)
				{

					BaseRemove(key);

					BaseAdd(value);

				}
			}
		}


		protected override ConfigurationElement CreateNewElement()
		{
			return new CameraSetting();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as CameraSetting).Name;
		}
	}

	/// <summary>
	/// CameraSetting, ConfigurationElement
	/// </summary>
	public class CameraSetting : ConfigurationElement
	{
		[ConfigurationProperty("Name")]
		public string Name
		{
			get
			{
				return (string)this["Name"];
			}
			set
			{
				this["Name"] = value;
			}
		}

		[ConfigurationProperty("Text")]
		public string Text
		{
			get
			{
				return (string)this["Text"];
			}
			set
			{
				this["Text"] = value;
			}
		}

		[ConfigurationProperty("SettingFilepath")]
		public string SettingFilepath
		{
			get
			{
				return (string)this["SettingFilepath"];
			}
			set
			{
				this["SettingFilepath"] = value;
			}
		}
	}
}

