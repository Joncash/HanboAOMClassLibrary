using Hanbo.Helper;
using Hanbo.Models;
using NLog;
using PD3_Ethernet_LightControl;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.SDMS.Model
{
	public class SDMSRepo
	{
		//
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

		//
		private static SDMSDataContext _dc = new SDMSDataContext();


		/// <summary>
		/// 巨集編程儲存新檔
		/// </summary>
		/// <param name="macroName"></param>
		/// <param name="macroGuid"></param>
		/// <param name="shapeModleFilepath"></param>
		/// <param name="note"></param>
		/// <param name="trainingImage"></param>
		/// <param name="exportUnit"></param>
		/// <param name="matchingParamData"></param>
		/// <param name="measureBinaryData"></param>
		/// <param name="upperLight"></param>
		/// <param name="bottomLight"></param>
		/// <returns></returns>
		public static bool SaveMacroPlan(string macroName, string macroGuid, string shapeModleFilepath, string note, Binary trainingImage, string exportUnit, Binary matchingParamData, Binary measureBinaryData, Binary measureAssistantParamData, LightChannel upperLight, LightChannel bottomLight, ShapeViewModel shapeView, string loginUser, string trainingImageFilepath, double sizeX, double sizeY)
		{
			bool success = true;

			try
			{
				MacroPlan plan = _dc.MacroPlan.SingleOrDefault(p => p.MacroGuid == macroGuid);
				if (plan == null)
				{
					plan = new MacroPlan()
					{
						MacroName = macroName,
						MacroGuid = macroGuid,
						ShapeModelFilepath = shapeModleFilepath,
						Note = note,
						TrainingImage = trainingImage,
						TrainingImageFilepath = trainingImageFilepath,
						ExportUnit = exportUnit,
						MatchingParamBinaryData = matchingParamData,
						MeasureBinaryData = measureBinaryData,
						UpperLightSwitch = upperLight.OnOff == LightControl.LightSwitch.On,
						UpperLightValue = upperLight.Intensity.ToString("d3"),
						BottomLightSiwtch = bottomLight.OnOff == LightControl.LightSwitch.On,
						BottomLigthValue = bottomLight.Intensity.ToString("d3"),
						CreateOn = DateTime.Now,
						ModifiedOn = DateTime.Now,
						CreateBy = loginUser,
						ModifiedBy = loginUser,
						MeasureAssistantBinaryData = measureAssistantParamData,
						ModelRow = shapeView.Row,
						ModelCol = shapeView.Col,
						ModelAngle = shapeView.Angle,
						IsDeleted = false,
						ObjectXLength = sizeX,
						ObjectYLength = sizeY,
					};
					_dc.MacroPlan.InsertOnSubmit(plan);
				}
				else
				{
					//Update
					plan.MacroName = macroName;
					plan.ShapeModelFilepath = shapeModleFilepath;
					plan.Note = note;
					plan.TrainingImage = trainingImage;
					plan.TrainingImageFilepath = trainingImageFilepath;
					plan.ExportUnit = exportUnit;
					plan.MatchingParamBinaryData = matchingParamData;
					plan.MeasureBinaryData = measureBinaryData;
					plan.UpperLightSwitch = upperLight.OnOff == LightControl.LightSwitch.On;
					plan.UpperLightValue = upperLight.Intensity.ToString("d3");
					plan.BottomLightSiwtch = bottomLight.OnOff == LightControl.LightSwitch.On;
					plan.BottomLigthValue = bottomLight.Intensity.ToString("d3");
					plan.ModifiedOn = DateTime.Now;
					plan.ModifiedBy = loginUser;
					plan.MeasureAssistantBinaryData = measureAssistantParamData;
					plan.ModelRow = shapeView.Row;
					plan.ModelCol = shapeView.Col;
					plan.ModelAngle = shapeView.Angle;
					plan.IsDeleted = false;
					plan.ObjectXLength = sizeX;
					plan.ObjectYLength = sizeY;
				}
				_dc.SubmitChanges();
			}
			catch (Exception ex)
			{
				success = false;
				logger.Error(ex.Message);
			}
			return success;
		}

		public static bool SaveMacroPlan(string macroName, string macroGuid, string shapeModleFilepath, string note, Binary trainingImage, string exportUnit, Binary matchingParamData, Binary measureBinaryData, Binary measureAssistantParamData, LightChannel upperLight, LightChannel bottomLight, ShapeViewModel shapeView, string loginUser, string trainingImageFilepath)
		{
			bool success = true;

			try
			{
				MacroPlan plan = _dc.MacroPlan.SingleOrDefault(p => p.MacroGuid == macroGuid);
				if (plan == null)
				{
					plan = new MacroPlan()
					{
						MacroName = macroName,
						MacroGuid = macroGuid,
						ShapeModelFilepath = shapeModleFilepath,
						Note = note,
						TrainingImage = trainingImage,
						TrainingImageFilepath = trainingImageFilepath,
						ExportUnit = exportUnit,
						MatchingParamBinaryData = matchingParamData,
						MeasureBinaryData = measureBinaryData,
						UpperLightSwitch = upperLight.OnOff == LightControl.LightSwitch.On,
						UpperLightValue = upperLight.Intensity.ToString("d3"),
						BottomLightSiwtch = bottomLight.OnOff == LightControl.LightSwitch.On,
						BottomLigthValue = bottomLight.Intensity.ToString("d3"),
						CreateOn = DateTime.Now,
						ModifiedOn = DateTime.Now,
						CreateBy = loginUser,
						ModifiedBy = loginUser,
						MeasureAssistantBinaryData = measureAssistantParamData,
						ModelRow = shapeView.Row,
						ModelCol = shapeView.Col,
						ModelAngle = shapeView.Angle,
						IsDeleted = false,
					};
					_dc.MacroPlan.InsertOnSubmit(plan);
				}
				else
				{
					//Update
					plan.MacroName = macroName;
					plan.ShapeModelFilepath = shapeModleFilepath;
					plan.Note = note;
					plan.TrainingImage = trainingImage;
					plan.TrainingImageFilepath = trainingImageFilepath;
					plan.ExportUnit = exportUnit;
					plan.MatchingParamBinaryData = matchingParamData;
					plan.MeasureBinaryData = measureBinaryData;
					plan.UpperLightSwitch = upperLight.OnOff == LightControl.LightSwitch.On;
					plan.UpperLightValue = upperLight.Intensity.ToString("d3");
					plan.BottomLightSiwtch = bottomLight.OnOff == LightControl.LightSwitch.On;
					plan.BottomLigthValue = bottomLight.Intensity.ToString("d3");
					plan.ModifiedOn = DateTime.Now;
					plan.ModifiedBy = loginUser;
					plan.MeasureAssistantBinaryData = measureAssistantParamData;
					plan.ModelRow = shapeView.Row;
					plan.ModelCol = shapeView.Col;
					plan.ModelAngle = shapeView.Angle;
					plan.IsDeleted = false;
				}
				_dc.SubmitChanges();
			}
			catch (Exception ex)
			{
				success = false;
				logger.Error(ex.Message);
			}
			return success;
		}

		public static MacroPlan GetMacroPlan(string macroGuid)
		{
			return _dc.MacroPlan.SingleOrDefault(p => p.MacroGuid == macroGuid);
		}

		public static bool UserIDExists(string uid)
		{
			var exists = _dc.UserMember.Any(p => p.UserID == uid);
			return exists;
		}

		public static UserMember GetCurrentUser(string uid)
		{
			return _dc.UserMember.SingleOrDefault(p => p.UserID == uid);
		}

		/// <summary>
		/// 新增使用者
		/// </summary>
		/// <param name="model"></param>
		/// <param name="loginUserID"></param>
		/// <returns></returns>
		public static bool AddUserMember(UserMemberViewModel model, string loginUserID)
		{
			var success = false;
			var actionType = "AddUser";
			try
			{
				var salt = Guid.NewGuid().ToString();
				var pwd = model.PWD + salt;
				var encodePWD = HanboSecurityHelper.GetEncrypt(pwd);

				var user = new UserMember()
				{
					UserID = model.Uid,
					Name = model.UName,
					Email = model.Email,
					Password = encodePWD,
					Salt = salt,
					DeptName = model.DeptName,
					CreateOn = DateTime.Now,
					ModifiedOn = DateTime.Now,
					CreateBy = loginUserID,
					ModifiedBy = loginUserID,
					Disabled = model.AccountDisable,
				};
				int singleSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Single").SN;
				int continueSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Continue").SN;
				int macroSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "MacroPlan").SN;
				int adjustSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Adjustment").SN;
				int accountSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "UserAccount").SN;

				user.UserAuthorizedFunction.Add(new UserAuthorizedFunction() { UserID = model.Uid, FunctionSN = accountSN, Authorized = model.AuthorizedAccount });
				user.UserAuthorizedFunction.Add(new UserAuthorizedFunction() { UserID = model.Uid, FunctionSN = adjustSN, Authorized = model.AuthorizedAdjust });
				user.UserAuthorizedFunction.Add(new UserAuthorizedFunction() { UserID = model.Uid, FunctionSN = continueSN, Authorized = model.AuthorizedContinue });
				user.UserAuthorizedFunction.Add(new UserAuthorizedFunction() { UserID = model.Uid, FunctionSN = macroSN, Authorized = model.AuthorizedMacroPlan });
				user.UserAuthorizedFunction.Add(new UserAuthorizedFunction() { UserID = model.Uid, FunctionSN = singleSN, Authorized = model.AuthorizedSingle });

				_dc.UserMember.InsertOnSubmit(user);
				_dc.SubmitChanges();
				success = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
			}
			finally
			{
				_dc.ActionLog.InsertOnSubmit(new ActionLog()
				{
					ActionType = actionType,
					ActionData = "UserID:" + model.Uid,
					CreateOn = DateTime.Now,
					UserID = loginUserID,
					Note = success ? "Success" : "Fail",
				});
				_dc.SubmitChanges();
			}
			return success;
		}


		/// <summary>
		/// 取得 UserMemberViewModel List
		/// </summary>
		/// <returns></returns>
		public static System.ComponentModel.BindingList<UserMemberViewModel> GetUserMemberViewModelBindingList()
		{
			var data = _dc.UserMember.Select(p => new UserMemberViewModel()
			{
				Uid = p.UserID,
				UName = p.Name,
				DeptName = p.DeptName,
				AccountDisable = p.Disabled.Value,
				Email = p.Email,
				AuthorizedAccount = p.UserAuthorizedFunction.Any(q => q.Code_Function.Code == "UserAccount" && q.Authorized),
				AuthorizedSingle = p.UserAuthorizedFunction.Any(q => q.Code_Function.Code == "Single" && q.Authorized),
				AuthorizedAdjust = p.UserAuthorizedFunction.Any(q => q.Code_Function.Code == "Adjustment" && q.Authorized),
				AuthorizedContinue = p.UserAuthorizedFunction.Any(q => q.Code_Function.Code == "Continue" && q.Authorized),
				AuthorizedMacroPlan = p.UserAuthorizedFunction.Any(q => q.Code_Function.Code == "MacroPlan" && q.Authorized),
			}).ToList();
			return new System.ComponentModel.BindingList<UserMemberViewModel>(data);
		}

		/// <summary>
		/// 重置使用者密碼
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="pwd"></param>
		/// <param name="loginUserID"></param>
		/// <returns></returns>
		public static bool ResetUserPassword(string uid, string pwd, string loginUserID)
		{
			var success = false;
			var user = _dc.UserMember.SingleOrDefault(p => p.UserID == uid);
			if (user != null)
			{
				var salt = Guid.NewGuid().ToString();
				var pwdsalt = pwd + salt;
				var encodePWD = HanboSecurityHelper.GetEncrypt(pwdsalt);
				user.Password = encodePWD;
				user.Salt = salt;
				_dc.SubmitChanges();
			}
			return success;
		}

		/// <summary>
		/// 更新使用者
		/// </summary>
		/// <param name="data"></param>
		/// <param name="loginUserID"></param>
		/// <returns></returns>
		public static int UpdateUserMember(List<UserMemberViewModel> data, string loginUserID)
		{
			int affectRows = 0;
			var errMsg = "";
			try
			{
				int singleSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Single").SN;
				int continueSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Continue").SN;
				int macroSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "MacroPlan").SN;
				int adjustSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "Adjustment").SN;
				int accountSN = _dc.Code_Function.SingleOrDefault(p => p.Code == "UserAccount").SN;
				foreach (var model in data)
				{
					var user = _dc.UserMember.SingleOrDefault(p => p.UserID == model.Uid);
					if (user != null)
					{

						if (user.Name != model.UName) user.Name = model.UName;
						if (user.DeptName != model.DeptName) user.DeptName = model.DeptName;
						if (user.Email != model.Email) user.Email = model.Email;
						if (user.Disabled != model.AccountDisable) user.Disabled = model.AccountDisable;

						//權限
						var authorizedData = user.UserAuthorizedFunction;

						var AuthorizedAccount = authorizedData.SingleOrDefault(q => q.Code_Function.Code == "UserAccount");
						if (AuthorizedAccount.Authorized ^ model.AuthorizedAccount) AuthorizedAccount.Authorized = model.AuthorizedAccount;

						var AuthorizedSingle = authorizedData.SingleOrDefault(q => q.Code_Function.Code == "Single");
						if (AuthorizedSingle.Authorized ^ model.AuthorizedSingle) AuthorizedSingle.Authorized = model.AuthorizedSingle;

						var AuthorizedAdjust = authorizedData.SingleOrDefault(q => q.Code_Function.Code == "Adjustment");
						if (AuthorizedAdjust.Authorized ^ model.AuthorizedAdjust) AuthorizedAdjust.Authorized = model.AuthorizedAdjust;

						var AuthorizedContinue = authorizedData.SingleOrDefault(q => q.Code_Function.Code == "Continue");
						if (AuthorizedContinue.Authorized ^ model.AuthorizedContinue) AuthorizedContinue.Authorized = model.AuthorizedContinue;

						var AuthorizedMacroPlan = authorizedData.SingleOrDefault(q => q.Code_Function.Code == "MacroPlan");
						if (AuthorizedMacroPlan.Authorized ^ model.AuthorizedMacroPlan) AuthorizedMacroPlan.Authorized = model.AuthorizedMacroPlan;
					}
				}
				affectRows = _dc.GetChangeSet().Updates.Count + _dc.GetChangeSet().Inserts.Count + _dc.GetChangeSet().Deletes.Count;
				if (affectRows > 0)
				{
					_dc.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				errMsg = ex.Message;
			}
			finally
			{
				_dc.ActionLog.InsertOnSubmit(new ActionLog()
				{
					ActionType = "Update UserMember",
					ActionData = String.Join(";", data.Select(p => p.Uid).ToArray()),
					CreateOn = DateTime.Now,
					UserID = loginUserID,
					Note = (affectRows > 0) ? "Success" : (errMsg == "") ? "Ignore" : "Fail : " + errMsg,
				});
				_dc.SubmitChanges();
			}
			return affectRows;
		}

		/// <summary>
		/// 取得連續量測巨集 ViewModel
		/// </summary>
		/// <returns></returns>
		public static List<MacroViewModel> GetMacroViewModels()
		{
			var data = _dc.MacroPlan.Where(p => p.IsDeleted != true).Select(p => new MacroViewModel
			{
				//
				Name = p.MacroName,
				ModelFilepath = p.ShapeModelFilepath,
				Note = p.Note,

				//
				CH1 = p.UpperLightValue,
				CH2 = p.BottomLigthValue,

				//
				AlgoDLLFilepath = "",
				SpecFilepath = "",
				Specs = null,

				//New
				MacroGuid = p.MacroGuid,
				TrainingModel = new ShapeViewModel() { Row = p.ModelRow, Col = p.ModelCol, Angle = p.ModelAngle },
				MeasureBinData = p.MeasureBinaryData,
				MAParamBinData = p.MeasureAssistantBinaryData,
				ExportUnit = p.ExportUnit,
			}).ToList();

			return data;
		}

		/// <summary>
		/// 匯出巨集編程
		/// </summary>
		/// <param name="plan">巨集編程</param>
		/// <returns></returns>
		public static ExportMacroPlanViewModel ExportMacroPlan(MacroPlan plan)
		{
			ExportMacroPlanViewModel model = null;
			if (plan != null)
			{
				try
				{
					model = new ExportMacroPlanViewModel()
							{
								MacroName = plan.MacroName,
								MacroGuid = plan.MacroGuid,
								Note = plan.Note,
								ShapeModelBinary = new Binary(File.ReadAllBytes(plan.ShapeModelFilepath)),
								TrainingImageBinary = new Binary(File.ReadAllBytes(plan.TrainingImageFilepath)),
								MatchingParamBinaryData = plan.MatchingParamBinaryData,
								MeasureBinaryData = plan.MeasureBinaryData,
								MeasureAssistantBinaryData = plan.MeasureAssistantBinaryData,
								ModelRow = plan.ModelRow,
								ModelCol = plan.ModelCol,
								ModelAngle = plan.ModelAngle,
								ExportUnit = plan.ExportUnit,
								UpperLightValue = plan.UpperLightValue,
								BottomLigthValue = plan.BottomLigthValue,
								UpperLightSwitch = plan.UpperLightSwitch,
								BottomLightSiwtch = plan.BottomLightSiwtch,
								CreateBy = plan.CreateBy,
								ModifiedBy = plan.ModifiedBy,
								CreateOn = plan.CreateOn,
								ModifiedOn = plan.ModifiedOn,
								IsDeleted = plan.IsDeleted,
							};
				}
				catch (Exception ex)
				{
					logger.Error(ex);
				}
			}
			return model;
		}

		/// <summary>
		/// 匯出巨集編程
		/// </summary>
		/// <param name="macroGuid">巨集編程 GUID</param>
		/// <returns></returns>
		public static ExportMacroPlanViewModel ExportMacroPlan(string macroGuid)
		{
			var plan = _dc.MacroPlan.SingleOrDefault(p => p.MacroGuid == macroGuid);
			return ExportMacroPlan(plan);
		}

		/// <summary>
		/// 匯入巨集編程
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool ImportMacroPlan(ExportMacroPlanViewModel model)
		{
			var success = false;
			try
			{
				MacroPlan plan = new MacroPlan() { };
				plan.MacroGuid = Guid.NewGuid().ToString();
				plan.MacroName = model.MacroName;
				plan.MacroGuid = model.MacroGuid;
				plan.Note = model.Note;
				plan.TrainingImage = model.TrainingImageBinary;
				plan.MatchingParamBinaryData = model.MatchingParamBinaryData;
				plan.MeasureBinaryData = model.MeasureBinaryData;
				plan.MeasureAssistantBinaryData = model.MeasureAssistantBinaryData;
				plan.ModelRow = model.ModelRow;
				plan.ModelCol = model.ModelCol;
				plan.ModelAngle = model.ModelAngle;
				plan.ExportUnit = model.ExportUnit;
				plan.UpperLightValue = model.UpperLightValue;
				plan.BottomLigthValue = model.BottomLigthValue;
				plan.UpperLightSwitch = model.UpperLightSwitch;
				plan.BottomLightSiwtch = model.BottomLightSiwtch;
				plan.CreateBy = model.CreateBy;
				plan.ModifiedBy = model.ModifiedBy;
				plan.CreateOn = model.CreateOn;
				plan.ModifiedOn = model.ModifiedOn;
				plan.IsDeleted = model.IsDeleted;

				var shapeModelFilepath = ConfigurationHelper.GetShapeModelFilePath();
				var trainingImageFilepath = ConfigurationHelper.GetTrainingImageFilepath();

				File.WriteAllBytes(shapeModelFilepath, model.ShapeModelBinary.ToArray());
				File.WriteAllBytes(trainingImageFilepath, model.TrainingImageBinary.ToArray());

				plan.ShapeModelFilepath = shapeModelFilepath;
				plan.TrainingImageFilepath = trainingImageFilepath;

				_dc.MacroPlan.InsertOnSubmit(plan);
				_dc.SubmitChanges();
				success = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return success;
		}

		/// <summary>
		/// 更新巨集編程
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool UpdateMacroPlan(ExportMacroPlanViewModel model)
		{
			bool success = false;
			try
			{
				var plan = _dc.MacroPlan.SingleOrDefault(p => p.MacroGuid == model.MacroGuid);

				var shapeModelFilepath = plan.ShapeModelFilepath;
				var trainingImageFilepath = plan.TrainingImageFilepath;

				File.WriteAllBytes(shapeModelFilepath, model.ShapeModelBinary.ToArray());
				File.WriteAllBytes(trainingImageFilepath, model.TrainingImageBinary.ToArray());

				plan.MacroName = model.MacroName;
				plan.MacroGuid = model.MacroGuid;
				plan.Note = model.Note;
				plan.TrainingImage = model.TrainingImageBinary;
				plan.MatchingParamBinaryData = model.MatchingParamBinaryData;
				plan.MeasureBinaryData = model.MeasureBinaryData;
				plan.MeasureAssistantBinaryData = model.MeasureAssistantBinaryData;
				plan.ModelRow = model.ModelRow;
				plan.ModelCol = model.ModelCol;
				plan.ModelAngle = model.ModelAngle;
				plan.ExportUnit = model.ExportUnit;
				plan.UpperLightValue = model.UpperLightValue;
				plan.BottomLigthValue = model.BottomLigthValue;
				plan.UpperLightSwitch = model.UpperLightSwitch;
				plan.BottomLightSiwtch = model.BottomLightSiwtch;
				plan.ModifiedBy = model.ModifiedBy;
				plan.ModifiedOn = DateTime.Now;
				plan.IsDeleted = model.IsDeleted;
				_dc.SubmitChanges();
				success = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return success;
		}
	}
}
