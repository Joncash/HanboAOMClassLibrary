using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public interface IGrabImage
	{
		/// <summary>
		/// 單張擷取
		/// </summary>
		void OneShot();

		/// <summary>
		/// 連續擷取
		/// </summary>
		void ContinuouslyGrab();

		/// <summary>
		/// 停止擷取
		/// </summary>
		void Stop();
	}
}
