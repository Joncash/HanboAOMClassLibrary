using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	public class MacroViewModel
	{
		/// <summary>
		/// 巨集名稱
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 巨集描述
		/// </summary>
		public string Note { get; set; }

		public string MacroGuid { get; set; }

		/// <summary>
		/// 量測二進位資料
		/// </summary>
		public Binary MeasureBinData { get; set; }

		/// <summary>
		/// 量測參數
		/// </summary>
		public Binary MAParamBinData { get; set; }

		/// <summary>
		/// 訓練模型 ViewModel
		/// </summary>
		public ShapeViewModel TrainingModel { get; set; }

		public string SpecFilepath { get; set; }

		/// <summary>
		/// ShapeModelFilepath
		/// </summary>
		public string ModelFilepath { get; set; }

		public List<SpecDTO> Specs { get; set; }

		public string AlgoDLLFilepath { get; set; }

		/// <summary>
		/// CH1 光源亮度 
		/// </summary>
		public string CH1 { get; set; }

		public bool CH1Switch { get; set; }

		/// <summary>
		/// CH2 光源亮度 
		/// </summary>
		public string CH2 { get; set; }
		public bool CH2Switch { get; set; }

		/// <summary>
		/// CH3 光源亮度 
		/// </summary>
		public string CH3 { get; set; }
		public string CH3Switch { get; set; }

		public string ExportUnit { get; set; }
	}
}
