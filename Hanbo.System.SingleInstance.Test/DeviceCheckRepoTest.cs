using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using FluentAssertions;
namespace Hanbo.System.SingleInstance.Test
{
	public class DeviceCheckRepoTest
	{
		[Theory]
		[InlineData()]
		public void GetDeviceCheckListTest()
		{
			//assign
			var repo = new DeviceCheckRepo();

			//act
			var checkList = repo.GetDeviceCheckList();

			//assert
			checkList.Should().NotBeEmpty("必須有檢查裝置清單");
		}
	}
}
