using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Collections;

namespace Hanbo.CalibrationModule
{
	/// <summary>
	/// This class contains all 
	/// information about its calibration image. 
	/// Besides the basic information for the calibration process, like
	/// the plate region and the marks, the calibration results are also 
	/// stored here. 
	/// 
	/// Each CalibImage instance has a status <c>mCanCalib</c>, which 
	/// describes the mode of  "being ready for a calibration", depending 
	/// on the validity and completeness of the parameters marks, pose 
	/// and the plate region. 
	/// If these basics can not be extracted from the calibration image
	/// <c>mImage</c> using the current set of calibration parameters, 
	/// the flag <c>mCanCalib</c> remains 1 and indicates that a calibration 
	/// process is not feasible using this calibration image.
	/// </summary>
	public class CalibImage
	{
		#region private variables=====================================
		/// <summary>
		/// Reference to the controller class that performs all 
		/// calibration operations and interacts with the GUI.
		/// </summary>
		private CalibrationAssistant _mAssistant;
		/// <summary>Calibration image</summary>
		private HImage _mImage;

		/// <summary>
		/// Tuple with row coordinates of the detected marks
		/// </summary>
		private HTuple _mMarkCenterRows;
		/// <summary>
		/// Tuple with column coordinates of the detected marks
		/// </summary>
		private HTuple _mMarkCenterCols;
		/// <summary>
		/// Estimation for the external camera parameters (position and
		/// orientation)
		/// </summary>
		private HPose _mEstimatedPose;

		/// Region of the plane calibration plate in the calibration image
		/// </summary>
		private HRegion _mCaltabRegion;
		/// <summary>
		/// XLD contour points of the marks detected in 
		/// the calibration image, generated from the row and 
		/// column values <c>mMarkCenterRows</c> and 
		/// <c>mMarkCenterCols</c> 
		/// </summary>
		private HXLDCont _mMarkCenter;
		/// <summary>
		/// Estimated world coordinate system (pose of the calibration plate
		/// in camera coordinates), based on the
		/// <c>mEstimatedPose</c> and the camera parameters 
		/// for this calibration image
		/// </summary>
		private HObject _mEstimatedWCS;

		private double _mEstimatedPlateSize;

		private ArrayList _mQualityIssuesList;

		#endregion

		#region public variables =====================================
		/// <summary>
		/// Width of calibration image
		/// </summary>
		public int Width;
		/// <summary>
		/// Height of calibration image
		/// </summary>
		public int Height;

		/// <summary>
		/// HALCON error message that occurs when calculating the 
		/// basic information for the calibration image 
		/// (plate region, marks and pose).
		/// </summary>
		public string ErrorMessage;
		/// <summary>
		/// Flag that describes the degree of success or failure 
		/// after an update of the basic information.
		/// </summary>
		public string PlateStatus;
		/// <summary>
		/// Flag that permits or forbids this calibration image
		/// to be part of the calibration process
		/// </summary>
		public int CanCalibFlag; // true  = 0  ||  false = 1
		public string ImageFilepath;
		#endregion

		#region 建構子=========================================
		public CalibImage(string filename, CalibrationAssistant assist)
		{
			ImageFilepath = filename;
			_mImage = new HImage(filename);
			_mAssistant = assist;
			init();
		}

		/// <summary>
		/// Initializes all status flags and objects to set up 
		/// this image for the calibration process
		/// </summary>
		/// <param name="img">Calibration image</param>
		/// <param name="assist">Reference to the Calibration Assistant</param>
		public CalibImage(HImage img, CalibrationAssistant assist)
		{
			_mImage = img;
			_mAssistant = assist;
			init();
		}

		private void init()
		{
			//default value
			CanCalibFlag = 1;  //labeled as 'for not having been evaluated'
			PlateStatus = CalibrationAssistant.PS_NOT_FOUND;// "No Plate found" yet	
			_mEstimatedPlateSize = 0;
			ErrorMessage = "";

			//Image width and height
			string tmp;
			_mImage.GetImagePointer1(out tmp, out Width, out Height);


			// initialize all instances
			_mCaltabRegion = new HRegion();
			_mMarkCenter = new HXLDCont();
			_mEstimatedWCS = new HObject();
			_mQualityIssuesList = new ArrayList(15);

			_mMarkCenterRows = new HTuple();
			_mMarkCenterCols = new HTuple();
			_mEstimatedPose = new HPose();
		}
		#endregion

