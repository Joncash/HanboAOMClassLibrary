using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.CalibrationModule.Models
{
	public class QualityIssue
	{
		/// <summary>
		/// Constant starting with QUALITY_ISSUE_*, defined in the class
		/// CalibrationAssistant.
		/// </summary>
		private int qIssue;
		/// <summary>
		/// Score obtained from the quality assessment
		/// </summary>
		private double qScore;

		/// <summary>Initialize an instance</summary>
		/// <param name="IType">
		/// Constant starting with QUALITY_ISSUE_*, defined in
		/// the class CalibrationAssistant.
		/// </param>
		/// <param name="scr">
		/// Score achieved for this quality measurement.
		/// </param>
		public QualityIssue(int IType, double scr)
		{
			qIssue = IType;
			qScore = scr;
		}

		// getter-setter methods
		public double getScore()
		{
			return qScore;
		}

		public int getIssueType()
		{
			return qIssue;
		}

	}//end of class
}
