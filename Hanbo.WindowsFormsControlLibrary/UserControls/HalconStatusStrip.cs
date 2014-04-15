using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hanbo.WindowsFormsControlLibrary.Enum;
using System.Reflection;
using App.Common.Views;

namespace Hanbo.WindowsFormsControlLibrary
{
	public delegate void MessageStoreHandler(int messageCount);

	/// <summary>
	/// 狀態列 User Control
	/// <para>連線狀態, 操作模式, 縮放比例, 灰階, 系統訊息</para>
	/// </summary>
	public partial class HalconStatusStrip : UserControl
	{
		private MessageStoreHandler _messageStoreCallback;

		/// <summary>
		/// 系統訊息儲存
		/// </summary>
		private List<string> _msgStore = new List<string>();
		public HalconStatusStrip()
		{
			InitializeComponent();
		}

		#region Public Methods
		/// <summary>
		/// 設定訊息儲存
		/// </summary>
		/// <param name="msgStore"></param>
		public void SetMessageStore(List<string> msgStore, MessageStoreHandler callback)
		{
			_msgStore = msgStore;
			_messageStoreCallback = null;
			_messageStoreCallback = callback;
			this.MsgStatus.Click -= MsgStatus_Click;
			this.MsgStatus.Click += MsgStatus_Click;
		}

		/// <summary>
		/// 點選訊息狀態
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MsgStatus_Click(object sender, EventArgs e)
		{
			var msgForm = new StatusForm(_msgStore) { Text = "系統訊息 (System Messages)" };
			msgForm.ShowDialog();
			if (_messageStoreCallback != null)
			{
				_messageStoreCallback(_msgStore.Count);
			}
		}

		/// <summary>
		/// 設定狀態列數值
		/// </summary>
		/// <param name="type">狀態列 Label 類型</param>
		/// <param name="dispText">顯示文字</param>
		public void SetStatus(SystemStatusType type, string dispText)
		{
			switch (type)
			{
				case SystemStatusType.ConnectionStatus:
					this.ConnectionStatus.Text = dispText;
					break;
				case SystemStatusType.ControlMode:
					this.ControlModeStatus.Text = dispText;
					break;
				case SystemStatusType.ZoomFactor:
					this.ZoomFactor.Text = dispText;
					break;
				case SystemStatusType.Coordinate:
					this.ImageCoordinate.Text = dispText;
					break;
				case SystemStatusType.GrayLevel:
					this.GrayLevelLabel.Text = dispText;
					break;
				case SystemStatusType.SystemMsg:
					this.MsgStatus.Text = dispText;
					break;
			}
		}
		#endregion
	}
}
