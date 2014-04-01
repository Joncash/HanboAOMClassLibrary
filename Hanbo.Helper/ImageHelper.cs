using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	public class ImageHelper
	{
		/// <summary>
		/// 二進位資料轉換為 HImage
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static HImage BinaryToHImage(Binary data)
		{
			HImage img = null;
			try
			{
				var bmp = BinaryToImage(data);
				var fpath = ConfigurationHelper.GetTmpFilepath();
				bmp.Save(fpath);
				img = new HImage(fpath);
				File.Delete(fpath);
			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}
			return img;
		}

		/// <summary>
		/// 將二進位資料轉換為 Bitmap
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Bitmap BinaryToImage(Binary data)
		{
			Bitmap image = null;

			if (data != null)
			{
				try
				{
					using (MemoryStream ms = new MemoryStream(data.ToArray()))
					{
						image = new Bitmap(Image.FromStream(ms));
					}
				}
				catch (IOException ex)
				{
					Hanbo.Log.LogManager.Error(ex);
				}
			}
			return image;
		}

		/// <summary>
		/// 將二進位資料轉換為 Bitmap，指定寬高
		/// </summary>
		/// <param name="data"></param>
		/// <param name="width">長</param>
		/// <param name="height">寬</param>
		/// <returns></returns>
		public static Bitmap BinaryToImage(Binary data, int width, int height)
		{
			var image = BinaryToImage(data);
			if (image != null)
				return new Bitmap(image, new Size(width, height));
			else
				return null;
		}
	}
}
