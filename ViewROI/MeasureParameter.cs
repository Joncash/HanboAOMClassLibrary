using System;
using System.Collections.Generic;
using System.Text;

namespace ViewROI
{
	public class MeasureParameter
	{
		/// <summary>
		/// Sigma of gaussian smoothing.
		/// Default value: 1.0
		/// </summary>
		public double Sigma { get; set; }

		/// <summary>
		/// Minimum edge amplitude
		/// Salient edges can be selected with the parameter Threshold, which constitutes a threshold on the amplitude
		/// </summary>
		public double Threshold { get; set; }

		/// <summary>
		/// Light/dark or dark/light edge.
		/// Default value: 'all' 
		/// List of values: 'all', 'positive', 'negative' 
		/// </summary>
		public string Transition { get; set; }

		/// <summary>
		/// Selection of end points.
		///	Default value: 'all' 
		/// List of values: 'all', 'first', 'last' 
		/// </summary>
		public string Select { get; set; }

		/*
		 * 
			this.mThresh = 40.0;
			this.mSigma = 1.0;
			this.mRoiWidth = 10;
			this.mInitThresh = 40.0;
			this.mInitSigma = 1.0;
			this.mInitRoiWidth = 10;
			this.mTransition = "all";
			this.mPosition = "last";
			this.mInterpolation = "bilinear";
			this.mDispEdgeLength = 30;
			this.mDispROIWidth = true;
			this.mSelPair = false;
			
			
			
			
			this.setUnit("cm");

			
			
		 */
	}
}
