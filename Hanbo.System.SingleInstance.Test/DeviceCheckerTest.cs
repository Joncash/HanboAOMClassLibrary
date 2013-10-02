using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanbo.System.SingleInstance.Test
{
	public class DeviceCheckerTest
	{
		[Theory]
		[InlineData()]
		public void CCDCheckTest()
		{
			//assign

			//act
			var result = DeviceChecker.CCDCheck();

			//assert
			result.Success.Should().BeTrue("CCD 要能連線！");
		}

		[Theory]
		[InlineData()]
		public void DBCheckTest()
		{
			//assign

			//act
			var result = DeviceChecker.DBCheck();

			//assert
			result.Success.Should().BeTrue("資料庫要能連線！");
		}

		[Theory]
		[InlineData()]
		public void LightControlCheckTest()
		{
			//assign

			//act
			var result = DeviceChecker.LightControlCheck();

			//assert
			result.Success.Should().BeTrue("光源要能連線！");
		}

	}
}
