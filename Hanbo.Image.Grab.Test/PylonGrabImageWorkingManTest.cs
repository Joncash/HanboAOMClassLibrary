using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;
using PylonC.NET;

namespace Hanbo.Image.Grab.Test
{
	public class PylonGrabImageWorkingManTest
	{
		[Theory]
		[InlineData("")]
		public void TestGetPylonDeviceHandle(string preserved)
		{
			//assign
			var worker = new PylonGrabImageWorkingMan();

			//act
			var deviceHandle = worker.GetPylonDeviceHandle();

			//assert
			deviceHandle.Should().NotBeNull("不可為 Null, 請確認裝置是否連接正確");
		}
		[Theory]
		[InlineData("SimpleGrab")]
		public void TestSetPylonDeviceHandleFeatures(string testCase)
		{
			//assign
			var worker = new PylonGrabImageWorkingMan();
			var features = getPylonDeviceHandleFeaturesTestCase(testCase);


			//act
			worker.DeviceOpen();
			var success = worker.SetPylonDeviceHandleFeatures(features);

			//assert
			success.Should().BeTrue("必須是 True, 代表設定成功");

			//Close
			worker.DeviceClose();
		}

		[Theory]
		[InlineData("")]
		public void TestOneShot(string preseved)
		{
			//assign
			var worker = new PylonGrabImageWorkingMan();

			//act
			worker.OneShot();
			var image = worker.GetLastestImage();

			//assert
			image.Should().NotBeNull("不可為 Null");
		}

		[Theory]
		[InlineData("")]
		public void TestContinuouslyGrab(string preserved)
		{
			//assign
			var worker = new PylonGrabImageWorkingMan();

			//act
			worker.ContinuouslyGrab();
			var image = worker.GetLastestImage();

			//assert
			image.Should().NotBeNull("不可為 Null");
		}

		#region 資料集
		private List<PylonFeature> getPylonDeviceHandleFeaturesTestCase(string testCase)
		{
			List<PylonFeature> features = new List<PylonFeature>();
			switch (testCase)
			{
				case "SimpleGrab":
					features = new List<PylonFeature>() { 
						new PylonFeature(){Name = "EnumEntry_PixelFormat_Mono8", Key = "PixelFormat", Value = "Mono8"},
						new PylonFeature(){Name = "EnumEntry_TriggerSelector_AcquisitionStart", Key = "TriggerSelector", Value = "AcquisitionStart"},
						new PylonFeature(){Name = "EnumEntry_TriggerSelector_AcquisitionStart", Key = "TriggerMode", Value = "Off"},
						new PylonFeature(){Name = "EnumEntry_TriggerSelector_FrameStart", Key = "TriggerSelector", Value = "FrameStart"},
						new PylonFeature(){Name = "EnumEntry_TriggerSelector_FrameStart", Key = "TriggerMode", Value = "Off"},			
					};
					break;
			}
			return features;
		}

		#endregion
	}
}
