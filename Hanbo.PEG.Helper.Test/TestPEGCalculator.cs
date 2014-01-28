using Hanbo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Hanbo.PEG.Helper.Test
{

	public class TestPEGCalculator
	{
		[Theory]
		[InlineData(57344, 57344, 4096, 4096, 7, 7, 4096, 2)]
		[InlineData(200000, 200000, 4096, 4096, 7, 7, 4082, 12)]
		public void TestGetPEGMoveModel(int width, int height, int hResolution, int vResolution
										, double hPixelsize, double vPixelsize
			, int expectedXMovingPixels, int expectedXMoveLoop)
		{
			//assign
			var spec = new CameraSpecViewModel()
			{
				HorizontalPixelSize = hPixelsize,
				HorizontalResolution = hResolution,
				VerticalPixelSize = vPixelsize,
				VerticalResolution = vResolution,
			};

			//act
			var model = PEGCalculator.GetPEGMoveModel(width, height, spec);

			//assert
			Assert.True(model.xMoveLoop == expectedXMoveLoop);
			Assert.True(model.XMovePixel == expectedXMovingPixels);
		}
	}
}
