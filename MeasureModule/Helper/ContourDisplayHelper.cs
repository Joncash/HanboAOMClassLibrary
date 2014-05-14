using HalconDotNet;
using MeasureModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeasureModule
{
	public class ContourDisplayHelper
	{
		/// <summary>
		/// 建立文字及圖形
		/// </summary>
		/// <param name="model"></param>
		/// <param name="textOnly">是否只建立文字</param>
		/// <returns></returns>
		public static ResultDisplayViewModel CreateDisplayViewModel(GeoDataGridViewModel model, bool textOnly)
		{
			return CreateDisplayViewModel(model, textOnly, 1);
		}

		public static ResultDisplayViewModel CreateDisplayViewModel(GeoDataGridViewModel model)
		{
			return CreateDisplayViewModel(model, false);
		}
		public static ResultDisplayViewModel CreateDisplayViewModel(GeoDataGridViewModel model, bool textOnly, int circleDistanceSetting)
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
						var radius = model.Distance / circleDistanceSetting;
						dispXLD.GenCircleContourXld(model.Row1, model.Col1, radius, 0.0, 6.28318, "positive", 1.0);
						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
							PositionX = posX,
							PositionY = posY,
						};
						break;

					case ViewROI.MeasureType.CrossPoint:
					case ViewROI.MeasureType.Point:
						dispXLD.GenCrossContourXld(model.Row1, model.Col1, 12, 0.785398);
						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
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
						var arrowXLD = new HXLDCont();
						//dispXLD.GenContourPolygonXld(new double[] { model.Row1, model.Row2 }, new double[] { model.Col1, model.Col2 });
						posX = (model.Col1 + model.Col2) / 2.0;
						posY = (model.Row1 + model.Row2) / 2.0;

						viewModel = new ResultDisplayViewModel()
						{
							DisplayText = dispName,
							PositionX = posX,
							PositionY = posY,
							FirstArrowX = model.Col1,
							FirstArrowY = model.Row1,
							SecArrowX = model.Col2,
							SecArrowY = model.Row2,
						};
						break;
				}
				//
				if (!textOnly && viewModel != null)
					viewModel.ImageXLD = dispXLD;

			}
			catch (Exception ex)
			{
				Hanbo.Log.LogManager.Error(ex);
			}

			return viewModel;
		}
	}
}
