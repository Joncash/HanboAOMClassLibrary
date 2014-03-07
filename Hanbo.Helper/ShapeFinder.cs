using HalconDotNet;
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
		private HTuple _minScore = 0.5;
		private HTuple _numMatches = 9999;
		private HTuple _maxOverlap = 0.5;
		private HTuple _subpixAlgo = "least_squares";
		private HTuple _numberOfLevel = 6;
		private HTuple _greediness = 0.75;

		public ShapeFinder()
		{
			_minScore = ConfigurationHelper.GetGlobalShapeFinderMinScore();
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
					Hanbo.Log.LogManager.Trace("ShapeFinder @Find() => readShapeModel()");
					HTuple hv_A1LModelId = ReadShapeModel(trainingModelfilepath);
					shapeModel = findShapeModel(hImage, hv_A1LModelId);
				}
				catch (Exception ex)
				{
					Hanbo.Log.LogManager.Error("ShapeFinder exception:" + ex.Message);
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
				Hanbo.Log.LogManager.Error("ShapeFinder exception:" + ex.Message);
			}
			return shapeModel;
		}

		private ShapeModel findShapeModel(HObject hImage, HTuple hv_A1LModelId)
		{
			ShapeModel shapeModel = null;
			//local variables
			HTuple hv_ModelRow, hv_ModelColumn, hv_ModelAngle, hv_ModelScore;

			//find
			Hanbo.Log.LogManager.Trace("ShapeFinder @Find() => FindShapeModel()");
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
			return GetOrderdShapeModel(shapeModel);
		}
		public ShapeModel GetOrderdShapeModel(ShapeModel raw)
		{
			ShapeModel orderedModel = null;
			if (raw != null)
			{
				List<ShapeModelDataTransferObject> aList = new List<ShapeModelDataTransferObject>();
				for (int i = 0; i < raw.Score.TupleLength(); i++)
				{
					aList.Add(new ShapeModelDataTransferObject()
					{
						Row = raw.Row[i].D,
						Col = raw.Col[i].D,
						Angle = raw.Angle[i].D,
						Score = raw.Score[i].D,
					});
				}
				var orderedList = aList.OrderBy(p => p.Col).ThenBy(p => p.Row).ToList();
				orderedModel = new ShapeModel()
				{
					ModelId = raw.ModelId,
					Row = orderedList.Select(p => p.Row).ToArray(),
					Col = orderedList.Select(p => p.Col).ToArray(),
					Angle = orderedList.Select(p => p.Angle).ToArray(),
					Score = orderedList.Select(p => p.Score).ToArray(),
				};
			}
			return orderedModel;
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

				Hanbo.Log.LogManager.Error("ReadShapeModel Exception: " + ex.Message);
			}
			return hv_A1LModelId;
		}
		class ShapeModelDataTransferObject
		{
			public double Row { get; set; }
			public double Col { get; set; }
			public double Angle { get; set; }
			public double Score { get; set; }
		}
	}
}
