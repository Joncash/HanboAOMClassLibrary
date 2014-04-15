namespace Hanbo.WindowsFormsControlLibrary
{
	partial class HalconStatusStrip
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HalconStatusStrip));
			this.StatusStripContainer = new System.Windows.Forms.StatusStrip();
			this.ConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ControlModeStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ZoomFactor = new System.Windows.Forms.ToolStripStatusLabel();
			this.ImageCoordinate = new System.Windows.Forms.ToolStripStatusLabel();
			this.GrayLevelLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.MsgStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusStripContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// StatusStripContainer
			// 
			resources.ApplyResources(this.StatusStripContainer, "StatusStripContainer");
			this.StatusStripContainer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectionStatus,
            this.ControlModeStatus,
            this.ZoomFactor,
            this.ImageCoordinate,
            this.GrayLevelLabel,
            this.MsgStatus});
			this.StatusStripContainer.Name = "StatusStripContainer";
			this.StatusStripContainer.ShowItemToolTips = true;
			// 
			// ConnectionStatus
			// 
			resources.ApplyResources(this.ConnectionStatus, "ConnectionStatus");
			this.ConnectionStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.ConnectionStatus.Name = "ConnectionStatus";
			// 
			// ControlModeStatus
			// 
			resources.ApplyResources(this.ControlModeStatus, "ControlModeStatus");
			this.ControlModeStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.ControlModeStatus.Name = "ControlModeStatus";
			// 
			// ZoomFactor
			// 
			resources.ApplyResources(this.ZoomFactor, "ZoomFactor");
			this.ZoomFactor.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.ZoomFactor.Name = "ZoomFactor";
			// 
			// CoordinateLabel
			// 
			resources.ApplyResources(this.ImageCoordinate, "CoordinateLabel");
			this.ImageCoordinate.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.ImageCoordinate.Name = "CoordinateLabel";
			// 
			// GrayGridentStripStatusLabel
			// 
			resources.ApplyResources(this.GrayLevelLabel, "GrayGridentStripStatusLabel");
			this.GrayLevelLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.GrayLevelLabel.Name = "GrayGridentStripStatusLabel";
			// 
			// MsgStatus
			// 
			resources.ApplyResources(this.MsgStatus, "MsgStatus");
			this.MsgStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.MsgStatus.IsLink = true;
			this.MsgStatus.Name = "MsgStatus";
			// 
			// HalconStatusStrip
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.StatusStripContainer);
			this.Name = "HalconStatusStrip";
			this.StatusStripContainer.ResumeLayout(false);
			this.StatusStripContainer.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip StatusStripContainer;
		private System.Windows.Forms.ToolStripStatusLabel ConnectionStatus;
		private System.Windows.Forms.ToolStripStatusLabel ControlModeStatus;
		private System.Windows.Forms.ToolStripStatusLabel ZoomFactor;
		private System.Windows.Forms.ToolStripStatusLabel ImageCoordinate;
		private System.Windows.Forms.ToolStripStatusLabel GrayLevelLabel;
		private System.Windows.Forms.ToolStripStatusLabel MsgStatus;

	}
}
