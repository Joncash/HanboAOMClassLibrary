using Hanbo.Custom.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Models
{
	public class UserMemberViewModel
	{
		private string _Uid;
		[LocalizedDisplayNameAttribute("", "使用者帳號 ( Account ID )")]
		public string Uid
		{
			get { return _Uid; }
			set
			{
				if (value == _Uid)
					return;

				_Uid = value;
				OnPropertyChanged("Uid");
			}
		}

		private string _PWD;
		public string PWD
		{
			get { return _PWD; }
			set
			{
				if (value == _PWD)
					return;

				_PWD = value;
				OnPropertyChanged("PWD");
			}
		}

		private string _Email;
		[LocalizedDisplayNameAttribute("", "電子郵件信箱 ( Email )")]
		public string Email
		{
			get { return _Email; }
			set
			{
				if (value == _Email)
					return;

				_Email = value;
				OnPropertyChanged("Email");
			}
		}

		private string _UName;
		[LocalizedDisplayNameAttribute("", "使用者名稱 ( Account Name )")]
		public string UName
		{
			get { return _UName; }
			set
			{
				if (value == _UName)
					return;

				_UName = value;
				OnPropertyChanged("UName");
			}
		}

		private string _DeptName;
		[LocalizedDisplayNameAttribute("", "部門 ( Department Name )")]
		public string DeptName
		{
			get { return _DeptName; }
			set
			{
				if (value == _DeptName)
					return;

				_DeptName = value;
				OnPropertyChanged("DeptName");
			}
		}

		private bool _AuthorizedSingle;
		[LocalizedDisplayNameAttribute("", "手動量測 ( Manual Measurement )")]
		public bool AuthorizedSingle
		{
			get { return _AuthorizedSingle; }
			set
			{
				if (value == _AuthorizedSingle)
					return;

				_AuthorizedSingle = value;
				OnPropertyChanged("AuthorizedSingle");
			}
		}

		private bool _AuthorizedContinue;
		[LocalizedDisplayNameAttribute("", "自動化測量 ( Automation Measurement )")]
		public bool AuthorizedContinue
		{
			get { return _AuthorizedContinue; }
			set
			{
				if (value == _AuthorizedContinue)
					return;

				_AuthorizedContinue = value;
				OnPropertyChanged("AuthorizedContinue");
			}
		}

		private bool _AuthorizedMacroPlan;
		[LocalizedDisplayNameAttribute("","程式編輯 ( Automation Planning )")]
		public bool AuthorizedMacroPlan
		{
			get { return _AuthorizedMacroPlan; }
			set
			{
				if (value == _AuthorizedMacroPlan)
					return;

				_AuthorizedMacroPlan = value;
				OnPropertyChanged("AuthorizedMacroPlan");
			}
		}

		private bool _AuthorizedAdjust;
		[LocalizedDisplayNameAttribute("", " CCD 校正 ( Camera Calibration )")]
		public bool AuthorizedAdjust
		{
			get { return _AuthorizedAdjust; }
			set
			{
				if (value == _AuthorizedAdjust)
					return;

				_AuthorizedAdjust = value;
				OnPropertyChanged("AuthorizedAdjust");
			}
		}

		private bool _AuthorizedAccount;
		[LocalizedDisplayNameAttribute("", "帳號管理功能 ( Account Management )")]
		public bool AuthorizedAccount
		{
			get { return _AuthorizedAccount; }
			set
			{
				if (value == _AuthorizedAccount)
					return;

				_AuthorizedAccount = value;
				OnPropertyChanged("AuthorizedAccount");
			}
		}

		private bool _AccountDisable;
		[LocalizedDisplayNameAttribute("", "帳號停用 ( Account Disabled )")]
		public bool AccountDisable
		{
			get { return _AccountDisable; }
			set
			{
				if (value == _AccountDisable)
					return;

				_AccountDisable = value;
				OnPropertyChanged("AccountDisable");
			}
		}
		#region INotifyPropertyChanged

		/// <summary>
		/// The PropertyChanged event is used by consuming code
		/// (like WPF's binding infrastructure) to detect when
		/// a value has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raise the PropertyChanged event for the 
		/// specified property.
		/// </summary>
		/// <param name="propertyName">
		/// A string representing the name of 
		/// the property that changed.</param>
		/// <remarks>
		/// Only raise the event if the value of the property 
		/// has changed from its previous value</remarks>
		protected void OnPropertyChanged(string propertyName)
		{
			// Validate the property name in debug builds
			VerifyProperty(propertyName);

			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Verifies whether the current class provides a property with a given
		/// name. This method is only invoked in debug builds, and results in
		/// a runtime exception if the <see cref="OnPropertyChanged"/> method
		/// is being invoked with an invalid property name. This may happen if
		/// a property's name was changed but not the parameter of the property's
		/// invocation of <see cref="OnPropertyChanged"/>.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		[System.Diagnostics.Conditional("DEBUG")]
		private void VerifyProperty(string propertyName)
		{
			Type type = this.GetType();

			// Look for a *public* property with the specified name
			System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
			if (pi == null)
			{
				// There is no matching property - notify the developer
				string msg = "OnPropertyChanged was invoked with invalid " +
								"property name {0}. {0} is not a public " +
								"property of {1}.";
				msg = String.Format(msg, propertyName, type.FullName);
				System.Diagnostics.Debug.Fail(msg);
			}
		}

		#endregion

	}
}
