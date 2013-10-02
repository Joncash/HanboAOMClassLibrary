using LightControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace LightControlTest
{
    public class CCSReceiveDataResolverTest
    {
		[Theory]
		//[InlineData("@00O004F\r\n", "OK")]
		[InlineData("@00OF075.S04.L160\r\n", "OK")]
		[InlineData("@00N0351\r\n", "NG")]
		
		public void ResolveTest(string message, string expectedStatus)
		{
			//act
			var model = CCSReceiveDataResolver.Resolve(message);

			System.Diagnostics.Debug.WriteLine("channel :" + model.Channel);
			System.Diagnostics.Debug.WriteLine("intensity :" + model.Intensity);
			System.Diagnostics.Debug.WriteLine("lightmodel :" + model.LightMode);
			System.Diagnostics.Debug.WriteLine("onOff :" + model.OnOff);
			System.Diagnostics.Debug.WriteLine("status :" + model.Status);

			//assert
			Assert.True(model.Status == expectedStatus);
		}
		/*
,
@00N0250\r\n
		 */
	}
}
