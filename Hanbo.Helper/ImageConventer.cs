using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Hanbo.Helper
{
	public class ImageConventer
	{
		/// <summary>
		/// Byte Array To HImage
		/// </summary>
		/// <param name="imageData">影像資料</param>
		/// <param name="width">影像寬度</param>
		/// <param name="height">影像長度</param>
		/// <returns></returns>
		public static HImage ByteArrayToHImage(byte[] imageData, int width, int height, bool color)
		{
			var imageSize = width * height;
			var pixelFormat = (color) ? PixelFormat.Format32bppRgb : PixelFormat.Format8bppIndexed;

			Bitmap bitmap = new Bitmap(width, height, pixelFormat);
			BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			IntPtr imagePtr = bmData.Scan0;
			Marshal.Copy(imageData, 0, imagePtr, imageSize);
			bitmap.UnlockBits(bmData);

			return new HImage("byte", width, height, imagePtr);
		}

		/// <summary>
		/// Halcon Image .NET Bitmap
		/// </summary>
		/// <param name="halconImage"></param>
		/// <returns></returns>
		public static Bitmap ConvertHalconImageToBitmap(HObject halconImage, bool isColor)
		{
			if (halconImage == null)
			{
				throw new ArgumentNullException("halconImage");
			}

			HTuple pointerRed = null;
			HTuple pointerGreen = null;
			HTuple pointerBlue = null;
			HTuple type;
			HTuple width;
			HTuple height;

			// Halcon
			var pixelFormat = (isColor) ? PixelFormat.Format32bppRgb : PixelFormat.Format8bppIndexed;
			if (isColor)
				HOperatorSet.GetImagePointer3(halconImage, out pointerRed, out pointerGreen, out pointerBlue, out type, out width, out height);
			else
				HOperatorSet.GetImagePointer1(halconImage, out pointerBlue, out type, out width, out height);


			Bitmap bitmap = new Bitmap((Int32)width, (Int32)height, pixelFormat);

			BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
			byte[] rgbValues = new byte[bytes];


			IntPtr ptrB = new IntPtr(pointerBlue);
			IntPtr ptrG = IntPtr.Zero;
			IntPtr ptrR = IntPtr.Zero;
			if (pointerGreen != null) ptrG = new IntPtr(pointerGreen);
			if (pointerRed != null) ptrR = new IntPtr(pointerRed);
			int channels = (isColor) ? 3 : 1;

			// Stride
			int strideTotal = Math.Abs(bmpData.Stride);
			int unmapByes = strideTotal - ((int)width * channels);
			for (int i = 0, offset = 0; i < bytes; i += channels, offset++)
			{
				if ((offset + 1) % width == 0)
				{
					i += unmapByes;
				}

				rgbValues[i] = Marshal.ReadByte(ptrB, offset);
				if (isColor)
				{
					rgbValues[i + 1] = Marshal.ReadByte(ptrG, offset);
					rgbValues[i + 2] = Marshal.ReadByte(ptrR, offset);
				}

			}

			Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
			bitmap.UnlockBits(bmpData);
			return bitmap;
		}

		/// <summary>
		/// .NET bitmap  Halcon HImage
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static HImage ConvertBitmapToHalconImage(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}

			int width = bitmap.Width;
			int height = bitmap.Height;
			int ptrSize = width * height;

			// R/G/B
			IntPtr ptrR = Marshal.AllocHGlobal(ptrSize);
			IntPtr ptrG = Marshal.AllocHGlobal(ptrSize);
			IntPtr ptrB = Marshal.AllocHGlobal(ptrSize);

			// Bitmap
			int offset = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color c = bitmap.GetPixel(x, y);
					Marshal.WriteByte(ptrR, offset, c.R);
					Marshal.WriteByte(ptrG, offset, c.G);
					Marshal.WriteByte(ptrB, offset, c.B);
					offset++;
				}
			}

			// Himage
			HImage halconImage = new HImage();
			halconImage.GenImage3("byte", width, height, ptrR, ptrG, ptrB);

			Marshal.FreeHGlobal(ptrB);
			Marshal.FreeHGlobal(ptrG);
			Marshal.FreeHGlobal(ptrR);

			return halconImage;
		}

		/// <summary>
		/// Bitmap 轉成 byteArray
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static byte[] ConvertBitMapToByteArray(Bitmap bitmap)
		{
			byte[] result = null;
			if (bitmap != null)
			{
				MemoryStream stream = new MemoryStream();
				bitmap.Save(stream, ImageFormat.Tiff);
				result = stream.ToArray();
			}
			return result;
		}

		/// <summary>
		/// 轉換  HalconImage to Byte Array
		/// </summary>
		/// <param name="halconImage"></param>
		/// <returns></returns>
		public static byte[] ConvertHalconImageToByteArray(HObject halconImage, bool isColor)
		{
			var bitmap = ConvertHalconImageToBitmap(halconImage, isColor);
			return ConvertBitMapToByteArray(bitmap);
		}
	}
}
