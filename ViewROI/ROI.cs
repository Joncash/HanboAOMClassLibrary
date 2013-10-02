using System;
using HalconDotNet;


namespace ViewROI
{
	/// <summary>
	/// 量測類型
	/// </summary>
	public enum MeasureType
	{
		None = 0, Point = 1, Line = 2, Circle = 4, Distance = 8,
		PointCircle = 16, Angle = 32, SymmetryLine = 64,
		CrossPoint = 128, DistanceX = 256, DistanceY = 512,
	};

	/// <summary>
	/// This class is a base class containing virtual methods for handling
	/// ROIs. Therefore, an inheriting class needs to define/override these
	/// methods to provide the ROIController with the necessary information on
	/// its (= the ROIs) shape and position. The example project provides 
	/// derived ROI shapes for rectangles, lines, circles, and circular arcs.
	/// To use other shapes you must derive a new class from the base class 
	/// ROI and implement its methods.
	/// </summary>   
	public class ROI
	{
		#region Measure Parameter
		public MeasureParameter MeasureParam;
		#endregion

		/// <summary>
		/// 識別 ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 是否可見
		/// </summary>
		public bool Visiable { get; set; }

		/// <summary>
		/// 量測類型
		/// 預設為 None (未指定)
		/// </summary>
		public MeasureType ROIMeasureType = MeasureType.None;

		// class members of inheriting ROI classes
		protected int NumHandles;
		protected int activeHandleIdx;

		/// <summary>
		/// Flag to define the ROI to be 'positive' or 'negative'.
		/// </summary>
		protected int OperatorFlag;

		/// <summary>Parameter to define the line style of the ROI.</summary>
		public HTuple flagLineStyle;

		/// <summary>Constant for a positive ROI flag.</summary>
		public const int POSITIVE_FLAG = ROIController.MODE_ROI_POS;

		/// <summary>Constant for a negative ROI flag.</summary>
		public const int NEGATIVE_FLAG = ROIController.MODE_ROI_NEG;

		public const int ROI_TYPE_LINE = 10;
		public const int ROI_TYPE_CIRCLE = 11;
		public const int ROI_TYPE_CIRCLEARC = 12;
		public const int ROI_TYPE_RECTANCLE1 = 13;
		public const int ROI_TYPE_RECTANGLE2 = 14;
		public const int ROI_TYPE_POINT = 15;


		protected HTuple posOperation = new HTuple();
		protected HTuple negOperation = new HTuple(new int[] { 2, 2 });

		/// <summary>Constructor of abstract ROI class.</summary>
		public ROI()
		{
			this.Visiable = true;
			this.ID = Guid.NewGuid().ToString();
			MeasureParam = new MeasureParameter()
			{
				Sigma = 1.0,
				Threshold = 30.0,
				Transition = "all",
				Select = "all",

			};
		}

		/// <summary>Creates a new ROI instance at the mouse position.</summary>
		/// <param name="midX">
		/// x (=column) coordinate for ROI
		/// </param>
		/// <param name="midY">
		/// y (=row) coordinate for ROI
		/// </param>
		public virtual void createROI(double midX, double midY) { }

		/// <summary>Paints the ROI into the supplied window.</summary>
		/// <param name="window">HALCON window</param>
		public virtual void draw(HalconDotNet.HWindow window) { }

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		/// <returns> 
		/// Distance of the closest ROI handle.
		/// </returns>
		public virtual double distToClosestHandle(double x, double y)
		{
			return 0.0;
		}

		/// <summary> 
		/// Paints the active handle of the ROI object into the supplied window. 
		/// </summary>
		/// <param name="window">HALCON window</param>
		public virtual void displayActive(HalconDotNet.HWindow window) { }

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y).
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		public virtual void moveByHandle(double x, double y) { }

		/// <summary>Gets the HALCON region described by the ROI.</summary>
		public virtual HRegion getRegion()
		{
			return null;
		}

		public virtual double getDistanceFromStartPoint(double row, double col)
		{
			return 0.0;
		}
		/// <summary>
		/// Gets the model information described by 
		/// the ROI.
		/// </summary> 
		public virtual HTuple getModelData()
		{
			return null;
		}
		public virtual void MakeROI(ROIViewModel model)
		{
			//implemented by inherit classes
		}
		public virtual ROIViewModel ToROIViewModel()
		{
			return null;
		}

		/// <summary>Number of handles defined for the ROI.</summary>
		/// <returns>Number of handles</returns>
		public int getNumHandles()
		{
			return NumHandles;
		}

		/// <summary>Gets the active handle of the ROI.</summary>
		/// <returns>Index of the active handle (from the handle list)</returns>
		public int getActHandleIdx()
		{
			return activeHandleIdx;
		}

		/// <summary>
		/// Gets the sign of the ROI object, being either 
		/// 'positive' or 'negative'. This sign is used when creating a model
		/// region for matching applications from a list of ROIs.
		/// </summary>
		public int getOperatorFlag()
		{
			return OperatorFlag;
		}

		/// <summary>
		/// Sets the sign of a ROI object to be positive or negative. 
		/// The sign is used when creating a model region for matching
		/// applications by summing up all positive and negative ROI models
		/// created so far.
		/// </summary>
		/// <param name="flag">Sign of ROI object</param>
		public void setOperatorFlag(int flag)
		{
			OperatorFlag = flag;
			switch (OperatorFlag)
			{
				case ROI.POSITIVE_FLAG:
					flagLineStyle = posOperation;
					break;
				case ROI.NEGATIVE_FLAG:
					flagLineStyle = negOperation;
					break;
				default:
					flagLineStyle = posOperation;
					break;
			}
		}
	}//end of class
}//end of namespace
