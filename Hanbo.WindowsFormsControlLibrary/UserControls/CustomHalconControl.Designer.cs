namespace Hanbo.WindowsFormsControlLibrary
{
	partial class CustomHalconControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomHalconControl));
			this.Panel_Top = new System.Windows.Forms.Panel();
			this.MyToolStrip = new System.Windows.Forms.ToolStrip();
			this.OpenFileButton = new System.Windows.Forms.ToolStripButton();
			this.CameraButton = new System.Windows.Forms.ToolStripButton();
			this.StopButton = new System.Windows.Forms.ToolStripButton();
			this.SaveImageButton = new System.Windows.Forms.ToolStripButton();
			this.SaveDrawToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.MoveImageModeButton = new System.Windows.Forms.ToolStripButton();
			this.ZoomImageModeButton = new System.Windows.Forms.ToolStripButton();
			this.MagnifyImageButton = new System.Windows.Forms.ToolStripButton();
			this.NoneImageModeButton = new System.Windows.Forms.ToolStripButton();
			this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.Panel_Center = new System.Windows.Forms.Panel();
			this.ViewPort = new HalconDotNet.HWindowControl();
			this.Panel_Top.SuspendLayout();
			this.MyToolStrip.SuspendLayout();
			this.Panel_Center.SuspendLayout();
			this.SuspendLayout();
			// 
			// Panel_Top
			// 
			this.Panel_Top.BackColor = System.Drawing.SystemColors.Control;
			this.Panel_Top.Controls.Add(this.MyToolStrip);
			this.Panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
			this.Panel_Top.Location = new System.Drawing.Point(0, 0);
			this.Panel_Top.Name = "Panel_Top";
			this.Panel_Top.Size = new System.Drawing.Size(668, 31);
			this.Panel_Top.TabIndex = 0;
			// 
			// MyToolStrip
			// 
			this.MyToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.MyToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenFileButton,
            this.CameraButton,
            this.StopButton,
            this.SaveImageButton,
            this.SaveDrawToolStripButton,
            this.toolStripSeparator1,
            this.MoveImageModeButton,
            this.ZoomImageModeButton,
            this.MagnifyImageButton,
            this.NoneImageModeButton,
            this.helpToolStripButton});
			this.MyToolStrip.Location = new System.Drawing.Point(0, 0);
			this.MyToolStrip.Name = "MyToolStrip";
			this.MyToolStrip.Size = new System.Drawing.Size(668, 31);
			this.MyToolStrip.TabIndex = 13;
			this.MyToolStrip.Text = "toolStrip1";
			// 
			// OpenFileButton
			// 
			this.OpenFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.OpenFileButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.folder_image;
			this.OpenFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.OpenFileButton.Name = "OpenFileButton";
			this.OpenFileButton.Size = new System.Drawing.Size(28, 28);
			this.OpenFileButton.Text = "開啟影像";
			this.OpenFileButton.ToolTipText = "開啟影像 ( Open Image File )";
			this.OpenFileButton.Click += new System.EventHandler(this.OpenFileButton_Click);
			// 
			// CameraButton
			// 
			this.CameraButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CameraButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.camera;
			this.CameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CameraButton.Name = "CameraButton";
			this.CameraButton.Size = new System.Drawing.Size(28, 28);
			this.CameraButton.Text = "擷取影像";
			this.CameraButton.ToolTipText = "擷取影像 (Image Acquistition )";
			this.CameraButton.Click += new System.EventHandler(this.CameraButton_Click);
			// 
			// StopButton
			// 
			this.StopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.StopButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.stop;
			this.StopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.StopButton.Name = "StopButton";
			this.StopButton.Size = new System.Drawing.Size(28, 28);
			this.StopButton.Text = "中斷連線";
			this.StopButton.ToolTipText = "中斷連線 (Disconnect Image Acquistition )";
			this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
			// 
			// SaveImageButton
			// 
			this.SaveImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SaveImageButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.save;
			this.SaveImageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SaveImageButton.Name = "SaveImageButton";
			this.SaveImageButton.Size = new System.Drawing.Size(28, 28);
			this.SaveImageButton.Text = "儲存原始影像";
			this.SaveImageButton.ToolTipText = "儲存原始影像 (Save the original image )";
			this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
			// 
			// SaveDrawToolStripButton
			// 
			this.SaveDrawToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SaveDrawToolStripButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.saveDraw;
			this.SaveDrawToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SaveDrawToolStripButton.Name = "SaveDrawToolStripButton";
			this.SaveDrawToolStripButton.Size = new System.Drawing.Size(28, 28);
			this.SaveDrawToolStripButton.Text = "儲存目前圖形";
			this.SaveDrawToolStripButton.ToolTipText = "儲存目前圖形 ( Save the current view image )";
			this.SaveDrawToolStripButton.Click += new System.EventHandler(this.SaveDrawToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
			// 
			// MoveImageModeButton
			// 
			this.MoveImageModeButton.CheckOnClick = true;
			this.MoveImageModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MoveImageModeButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.move;
			this.MoveImageModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MoveImageModeButton.Name = "MoveImageModeButton";
			this.MoveImageModeButton.Size = new System.Drawing.Size(28, 28);
			this.MoveImageModeButton.Text = "移動影像";
			this.MoveImageModeButton.ToolTipText = "移動影像 ( Move image )";
			this.MoveImageModeButton.Click += new System.EventHandler(this.MoveImageModeButton_Click);
			// 
			// ZoomImageModeButton
			// 
			this.ZoomImageModeButton.CheckOnClick = true;
			this.ZoomImageModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ZoomImageModeButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.zoom_in;
			this.ZoomImageModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ZoomImageModeButton.Name = "ZoomImageModeButton";
			this.ZoomImageModeButton.Size = new System.Drawing.Size(28, 28);
			this.ZoomImageModeButton.Text = "影像縮放";
			this.ZoomImageModeButton.ToolTipText = "影像縮放(左鍵放大，右鍵縮小) ( Zoom image (press left mouse key to zoom in, pree right mouse k" +
    "ey to zoom out)";
			this.ZoomImageModeButton.Click += new System.EventHandler(this.ZoomImageModeButton_Click);
			// 
			// MagnifyImageButton
			// 
			this.MagnifyImageButton.CheckOnClick = true;
			this.MagnifyImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MagnifyImageButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.lens;
			this.MagnifyImageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MagnifyImageButton.Name = "MagnifyImageButton";
			this.MagnifyImageButton.Size = new System.Drawing.Size(28, 28);
			this.MagnifyImageButton.Text = "放大鏡";
			this.MagnifyImageButton.ToolTipText = "放大鏡 (Magnify the part of image )";
			this.MagnifyImageButton.Click += new System.EventHandler(this.MagnifyImageButton_Click);
			// 
			// NoneImageModeButton
			// 
			this.NoneImageModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.NoneImageModeButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.zoom_fit_best3;
			this.NoneImageModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NoneImageModeButton.Name = "NoneImageModeButton";
			this.NoneImageModeButton.Size = new System.Drawing.Size(28, 28);
			this.NoneImageModeButton.Text = "影像最佳化";
			this.NoneImageModeButton.ToolTipText = "影像最佳化 ( Resize image to fit window )";
			this.NoneImageModeButton.Click += new System.EventHandler(this.NoneImageModeButton_Click);
			// 
			// helpToolStripButton
			// 
			this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
			this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.helpToolStripButton.Name = "helpToolStripButton";
			this.helpToolStripButton.Size = new System.Drawing.Size(28, 28);
			this.helpToolStripButton.Text = "He&lp";
			this.helpToolStripButton.Visible = false;
			// 
			// Panel_Center
			// 
			this.Panel_Center.BackColor = System.Drawing.Color.CornflowerBlue;
			this.Panel_Center.Controls.Add(this.ViewPort);
			this.Panel_Center.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel_Center.Location = new System.Drawing.Point(0, 31);
			this.Panel_Center.Name = "Panel_Center";
			this.Panel_Center.Padding = new System.Windows.Forms.Padding(3);
			this.Panel_Center.Size = new System.Drawing.Size(668, 623);
			this.Panel_Center.TabIndex = 1;
			// 
			// ViewPort
			// 
			this.ViewPort.BackColor = System.Drawing.Color.Black;
			this.ViewPort.BorderColor = System.Drawing.Color.Black;
			this.ViewPort.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewPort.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
			this.ViewPort.Location = new System.Drawing.Point(3, 3);
			this.ViewPort.Name = "ViewPort";
			this.ViewPort.Size = new System.Drawing.Size(662, 617);
			this.ViewPort.TabIndex = 0;
			this.ViewPort.WindowSize = new System.Drawing.Size(662, 617);
			// 
			// CustomHalconControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.Panel_Center);
			this.Controls.Add(this.Panel_Top);
			this.Name = "CustomHalconControl";
			this.Size = new System.Drawing.Size(668, 654);
			this.Panel_Top.ResumeLayout(false);
			this.Panel_Top.PerformLayout();
			this.MyToolStrip.ResumeLayout(false);
			this.MyToolStrip.PerformLayout();
			this.Panel_Center.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel Panel_Top;
		private System.Windows.Forms.Panel Panel_Center;
		private System.Windows.Forms.ToolStrip MyToolStrip;
		private System.Windows.Forms.ToolStripButton OpenFileButton;
		private System.Windows.Forms.ToolStripButton CameraButton;
		private System.Windows.Forms.ToolStripButton StopButton;
		private System.Windows.Forms.ToolStripButton SaveImageButton;
		private System.Windows.Forms.ToolStripButton SaveDrawToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton MoveImageModeButton;
		private System.Windows.Forms.ToolStripButton ZoomImageModeButton;
		private System.Windows.Forms.ToolStripButton MagnifyImageButton;
		private System.Windows.Forms.ToolStripButton NoneImageModeButton;
		private System.Windows.Forms.ToolStripButton helpToolStripButton;
		private HalconDotNet.HWindowControl ViewPort;

	}
}