		#region public Getter Methods=========================
		public HImage GetImage()
		{
			return _mImage;
		}
		public HTuple GetMarkCenterRows()
		{
			return _mMarkCenterRows;
		}
		public HTuple GetMarkCenterColumns()
		{
			return _mMarkCenterCols;
		}
		public HXLDCont GetMarkCenters()
		{
			return _mMarkCenter;
		}
		public HTuple GetEstimatedPose()
		{
			return _mEstimatedPose;
		}
		public HObject GetEstimatedWCS()
		{
			return _mEstimatedWCS;
		}
		public double GetEstimatedPlateSize()
		{
			return _mEstimatedPlateSize;
		}
		public HObject GetCaltabRegion()
		{
			return _mCaltabRegion;
		}
		public ArrayList GetQualityIssueList()
		{
			return _mQualityIssuesList;
		}
		public string GetPlateStatus()
		{
			return PlateStatus;
		}
		#endregion

		#region Public Fucs ===============================================
		/// <summary>
		/// Determine s(or updates) the basic information for this 
		/// calibration image, which are the values for the region 
		/// plate, the center marks, and the estimated pose. 
		/// The flag <c>mPlateStatus</c> describes the evaluation 
		/// of the computation process.
		/// If desired the quality assessment can be recalculated 
		/// as well.
		/// </summary>
		/// <param name="updateQuality">
		/// Triggers the recalculation of the quality assessment for
		/// this calibration image 
		/// </param>
		public void UpdateCaltab(bool updateQuality)
		{
			HTuple worldX, worldY;
			HTuple unit = new HTuple("m");

			bool failed = false;
			QualityProcedures proc = new QualityProcedures();
			string descrFile;
			HTuple startCamp;
			ErrorMessage = "";


			_mCaltabRegion.Dispose();
			_mMarkCenter.Dispose();
			_mEstimatedWCS.Dispose();

			//reset this variable
			_mMarkCenterRows = new HTuple();

			PlateStatus = CalibrationAssistant.PS_NOT_FOUND;

			descrFile = _mAssistant.getDesrcFile();

			try
			{
				_mCaltabRegion = _mImage.FindCaltab(descrFile,
												 (int)_mAssistant.mFilterSize,
												 (int)_mAssistant.mMarkThresh,
												 (int)_mAssistant.mMinMarkDiam);

				PlateStatus = CalibrationAssistant.PS_MARKS_FAILED;

				//-- Quality issue measurements --
				if (updateQuality)
				{
					_mQualityIssuesList.Clear();
					failed = _mAssistant.testQualityIssues(this);
				}

				startCamp = _mAssistant.getCameraParams(this);
				_mMarkCenterRows = _mImage.FindMarksAndPose(_mCaltabRegion,
														  descrFile,
														  startCamp,
														  (int)_mAssistant.mInitThresh,
														  (int)_mAssistant.mThreshDecr,
														  (int)_mAssistant.mMinThresh,
														  _mAssistant.mSmoothing,
														  _mAssistant.mMinContLength,
														  _mAssistant.mMaxMarkDiam,
														  out _mMarkCenterCols,
														  out _mEstimatedPose);


				_mMarkCenter.GenCrossContourXld(_mMarkCenterRows,
											   _mMarkCenterCols,
											   new HTuple(6.0),
											   0.785398);

				if (failed)
					_mAssistant.addQualityIssue(this, CalibrationAssistant.QUALITY_ISSUE_FAILURE, 0.0);


				HOperatorSet.ImagePointsToWorldPlane(startCamp, _mEstimatedPose,
													 _mMarkCenterRows, _mMarkCenterCols,
													 unit, out worldX, out worldY);
				_mEstimatedPlateSize = HMisc.DistancePp(worldY[0].D, worldX[0].D,
													   worldY[1].D, worldX[1].D);
				_mEstimatedPlateSize *= 10.0;
				proc.get_3d_coord_system(_mImage, out _mEstimatedWCS,
										 startCamp, _mEstimatedPose,
										 new HTuple(_mEstimatedPlateSize / 2.0));

				PlateStatus = _mQualityIssuesList.Count > 0 ? CalibrationAssistant.PS_QUALITY_ISSUES : CalibrationAssistant.PS_OK; // "Quality Issues found": "OK";
				CanCalibFlag = 0;
			}
			catch (HOperatorException e)
			{
				this.ErrorMessage = e.Message;
				CanCalibFlag = 1;

				/* if exception was raised due to lack of memory, 
				 * forward the error to the calling method */
				if (e.Message.IndexOf("not enough") != -1)
					throw (e);
			}
		}

		/// <summary>
		/// Releases the memory for all iconic HALCON objects contained in
		/// this instance.
		/// </summary>
		public void Clear()
		{
			_mImage.Dispose();
			_mCaltabRegion.Dispose();
		}
		#endregion

	}//end of class

}
