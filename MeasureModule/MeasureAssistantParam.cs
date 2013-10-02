using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureModule
{
	[Serializable]
	public class MeasureAssistantParam
	{
		//
		public double mThresh { get; set; }
		public double mSigma { get; set; }
		public double mRoiWidth { get; set; }
		public double mInitThresh { get; set; }
		public double mInitSigma { get; set; }
		public double mInitRoiWidth { get; set; }
		public string mTransition { get; set; }
		public string mPosition { get; set; }
		public string mInterpolation { get; set; }
		public int mDispEdgeLength { get; set; }
		public bool mDispROIWidth { get; set; }
		public bool mSelPair { get; set; }

		//
		public MeasureAssistantParam()
		{
			//初始化參數
			initParam();
		}

		private void initParam()
		{
			this.mThresh = 40.0;
			this.mSigma = 1.0;
			this.mRoiWidth = 10;
			this.mInterpolation = "bilinear";
			this.mSelPair = false;
			this.mTransition = "all";
			this.mPosition = "last";
			this.mDispEdgeLength = 30;
			this.mDispROIWidth = true;

			this.mInitThresh = 40.0;
			this.mInitSigma = 1.0;
			this.mInitRoiWidth = 10;
		}


	}
}
