using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI
{
	/// <summary>
	/// 圓 的 Contour
	/// </summary>
	public class ROICircleContour : ROI
	{
		private double _radius;
		private double _midR, _midC;  // first handle, 圓心的位置
		private HObject _CircleContour;

		/// <summary>
		/// ROICircleContour
		/// </summary>
		/// <param name="centerRow">圓心 y 的位置</param>
		/// <param name="centerCol">圓心 x 的位置</param>
		/// <param name="radius">半徑</param>
		public ROICircleContour(double centerRow, double centerCol, double radius)
		{
			initialize(centerRow, centerCol, radius);
		}
		public ROICircleContour(HObject circleContour, double centerRow, double centerCol, double radius)
		{
			_CircleContour = circleContour;
			initialize(centerRow, centerCol, radius);
		}
		private void initialize(double centerRow, double centerCol, double radius)
		{
			_radius = radius;
			_midR = centerRow;
			_midC = centerCol;

			NumHandles = 1; //可 Handle 移動等等的數量
			activeHandleIdx = 0; //幹啥用的??
		}

		#region 一定要 override

		/// <summary>
		/// ROI 長啥樣
		/// </summary>
		/// <param name="window"></param>		
		public override void draw(HalconDotNet.HWindow window)
		{
			//一個圓
			if (_CircleContour == null)
			{
				//window.DispCircle(_midR, _midC, _radius);
				//HObject contour = null;
				//contour.DispObj(window);
				HOperatorSet.GenCircleContourXld(out _CircleContour, _midR, _midC, _radius, 0, 4 * ((new HTuple(0)).TupleAcos()), "positive", 1);
			}
			_CircleContour.DispObj(window);

		}

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		public override double distToClosestHandle(double x, double y)
		{
			double[] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, _midR, _midC); // midpoint
			var distance = (Math.Abs(_radius - val[0]) <= 5) ? 5 : val[0];
			return distance < val[0] ? distance : val[0];
		}

		/// <summary>
		/// Gets the model information described by 
		/// the  ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { _midR, _midC, _radius });
		}
		#endregion
	}
}
