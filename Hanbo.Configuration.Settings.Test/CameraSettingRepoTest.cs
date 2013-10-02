using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit.Extensions;
using Hanbo.Extensions;
using Hanbo.Configuration.Settings;
using Xunit;

namespace Test.Hanbo.Configuration.Settings
{

	public class CameraSettingRepoTest
	{
		[Theory]
		[InlineData(@"Data\CameraSetting.xml")]
		public void TestGetCameraSettingDictionary(string settingXmlFilepath)
		{
			//assign
			var fpath = settingXmlFilepath.ToAppAbsolutePath();

			//act
			var dict = CameraSettingRepo.GetCameraSettingDictionary(fpath);

			//assert
			Assert.True(dict.Count() > 0);
		}

		[Theory]
		[InlineData(@"Data\CameraSetting.xml", "Name", "MyCamera")]
		public void TestSetCameraSetting(string settingXmlFilepath, string key, string value)
		{
			//assign
			var fpath = settingXmlFilepath.ToAppAbsolutePath();
			var dict = new Dictionary<string, string>() { 
				{key, value}
			};
			var success = false;

			//act
			CameraSettingRepo.SetCameraSetting(dict, fpath, out success);

			//assert
			var newDict = CameraSettingRepo.GetCameraSettingDictionary(fpath);
			Assert.True(newDict[key] == value);
			Assert.True(success);
		}
		/*
		 CameraSetting.xml example
<CameraSetting>
	<Name>GigEVision</Name>
	<HorizontalResolution>0</HorizontalResolution>
	<VerticalResolution>0</VerticalResolution>
	<ImageWidth>0</ImageWidth>
	<ImageHeight>0</ImageHeight>
	<StartRow>0</StartRow>
	<StartColumn>0</StartColumn>
	<Field>progressive</Field>
	<BitsPerChannel>-1</BitsPerChannel>
	<ColorSpace></ColorSpace>
	<Generic></Generic>
	<ExternalTrigger></ExternalTrigger>
	<CameraType></CameraType>
	<Device></Device>
	<Port></Port>
	<LineIn></LineIn>
</CameraSetting>
		 */
	}
}
