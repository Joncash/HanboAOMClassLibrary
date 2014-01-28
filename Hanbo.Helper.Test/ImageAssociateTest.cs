using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Hanbo.Helper.Test
{
	public class ImageAssociateTest
	{
		[Theory]
		[InlineData(@"D:\Tmp\ghi\01.png", @"D:\Tmp\ghi\02.png", 4096.0, 8192.0)]
		[InlineData(@"D:\Tmp\ghi\c1.tif", @"D:\Tmp\ghi\03.png", 4096.0, 12288.0)]
		public void TestMergeTailImage(string sImagePath, string tImagePath, double expectedWidth, double expectedHeight)
		{
			//assign
			var sImage = new HImage(sImagePath);
			var tImage = new HImage(tImagePath);

			//act
			var mergedImage = ImageAssociate.MergeTailImage(sImage, tImage, MergeDirection.Vertical);

			//assert
			HTuple width, height;
			mergedImage.GetImageSize(out width, out height);
			Assert.Equal(expectedWidth, width.D);
			Assert.Equal(expectedHeight, height.D);

		}
	}
}
