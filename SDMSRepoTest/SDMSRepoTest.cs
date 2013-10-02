using HalconDotNet;
using Hanbo.Helper;
using Hanbo.Models;
using Hanbo.SDMS.Model;
using LightControl;
using MatchingModule;
using MeasureModule;
using MeasureModule.ViewModel;
using PD3_Ethernet_LightControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;


namespace Hanbo.SDMS.Model.Test
{
	public class SDMSRepoTest
	{
		[Theory]
		[InlineData(@"D:\SDMS_Field_Sample\NewSTD_SDMS.tif")]
		public void SaveMacroPlanTest(string imagepath)
		{
			//assign
			var shapemodelpath = "filepath";
			var note = "note";
			var exportUnit = "mm";
			var image = new HImage(imagepath);

			var imageBinData = new Binary(ImageConventer.ConvertHalconImageToByteArray(image, false));

			var matchingParam = new MatchingParam();

			var matchingParamByteArray = ModelSerializer.DoSerialize(matchingParam);

			var matchingParamBinData = new Binary(matchingParamByteArray);

			BindingList<GeoDataGridViewModel> a = new BindingList<GeoDataGridViewModel>();
			a.Add(new GeoDataGridViewModel() { RecordID = "aaa" });

			var measureBinData = ModelSerializer.DoSerialize(a);
			//BindingList a;

			var ma = new MeasureAssistant();
			var maParam = ma.GetMeasureAssistantParam();
			var maParamBin = ModelSerializer.DoSerialize(maParam);

			LightChannel upper = new LightChannel() { Channel = "00", Intensity = 100, OnOff = LightSwitch.On };

			LightChannel bottom = new LightChannel() { Channel = "01", Intensity = 200, OnOff = LightSwitch.OFF };
			//act
			var success = SDMSRepo.SaveMacroPlan("Test", Guid.NewGuid().ToString(), shapemodelpath, note, imageBinData, exportUnit, matchingParamBinData, measureBinData, maParamBin, upper, bottom, new ShapeViewModel() { }, "system", "");

			//assert
			Assert.True(success);
		}

		[Theory]
		[InlineData("24d48aae-3087-4605-b4bf-b060e304da17")]
		public void GetMacroPlanTest(string macroGuid)
		{
			//assign

			//act
			var plan = SDMSRepo.GetMacroPlan(macroGuid);

			//Assert
			Assert.True(plan != null);
		}

		[Theory]
		[InlineData(@"8cd50eba-cb7a-4178-b453-d39860fdee35")]
		public void ExportMacroPlanViewModelTest(string guid)
		{
			//assign
			var plan = SDMSRepo.GetMacroPlan(guid);

			//act
			var model = SDMSRepo.ExportMacroPlan(plan);

			//assert
			Assert.True(model != null);
		}

		[Theory]
		[InlineData(@"8cd50eba-cb7a-4178-b453-d39860fdee35")]
		public void UpdateMacroPlanTest(string guid)
		{
			//assign
			var model = SDMSRepo.ExportMacroPlan(guid);

			//act
			var success = SDMSRepo.UpdateMacroPlan(model);

			//assert
			Assert.True(success);
		}

		[Theory]
		[InlineData(@"Data\importMacroPlan.data")]
		public void ImportMacroPlanTest(string fpath)
		{
			//assign
			var model = ModelSerializer.DeSerialize(fpath) as ExportMacroPlanViewModel;

			//act
			var success = SDMSRepo.ImportMacroPlan(model);

			//assert
			Assert.True(success);
		}

	}
}
