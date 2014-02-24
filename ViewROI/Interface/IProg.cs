using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI.Model;

namespace ViewROI.Interface
{
	public interface IProg
	{
		ProgGraphicModel GetProgGraphicModel();
		void SetCustomPos(double userDefineX, double userDefineY);
	}
}
