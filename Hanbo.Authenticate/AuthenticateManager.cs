using Hanbo.SDMS.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Hanbo.Authenticate
{
	public class AuthenticateManager
	{
		private static string _userID = "";
		private static DateTime _loginTime;
		private static DateTime _lastLoginTime;
		private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
		public static bool Authenticate(string userID, string password)
		{
			var actionType = "Login";
			var actionData = "userID:" + userID;
			var success = false;
			logger.Debug("Authenticate Start");
			try
			{
				//取得User 資訊
				var dc = new SDMSDataContext();
				var user = dc.UserMember.SingleOrDefault(p => p.UserID == userID && p.Disabled == false);
				if (user != null)
				{
					// salt
					var encodePassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password + user.Salt, "SHA1");
					success = encodePassword == user.Password;
					if (success)
					{
						_userID = user.UserID;
						_loginTime = DateTime.Now;
					}
				}
				//ActionLog
				var userLoginLog = dc.ActionLog.Where(p => p.UserID == userID
														&& p.ActionType == "Login"
														&& p.Note != "Fail")
														.OrderByDescending(p => p.CreateOn)
														.Take(1).SingleOrDefault();
				if (userLoginLog != null) _lastLoginTime = userLoginLog.CreateOn;

				var acLog = new ActionLog()
				{
					ActionType = actionType,
					ActionData = actionData,
					UserID = GetUserID(),
					CreateOn = DateTime.Now,
					Note = (success) ? "Success" : "Fail",
				};
				dc.ActionLog.InsertOnSubmit(acLog);
				dc.SubmitChanges();
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message);
			}
			logger.Debug("Authenticate Done");
			return success;
		}
		public static string GetUserID()
		{
			return _userID;
		}
		public static DateTime GetLoginTime()
		{
			return _loginTime;
		}
		public static DateTime GetLastLoginTime()
		{
			return _lastLoginTime;
		}



	}
}
