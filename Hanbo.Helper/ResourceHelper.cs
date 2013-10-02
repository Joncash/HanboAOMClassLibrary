using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ViewROI;

namespace Hanbo.Helper
{
	public class ResourceHelper
	{

		public static ImageList GetTreeViewImageList()
		{
			var imageList = new ImageList();
			imageList.Images.Add(MeasureType.Point.ToString(), Hanbo.Resources.Resource.draw_point);
			imageList.Images.Add(MeasureType.Line.ToString(), Hanbo.Resources.Resource.draw_line);
			imageList.Images.Add(MeasureType.Circle.ToString(), Hanbo.Resources.Resource.draw_circle);
			imageList.Images.Add(MeasureType.Distance.ToString(), Hanbo.Resources.Resource.distance);
			imageList.Images.Add(MeasureType.Angle.ToString(), Hanbo.Resources.Resource.angle);
			imageList.Images.Add(MeasureType.SymmetryLine.ToString(), Hanbo.Resources.Resource.symmetryLine);
			imageList.Images.Add(MeasureType.CrossPoint.ToString(), Hanbo.Resources.Resource.crossPoint);
			imageList.Images.Add(MeasureType.DistanceX.ToString(), Hanbo.Resources.Resource.distanceX);
			imageList.Images.Add(MeasureType.DistanceY.ToString(), Hanbo.Resources.Resource.distanceY);
			return imageList;
		}

		public static Dictionary<string, Bitmap> GetGeoDataGridViewImageDictionary()
		{
			var imageDict = new Dictionary<string, Bitmap>() { 
				{MeasureType.Point.ToString(), Hanbo.Resources.Resource.draw_point},
				{MeasureType.Line.ToString(), Hanbo.Resources.Resource.draw_line},
				{MeasureType.Circle.ToString(), Hanbo.Resources.Resource.draw_circle},
				{MeasureType.Distance.ToString(), Hanbo.Resources.Resource.distance},
				{MeasureType.PointCircle.ToString(), Hanbo.Resources.Resource._3pointCircle},
				{MeasureType.Angle.ToString(), Hanbo.Resources.Resource.angle},
				{MeasureType.SymmetryLine.ToString(), Hanbo.Resources.Resource.symmetryLine},
				{MeasureType.CrossPoint.ToString(), Hanbo.Resources.Resource.crossPoint},
				{MeasureType.DistanceX.ToString(), Hanbo.Resources.Resource.distanceX},
				{MeasureType.DistanceY.ToString(), Hanbo.Resources.Resource.distanceY},
			};
			return imageDict;
		}

	}
}
