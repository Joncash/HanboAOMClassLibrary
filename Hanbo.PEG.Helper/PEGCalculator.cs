using Hanbo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.PEG.Helper
{
	public static class PEGCalculator
	{
		/// <summary>
		/// 取得 PEGMoveModel
		/// </summary>
		/// <param name="objectWidth">受測物長度 (um)</param>
		/// <param name="objectHeight">受測物寬度 (um)</param>
		/// <param name="spec">Line Scan Camera 的規格 ( Resolution, Pixel Szie, Scan Rate, exposeTime)</param>
		/// <returns></returns>
		public static PEGMoveViewModel GetPEGMoveModel(int objectWidth, int objectHeight, CameraSpecViewModel spec)
		{
			PEGMoveViewModel model = null;
			var xMoveLoop = getMoveLoopCount(objectWidth, spec.HorizontalResolution, spec.HorizontalPixelSize);
			var xMovePixel = getMovePixel(objectWidth, xMoveLoop, spec.HorizontalPixelSize);

			var yMoveLoop = getMoveLoopCount(objectHeight, spec.VerticalResolution, spec.VerticalPixelSize);
			var yMovePixel = getMovePixel(objectHeight, yMoveLoop, spec.VerticalPixelSize);

			model = new PEGMoveViewModel()
			{
				XMovePixel = xMovePixel,
				xMoveLoop = xMoveLoop,
				YMovePixel = yMovePixel,
				YMoveLoop = yMoveLoop,

			};
			return model;
		}


		//========================= 計算公式 ===========================
		/*
		 * 最佳移動次數
								length - (pixels * pixel_Length)
					M =	Ceil {	-------------------------------- }
								(pixels /2 ) * pixel_Length
								
					M 必須是 2 的倍數
					
									M
					newM = Ceil { ------ } * 2.0
					 				2.0
					
		 * 最佳移動 Pixel 單位
		 
							2 ( length )
				NewPixels = -------------------------------
							NewM * pixel_length + 2 * pixel_length
			
			*/

		/// <summary>
		/// 取得最佳移動 Pixel 單位
		/// </summary>
		/// <param name="objectLength">受測物某方向的長度 ( X or Y )</param>
		/// <param name="spec"></param>
		private static int getMovePixel(int objectLength, int moveLoopCount, double pixelSize)
		{
			var b = (2 * objectLength)
					/
					(moveLoopCount * pixelSize + 2.0 * pixelSize);

			var movePixel = (int)Math.Ceiling(b);
			return movePixel;
		}

		/// <summary>
		/// 取得移動次數
		/// </summary>
		/// <param name="objectLength">受測物某方向的長度 ( X or Y )</param>
		/// <param name="spec"></param>
		/// <returns></returns>
		private static int getMoveLoopCount(int objectLength, int resolution, double pixelSize)
		{
			var a = (objectLength - (resolution * pixelSize))
							 /
							 (resolution / 2.0 * pixelSize);
			var m = Math.Ceiling(a);
			var moveLoopCount = (int)Math.Ceiling(m / 2.0) * 2;
			return moveLoopCount;
		}


	}
}
