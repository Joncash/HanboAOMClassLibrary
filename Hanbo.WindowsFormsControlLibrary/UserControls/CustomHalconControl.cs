using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ViewROI;
using Hanbo.Image.Grab;
using HalconDotNet;
using Hanbo.Device.SingleInstance;
using MeasureModule;
using System.IO;
//using Hanbo.System.SingleInstance;

namespace Hanbo.WindowsFormsControlLibrary
{
	/// <summary>
	/// 自訂 Halocn Window Control With ToolStrip
	/// <para>使用時請叫用 Init </para>
	/// </summary>
	[Designer(typeof(CustomHalconControlDesigner))]
	public partial class CustomHalconControl : UserControl
	{
		private HWndCtrl mView;
		private MeasureAssistant mAssistant;
		//public ROIController roiController;
		private GrabImageWorkingMan _grabImageWorkingMan;
		private OpenFileDialog _openImageFileDialog;
		private SaveFileDialog _saveImageDialog;
		private string[] _validImageExtensions = new string[] { "bmp", "jpg", "png", "tiff" };//ConfigurationHelper.GetValidImageExtensions();
		private bool _saveImageFlag = false;
		private bool _openImageFlag = false;
		private bool _isReady = false;
		private string _edgeColor = "red";
		private int _lineWidth = 1;
		private HImage _saveImage;
		public CustomHalconControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool Init(out string message)
		{
			_isReady = true;
			message = "";
			try
			{
				var roiController = new ROIController();
				mView = new HWndCtrl(this.ViewPort);
				mView.useROIController(roiController);

				mAssistant = new MeasureAssistant(roiController);
				initFileDialog();
				initializeGrabImage();
			}
			catch (Exception ex)
			{
				_isReady = false;
				message = ex.Message;
			}
			return _isReady;
		}

		private void initFileDialog()
		{
			// var filter = ConfigurationHelper.GetOpenImageFilter();
			var filter = "Image Files(*.png;*.tiff;*.tif;*.jpg;*.jpeg)|*.png;*.tiff;*.tif;*.jpg;*.jpeg|png (*.png)|*.png|tiff (*.tiff;*.tif)|*.tiff;*.tif|jpeg (*.jpg;*.jpeg)|*.jpg;*.jpeg|all files (*.*)|*.*";
			_openImageFileDialog = new OpenFileDialog()
			{
				Filter = filter,
				RestoreDirectory = true,
			};

			//sFilter;//ConfigurationHelper.GetSaveImageFilter();
			var sFilter = "tiff (*.tiff;*.tif)|*.tiff;*.tif|jpeg (*.jpg;*.jpeg)|*.jpg;*.jpeg|png (*.png)|*.png|bmp (*.bmp)|*.bmp|all files (*.*)|*.*";
			_saveImageDialog = new SaveFileDialog()
			{
				Filter = sFilter,
				RestoreDirectory = true,
			};

			//binding event
			//SaveImageButton.Click += SaveImageButton_Click;
		}

		private void initializeGrabImage()
		{
			_grabImageWorkingMan = DeviceController.GetGrabImageWorkingManInstance();

			if (_grabImageWorkingMan != null)
			{
				//擷取
				_grabImageWorkingMan.GrabImageChanged += On_CameraGrabImageChanged;

				//Exception
				_grabImageWorkingMan.GrabImageException += On_CameraException;

				//狀態
				_grabImageWorkingMan.On_GrabImageStatusChanged += On_CameraStatusChanged;

				//停止
				_grabImageWorkingMan.GrabImageStopped += On_CameraStopped;
			}
			else
			{
				//Notify
			}
		}

		#region Camera 事件通知
		private void On_CameraStopped(object sender, RunWorkerCompletedEventArgs e)
		{
			var doRestartGrab = e.Cancelled;
			if (_openImageFlag)
			{
				openImageDialog();
			}
			if (_saveImageFlag)
			{
				openSaveImageDialog(doRestartGrab);
			}
		}

		private void On_CameraStatusChanged(object sender, GrabImageStatusChangedEventArgs e)
		{
			var cameraConnStatus = Hanbo.Resources.Resource.Disconnected;
			var status = e.Status;
			switch (status.Stage)
			{
				case GrabStage.Connected:
				case GrabStage.ContinuouslyGrabbing:
				case GrabStage.Grabbed:
				case GrabStage.Grabbing:
					cameraConnStatus = Hanbo.Resources.Resource.Connected;
					break;
			}
			//	WinStatusStrip.SetStatus(SystemStatusType.ConnectionStatus, cameraConnStatus);
		}

