using HalconDotNet;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	/// <summary>
	/// Matching 模型
	/// </summary>
	public class ShapeModel
	{
		public HTuple Row { get; set; }
		public HTuple Col { get; set; }
		public HTuple Angle { get; set; }
		public HTuple Score { get; set; }
		public HTuple ModelId { get; set; }
	}

	/// <summary>
	/// 找 ShapeModel
	/// </summary>
	public class ShapeFinder
	{
		//NLog
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private HTuple _minScore = 0.5;
		private HTuple _numMatches = 99;
		private HTuple _maxOverlap = 0.5;
		private HTuple _subpixAlgo = "least_squares";
		private HTuple _numberOfLevel = 6;
		private HTuple _greediness = 0.75;

		public ShapeFinder()
		{

		}
		/// <summary>
		/// 設定 Finder 參數
		/// </summary>
		/// <param name="minScore">最少分數</param>
		/// <param name="numMatches">Match 數目</param>
		/// <param name="maxOverlap"></param>
		/// <param name="subpixAlgo">採用的演算法</param>
		/// <param name="numberOfLevel"></param>
		/// <param name="greediness"></param>
		public ShapeFinder(double minScore, double numMatches, double maxOverlap, string subpixAlgo, int numberOfLevel, double greediness)
		{
			_minScore = minScore;
			_numMatches = numMatches;
			_maxOverlap = maxOverlap;
			_subpixAlgo = subpixAlgo;
			_numberOfLevel = numberOfLevel;
			_greediness = greediness;
		}

		/// <summary>
		/// 找到匹配模型
		/// </summary>
		/// <param name="hImage">影像</param>
		/// <param name="trainingModelfilepath">匹配模型數據檔案路徑</param>
		/// <returns></returns>
		public ShapeModel Find(HObject hImage, string trainingModelfilepath)
		{
			ShapeModel shapeModel = null;
			if (File.Exists(trainingModelfilepath))
			{
				try
				{
					logger.Trace("ShapeFinder @Find() => readShapeModel()");
					HTuple hv_A1LModelId = ReadShapeModel(trainingModelfilepath);
					shapeModel = findShapeModel(hImage, hv_A1LModelId);
				}
				catch (Exception ex)
				{
					logger.Error("ShapeFinder exception:" + ex.Message);
				}
			}
			return shapeModel;
		}

		/// <summary>
		/// 找到匹配模型
		/// </summary>
		/// <param name="hImage">影像</param>
		/// <param name="hv_A1LModelId">匹配模型 ID</param>
		/// <returns></returns>
		public ShapeModel Find(HObject hImage, HTuple hv_A1LModelId)
		{
			ShapeModel shapeModel = null;
			try
			{
				shapeModel = findShapeModel(hImage, hv_A1LModelId);
			}
			catch (Exception ex)
			{
				logger.Error("ShapeFinder exception:" + ex.Message);
			}
			return shapeModel;
		}

		private ShapeModel findShapeModel(HObject hImage, HTuple hv_A1LModelId)
		{
			ShapeModel shapeModel = null;
			//local variables
			HTuple hv_ModelRow, hv_ModelColumn, hv_ModelAngle, hv_ModelScore;

			//find
			logger.Trace("ShapeFinder @Find() => FindShapeModel()");
			HOperatorSet.FindShapeModel(hImage, hv_A1LModelId, (new HTuple(0)).TupleRad()
				, (new HTuple(360)).TupleRad(),
				_minScore,
				_numMatches,
				_maxOverlap,
				_subpixAlgo,
				_numberOfLevel,
				_greediness,
				out hv_ModelRow, out hv_ModelColumn, out hv_ModelAngle, out hv_ModelScore);

			//result model
			if (hv_ModelScore.DArr.Length > 0)
			{
				shapeModel = new ShapeModel()
				{
					ModelId = hv_A1LModelId,
					Row = hv_ModelRow,
					Col = hv_ModelColumn,
					Angle = hv_ModelAngle,
					Score = hv_ModelScore
				};
			}
			return shapeModel;
		}
		/// <summary>
		/// ReadShapeModel
		/// </summary>
		/// <param name="trainingModelFilepath"></param>
		/// <returns>ModelID</returns>
		public HTuple ReadShapeModel(string trainingModelFilepath)
		{

			HTuple hv_A1LModelId = null;
			try
			{
				HOperatorSet.ReadShapeModel(trainingModelFilepath, out hv_A1LModelId);
			}
			catch (Exception ex)
			{

				logger.Error("ReadShapeModel Exception: " + ex.Message);
			}
			return hv_A1LModelId;
		}
	}
}
