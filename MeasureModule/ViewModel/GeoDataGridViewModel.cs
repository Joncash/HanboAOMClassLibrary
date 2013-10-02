
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ViewROI;

namespace MeasureModule.ViewModel
{
	/// <summary>
	/// DataGridView 幾何資料模型
	/// </summary>
	[Serializable]
	public class GeoDataGridViewModel
	{
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

		public GeoDataGridViewModel()
		{
			_RecordID = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// 是否選擇
		/// </summary>		
		private bool _Selected;
		[DisplayName("選擇")]
		public bool Selected
		{
			get { return _Selected; }
			set
			{
				if (value == _Selected)
					return;

				_Selected = value;
				OnPropertyChanged("Selected");
			}
		}


		private bool _IsExportItem = true;
		[DisplayName("輸出項目")]
		public bool IsExportItem
		{
			get { return _IsExportItem; }
			set
			{
				if (value == _IsExportItem)
					return;

				_IsExportItem = value;
				OnPropertyChanged("IsExportItem");
			}
		}


		private double _StartPhi;
		public double StartPhi
		{
			get { return _StartPhi; }
			set
			{
				if (value == _StartPhi)
					return;

				_StartPhi = value;
				OnPropertyChanged("StartPhi");
			}
		}


		private double _EndPhi;
		public double EndPhi
		{
			get { return _EndPhi; }
			set
			{
				if (value == _EndPhi)
					return;

				_EndPhi = value;
				OnPropertyChanged("EndPhi");
			}
		}


		private string _PointOrder;
		public string PointOrder
		{
			get { return _PointOrder; }
			set
			{
				if (value == _PointOrder)
					return;

				_PointOrder = value;
				OnPropertyChanged("PointOrder");
			}
		}

		/// <summary>
		/// 量測的名稱
		/// </summary>
		private string _Name;
		[DisplayName("量測名稱")]
		public string Name
		{
			get { return _Name; }
			set
			{
				if (value == _Name)
					return;

				_Name = value;
				OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// 幾何模型 ID
		/// </summary>
		private string _RecordID;
		public string RecordID
		{
			get { return _RecordID; }
			set
			{
				if (value == _RecordID)
					return;

				_RecordID = value;
				OnPropertyChanged("RecordID");
			}
		}

		/// <summary>
		/// 相依的資料列們
		/// </summary>
		private string[] _DependGeoRowNames;
		public string[] DependGeoRowNames
		{
			get { return _DependGeoRowNames; }
			set
			{
				if (value == _DependGeoRowNames)
					return;

				_DependGeoRowNames = value;
				OnPropertyChanged("DependGeoRowNames");
			}
		}

		/// <summary>
		/// 相依的 ROI ID
		/// </summary>
		private string _ROIID;
		public string ROIID
		{
			get { return _ROIID; }
			set
			{
				if (value == _ROIID)
					return;

				_ROIID = value;
				OnPropertyChanged("ROIID");
			}
		}

		/// <summary>
		/// ROI 模型
		/// </summary>
		public ROIViewModel ROIModel { get; set; }

		private MeasureType _GeoType;
		public MeasureType GeoType
		{
			get { return _GeoType; }
			set
			{
				if (value == _GeoType)
					return;

				_GeoType = value;
				OnPropertyChanged("GeoType");
			}
		}

		/// <summary>
		/// 幾何元素類型 ICON
		/// </summary>
		private Image _Icon;
		[DisplayName("元素")]
		public Image Icon
		{
			get { return _Icon; }
			set
			{
				if (value == _Icon)
					return;

				_Icon = value;
				OnPropertyChanged("Icon");
			}
		}

		private double _Col1;
		[DisplayName("Start.X")]
		public double Col1
		{
			get { return _Col1; }
			set
			{
				if (value == _Col1)
					return;

				_Col1 = value;
				OnPropertyChanged("Col1");
			}
		}

		private double _Row1;
		[DisplayName("Start.Y")]
		public double Row1
		{
			get { return _Row1; }
			set
			{
				if (value == _Row1)
					return;

				_Row1 = value;
				OnPropertyChanged("Row1");
			}
		}

		private double _Col2;
		[DisplayName("End.X")]
		public double Col2
		{
			get { return _Col2; }
			set
			{
				if (value == _Col2)
					return;

				_Col2 = value;
				OnPropertyChanged("Col2");
			}
		}

		private double _Row2;
		[DisplayName("End.Y")]
		public double Row2
		{
			get { return _Row2; }
			set
			{
				if (value == _Row2)
					return;

				_Row2 = value;
				OnPropertyChanged("Row2");
			}
		}

		private double _Distance;
		[DisplayName("長度")]
		public double Distance
		{
			get { return _Distance; }
			set
			{
				if (value == _Distance)
					return;

				_Distance = value;
				OnPropertyChanged("Distance");
			}
		}

		private double _WorldDistance;
		[DisplayName("長度 (mm)")]
		public double WorldDistance
		{
			get { return _WorldDistance; }
			set
			{
				if (value == _WorldDistance)
					return;

				_WorldDistance = value;
				OnPropertyChanged("WorldDistance");
			}
		}

		private string _Normal;
		[DisplayName("標準值 (mm)")]
		public string Normal
		{
			get { return _Normal; }
			set
			{
				if (value == _Normal)
					return;

				_Normal = value;
				OnPropertyChanged("Normal");
			}
		}

		private string _UpperBound;
		[DisplayName("公差上限 (mm)")]
		public string UpperBound
		{
			get { return _UpperBound; }
			set
			{
				if (value == _UpperBound)
					return;

				_UpperBound = value;
				OnPropertyChanged("UpperBound");
			}
		}

		private string _LowerBound;
		[DisplayName("公差下限 (mm)")]
		public string LowerBound
		{
			get { return _LowerBound; }
			set
			{
				if (value == _LowerBound)
					return;

				_LowerBound = value;
				OnPropertyChanged("LowerBound");
			}
		}

		
		private string _Unit;
		/// <summary>
		/// 單位
		/// </summary>
		[DisplayName("量測單位")]
		public string Unit
		{
			get { return _Unit; }
			set
			{
				if (value == _Unit)
					return;

				_Unit = value;
				OnPropertyChanged("Unit");
			}
		}
	}
}
