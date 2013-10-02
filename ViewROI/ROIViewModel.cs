using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI
{
	/// <summary>
	/// 記錄 ROI 型態及資料
	/// </summary>
	[Serializable]
	public class ROIViewModel
	{
		/// <summary>
		/// ROI ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// ROI 中心位置 Row 座標
		/// </summary>
		public double CenterRow { get; set; }

		/// <summary>
		/// ROI 中心位置 Col 座標
		/// </summary>
		public double CenterCol { get; set; }

		/// <summary>
		/// ROI 角度
		/// </summary>
		public double Phi { get; set; }

		/// <summary>
		/// ROI 長 => length1
		/// </summary>
		public double Length { get; set; }

		/// <summary>
		/// ROI 寬 => length2
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// ROI 半徑
		/// </summary>
		public double Radius { get; set; }

		/// <summary>
		/// ROI 類型, Recontange1, or ROI Circle
		/// </summary>
		public int ROIType { get; set; }

		/// <summary>
		/// 重建模型資料
		/// </summary>
		/// <returns></returns>
		public ROI MakeROI()
		{
			ROI roi = null;
			switch (ROIType)
			{
				case ROI.ROI_TYPE_RECTANGLE2:
					roi = new ROIRectangle2();
					(roi as ROIRectangle2).MakeROI(CenterRow, CenterCol, Phi, Length, Width);
					break;
				case ROI.ROI_TYPE_CIRCLE:
					roi = new ROICircle();
					(roi as ROICircle).MakeROI(CenterRow, CenterCol, Radius);
					break;
			}
			if (roi != null) roi.ID = this.ID;
			return roi;
		}
	}
}
