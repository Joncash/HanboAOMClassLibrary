using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Hanbo.System.SingleInstance
{
	public class DeviceCheckRepo
	{
		public List<DeviceCheckViewModel> GetDeviceCheckList()
		{
			var deviceList = new List<DeviceCheckViewModel>();
			var fpath = ConfigurationManager.AppSettings["DeviceCheckList"];
			if (File.Exists(fpath))
			{
				var xDoc = XDocument.Load(fpath);
				deviceList = xDoc.Root.Elements("Device").Select(p => new DeviceCheckViewModel()
				{
					Name = p.Attribute("Name").Value,
					Description = p.Attribute("Description").Value,
					StaticClassName = p.Element("Check").Attribute("StaticClassName").Value,
					InvokeMethod = p.Element("Check").Attribute("InvokeMethod").Value,
					Params = p.Element("Check").Attribute("Params").Value,
				}).ToList();
			}
			return deviceList;
		}
	}
}
