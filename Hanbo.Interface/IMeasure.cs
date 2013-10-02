using HalconDotNet;
using MeasureModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Interface
{
    public interface IMeasure
    {
		MeasureResult Action();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="image">影像</param>
		/// <param name="modelRow">比對模型 Row 位置</param>
		/// <param name="modelColumn">比對模型 Col 位置</param>
		/// <param name="modelAngle">比對模型 Angle</param>
		void Initialize(HImage image, HTuple modelRow, HTuple modelColumn, HTuple modelAngle);
    }
}
