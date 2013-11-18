using HalconDotNet;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	public enum MergeDirection { Horizontal, Vertical };
	public static class ImageAssociate
	{
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 把 TailImage 與 Source Image 作合併
		/// </summary>
		/// <param name="sourceImage">原始影像</param>
		/// <param name="tailImage">要接在原始影像後的影像</param>
		/// <param name="direction">合併方向</param>
		/// <returns></returns>
		public static HImage MergeTailImage(HImage sourceImage, HImage tailImage, MergeDirection direction)
		{
			HImage resultImage;
			HObject mergedImage, images;
			HOperatorSet.GenEmptyObj(out mergedImage);
			HOperatorSet.GenEmptyObj(out images);

			try
			{
				HTuple sWidth, sHeight, tWidth, tHeight;
				sourceImage.GetImageSize(out sWidth, out sHeight);
				tailImage.GetImageSize(out tWidth, out tHeight);
				HOperatorSet.ConcatObj(sourceImage, tailImage, out images);

				//產生參數
				var param = genMergeParams(direction, sWidth, sHeight, tWidth, tHeight);

				//合併
				HOperatorSet.TileImagesOffset(images, out mergedImage
					, param.offsetRow, param.offsetCol
					, param.row1, param.col1, param.row2, param.col2
					, param.width, param.height);

				resultImage = mergedImage as HImage;
				if (resultImage == null)
				{
					var fpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Guid.NewGuid().ToString());
					HOperatorSet.WriteImage(mergedImage, "tiff", 0, fpath);
					var fpathWithExtension = fpath + ".tif";
					resultImage = new HImage(fpath);
					if (File.Exists(fpathWithExtension))
						File.Delete(fpathWithExtension);
				}
			}
			catch (HOperatorException ex)
			{
				logger.Error(ex);
				resultImage = null;
			}
			return resultImage;
		}

		/// <summary>
		/// 產生合併用的參數
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="sWidth"></param>
		/// <param name="sHeight"></param>
		/// <param name="tWidth"></param>
		/// <param name="tHeight"></param>
		/// <returns></returns>
		private static MergeParamModel genMergeParams(MergeDirection direction, double sWidth, double sHeight, double tWidth, double tHeight)
		{
			HTuple row1 = new HTuple(new int[] { -1, -1 });
			HTuple col1 = new HTuple(new int[] { -1, -1 });
			HTuple row2 = new HTuple(new int[] { -1, -1 });
			HTuple col2 = new HTuple(new int[] { -1, -1 });
			double height = sHeight, width = sWidth;
			double[] offsetR = null, offsetC = null;
			MergeParamModel model = new MergeParamModel()
			{
				row1 = row1,
				col1 = col1,
				row2 = row2,
				col2 = col2,

			};
			switch (direction)
			{
				case MergeDirection.Vertical:
					width = sWidth;
					height = sHeight + tHeight;
					offsetR = new double[] { 0, sHeight };
					offsetC = new double[] { 0, 0 };
					break;
				case MergeDirection.Horizontal:
					width = sWidth + tWidth;
					height = sHeight;
					offsetR = new double[] { 0, 0 };
					offsetC = new double[] { 0, sWidth };
					break;
			}
			model.height = new HTuple(height);
			model.width = new HTuple(width);
			model.offsetRow = new HTuple(offsetR);
			model.offsetCol = new HTuple(offsetC);

			return model;
		}
		public class MergeParamModel
		{
			public HTuple offsetRow { get; set; }
			public HTuple offsetCol { get; set; }
			public HTuple row1 { get; set; }
			public HTuple col1 { get; set; }
			public HTuple row2 { get; set; }
			public HTuple col2 { get; set; }
			public HTuple width { get; set; }
			public HTuple height { get; set; }
		}
	}
}
