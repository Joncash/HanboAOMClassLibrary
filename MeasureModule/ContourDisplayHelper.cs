using HalconDotNet;
using MeasureModule.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeasureModule
{
	public class ContourDisplayHelper
	{
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
		public static ResultDisplayViewModel CreateDisplayViewModel(GeoDataGridViewModel model)
		{
			ResultDisplayViewModel viewModel = null;

			try
			{
				//init
				var dispXLD = new HXLDCont();
				dispXLD.Dispose();
				dispXLD.GenEmptyObj();

				double posX = model.Col1, posY = model.Row1;
				string dispName = model.Name;

				switch (model.GeoType)
				{
					case ViewROI.MeasureType.Angle:
						break;

					case ViewROI.MeasureType.Circle:
					case ViewROI.MeasureType.PointCircle:
						dispXLD.GenCircleContourXld(model.Row1, model.Col1, model.Distance / 2.0, 0.0, 6.28318, "positive", 1.0);
						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
							ImageXLD = dispXLD,
							PositionX = posX,
							PositionY = posY,
						};
						break;

					case ViewROI.MeasureType.CrossPoint:
					case ViewROI.MeasureType.Point:
						dispXLD.GenCrossContourXld(model.Row1, model.Col1, 6, 0.785398);
						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
							ImageXLD = dispXLD,
							PositionX = posX,
							PositionY = posY,
						};
						break;

					case ViewROI.MeasureType.Distance:
					case ViewROI.MeasureType.DistanceX:
					case ViewROI.MeasureType.DistanceY:
					case ViewROI.MeasureType.FitLine:
					case ViewROI.MeasureType.Line:
					case ViewROI.MeasureType.SymmetryLine:
						dispXLD.GenContourPolygonXld(new double[] { model.Row1, model.Row2 }, new double[] { model.Col1, model.Col2 });
						posX = (model.Col1 + model.Col2) / 2.0;
						posY = (model.Row1 + model.Row2) / 2.0;
						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
							ImageXLD = dispXLD,
							PositionX = posX,
							PositionY = posY,
							FirstArrowX = model.Col1,
							FirstArrowY = model.Row1,
							SecArrowX = model.Col2,
							SecArrowY = model.Row2,
						};
						break;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}

			return viewModel;
		}
	}
}