		private void On_CameraException(Exception ex)
		{
			var hException = ex as HOperatorException;
			if (hException != null)
			{
				var errorNumber = hException.GetErrorNumber();
				if (errorNumber == 5312)
				{
					MessageBox.Show(Hanbo.Resources.Resource.Message_CameraIsOccupied);
				}
			}
			else
			{
				//addSystemMessage(ex.Message, ex);
			}
		}

		private void On_CameraGrabImageChanged(object sender, ProgressChangedEventArgs e)
		{
			var ho_Image = e.UserState as HImage;
			try
			{
				//currImage = ho_Image;
				//mAssistant.setImage(ho_Image);
				//UpdateView();
				updateView(ho_Image);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		#endregion

		#region Apperence Public Methods============================================
		/// <summary>
		/// 設定 HalconControl 的大小
		/// </summary>
		/// <param name="w"></param>
		/// <param name="h"></param>
		public void SetViewPortSize(int w, int h)
		{
			var panelPadding = this.Panel_Center.Padding;
			var wPadding = panelPadding.Left + panelPadding.Right;
			var hPadding = panelPadding.Top + panelPadding.Bottom;

			//
			this.Width = w + wPadding;
			this.Height = this.MyToolStrip.Height + h + hPadding;
		}
		#endregion

		#region Designer===========================================================
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Panel HalconContainer { get { return this.Panel_Center; } }
		#endregion


		#region ToolStrip Butoon Click Events *****************************
		private void MoveImageModeButton_Click(object sender, EventArgs e)
		{
			setViewControlMode(sender, HWndCtrl.MODE_VIEW_MOVE);
		}
		private void ZoomImageModeButton_Click(object sender, EventArgs e)
		{
			setViewControlMode(sender, HWndCtrl.MODE_VIEW_ZOOM);
		}
		private void MagnifyImageButton_Click(object sender, EventArgs e)
		{
			setViewControlMode(sender, HWndCtrl.MODE_VIEW_ZOOMWINDOW);
		}
		private void NoneImageModeButton_Click(object sender, EventArgs e)
		{
			//影像 fit to window
			if (mView != null)
			{
				mView.resetWindow();
				updateView(null);
				//drawNonROIElement();
			}
		}
		private void CameraButton_Click(object sender, EventArgs e)
		{
			_grabImageWorkingMan.Start();
		}

		#region 開啟影像按鈕
		private void OpenFileButton_Click(object sender, EventArgs e)
		{
			_openImageFlag = true;
			if (_grabImageWorkingMan.IsBusy)
			{
				stopCameraThenOpenFileDialog();
			}
			else
			{
				openImageDialog();
			}
		}
		private void stopCameraThenOpenFileDialog()
		{
			var confirmText = Hanbo.Resources.Resource.Message_ImageAcquitionStopNotice;
			var confirmTitle = Hanbo.Resources.Resource.Message_Notice;
			var cameraStopActionConfirmed = MessageBox.Show(confirmText, confirmTitle, MessageBoxButtons.YesNo) == DialogResult.Yes;
			if (cameraStopActionConfirmed)
			{
				_grabImageWorkingMan.Cancel();
			}
			else
			{
				_openImageFlag = false;
			}
		}

		/// <summary>
		/// TestOK, But ToDo
		/// </summary>
		private void openImageDialog()
		{
			_openImageFlag = false;
			if (_openImageFileDialog.ShowDialog() == DialogResult.OK)
			{
				var fpath = _openImageFileDialog.FileName;
				try
				{
					HImage image = new HImage(fpath);
					updateView(image);

					//Notify to geoManager
					//_geoManager.Clear();
				}
				catch (HalconException ex)
				{
					if (ex.GetErrorNumber() == 5235)
					{
						MessageBox.Show(Hanbo.Resources.Resource.Message_ImageFormatErrorNotice);
					}
				}
			}
		}
		#endregion
		private void StopButton_Click(object sender, EventArgs e)
		{
			_grabImageWorkingMan.Cancel();
		}

		#region 儲存影像按鈕
		private void SaveImageButton_Click(object sender, EventArgs e)
		{
			prepareToSaveImage("instant");
		}
		private void SaveDrawToolStripButton_Click(object sender, EventArgs e)
		{
			prepareToSaveImage("draw");
		}
		private void prepareToSaveImage(string imageType)
		{
			_saveImage = (imageType == "draw") ? ViewPort.HalconWindow.DumpWindowImage() : mAssistant.getImage();
			if (_saveImage == null)
			{
				MessageBox.Show(Hanbo.Resources.Resource.Message_NoImage);
				return;
			}

			//儲存影像
			_saveImageFlag = true;
			if (_grabImageWorkingMan.IsBusy)
			{
				_grabImageWorkingMan.Cancel();
			}
			else
			{
				openSaveImageDialog(false);
			}
		}
		private void openSaveImageDialog(bool doStartCamera)
		{
			//reset flag
			_saveImageFlag = false;
			//存檔
			if (_saveImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var extension = Path.GetExtension(_saveImageDialog.FileName).Replace(".", "");
				if (!_validImageExtensions.Contains(extension)) extension = "tiff";
				HOperatorSet.WriteImage(_saveImage, extension, 0, _saveImageDialog.FileName);
			}
			if (doStartCamera) _grabImageWorkingMan.Start();
		}
		#endregion

		/// <summary>
		/// 設定 ViewController 操作模式
		/// </summary>
		/// <param name="button"></param>
		/// <param name="mode"></param>
		private void setViewControlMode(object button, int mode)
		{
			var btn = button as ToolStripButton;
			if (btn != null)
			{
				setSingleToolStripButtonOn(btn, new ToolStripButton[] { });
				var newMode = (btn.Checked) ? mode : HWndCtrl.MODE_VIEW_NONE;
				if (mView != null)
				{
					mView.setViewState(newMode);
				}
			}
		}

		/// <summary>
		/// ToolStripButton 有設定 CheckOnClick 屬性為 true 的按鈕們，當 任一按鈕 Checked 時，其他按鈕 unChecked
		/// </summary>
		/// <param name="sender"></param>
		private void setSingleToolStripButtonOn(ToolStripButton button, ToolStripButton[] excludeButtons)
		{
			if (button.Checked)
			{
				var checkOnClickItems = button.GetCurrentParent().Items.OfType<ToolStripButton>()
										.Where(p => p.CheckOnClick == true && !p.Equals(button));
				if (excludeButtons != null)
				{
					checkOnClickItems = checkOnClickItems.Where(p => !excludeButtons.Contains(p));
				}

				foreach (ToolStripButton checkOnClickToolStrip in checkOnClickItems)
				{
					checkOnClickToolStrip.Checked = false;
				}
			}
		}
		#endregion*******************************************************************

		/// <summary>
		/// 更新畫面
		/// </summary>
		/// <param name="contour"></param>
		private void updateView(HObject contour)
		{
			if (contour != null)
			{
				mView.addIconicVar(contour);
				var image = contour as HImage;
				if (image != null)
				{
					mAssistant.setImage(image);
					showMeasureResult();
					//if (roiController.getROIList().Count > 0)
					//{
					//	updateEdges();
					//	updateRegions();
					//}
				}
			}
			mView.repaint();
		}

		/// <summary>
		/// Original => updateEdges()
		/// <para>顯示測量結果</para>
		/// </summary>
		private void showMeasureResult()
		{
			HObject edges = mAssistant.getMeasureResults();
			if (edges.IsInitialized())
			{
				mView.changeGraphicSettings(GraphicsContext.GC_COLOR, _edgeColor);
				mView.changeGraphicSettings(GraphicsContext.GC_LINEWIDTH, _lineWidth);
				mView.addIconicVar(edges);
			}
		}

		#region Public Methods *******************************************************
		public HWndCtrl GetViewController()
		{
			return mView;
		}
		public HWindowControl GetViewPort()
		{
			return ViewPort;
		}
		public GrabImageWorkingMan GetCamera()
		{
			return _grabImageWorkingMan;
		}
		public void UseHalconStatus(HalconStatusStrip statusStrip)
		{
			if (statusStrip != null)
			{
				statusStrip.Init(ViewPort, mView, _grabImageWorkingMan);
			}
		}
		#endregion *************************************************************************
	}
}
