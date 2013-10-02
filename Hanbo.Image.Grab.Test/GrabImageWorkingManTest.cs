
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Xunit.Extensions;
//
using Hanbo.Extensions;
using Hanbo.Configuration.Settings;
using Xunit;
using System.IO;

namespace Hanbo.Image.Grab.Test
{
	public class GrabImageWorkingManTest
	{
		[Theory]
		[InlineData(@"D:\Repo\Hanbo.Image.Grab.Test\App_Data\CameraAsetting.xml")]
		public void Test(string fpath)
		{
			//assign
			if (!File.Exists(fpath)) return;
			var dict = CameraSettingRepo.GetCameraSettingDictionary(fpath);
			var _fgArgs = new FrameGrabberArgs()
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
			var grabber = new HalconGrabber(_fgArgs);

			//act
			var model = grabber.SnapShot2();

			//assert
			Assert.True(model.Result.State == GrabberEventState.Done);
			
		}
	}
}
