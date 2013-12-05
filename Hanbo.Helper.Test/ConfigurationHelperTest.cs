
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Hanbo.Helper.Test
{
	public class ConfigurationHelperTest
	{
		[Theory]
		[InlineData("PlanLight")]
		public void TestGetLightSettings(string settingName)
		{
			//assign & act
			var model = ConfigurationHelper.GetLightSettings(settingName);

			//assert
			Assert.True(model.LightDTOs.Count > 0);
		}
	}
}
