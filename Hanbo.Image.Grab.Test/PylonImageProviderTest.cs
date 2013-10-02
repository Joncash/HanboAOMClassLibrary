using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanbo.Image.Grab.Test
{
	public class PylonImageProviderTest : IDisposable
	{
		PylonImageProvider worker;
		public PylonImageProviderTest()
		{
			worker = new PylonImageProvider();
		}

		[Theory]
		[InlineData(PylonDrivenMode.Stream)]
		public void GrabImageTest(PylonDrivenMode mode)
		{
			//assign			
			worker.Connect();

			//act
			worker.GrabImage();
			var image = worker.Image;

			//assert
			image.Should().NotBeNull("不可為 Null");
		}

		public void Dispose()
		{
			worker.Dispose();
		}
	}
}
