namespace PD3_Ethernet_LightControl
{
	partial class CCSLightControlForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCSLightControlForm));
			this.label17 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.L1CheckBox = new System.Windows.Forms.CheckBox();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.L2CheckBox = new System.Windows.Forms.CheckBox();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.L3CheckBox = new System.Windows.Forms.CheckBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.IPLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.PortLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.ControlPanel = new System.Windows.Forms.Panel();
			this.LightControlToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ResetButton = new System.Windows.Forms.Button();
			this.LightConnectTimer = new System.Windows.Forms.Timer(this.components);
			this.TraceConnectTimer = new System.Windows.Forms.Timer(this.components);
			this.LocationLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.ControlPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label17
			// 
			resources.ApplyResources(this.label17, "label17");
			this.label17.Name = "label17";
			// 
			// label19
			// 
			resources.ApplyResources(this.label19, "label19");
			this.label19.Name = "label19";
			// 
			// L1CheckBox
			// 
			resources.ApplyResources(this.L1CheckBox, "L1CheckBox");
			this.L1CheckBox.Name = "L1CheckBox";
			this.L1CheckBox.Tag = "00";
			this.L1CheckBox.UseVisualStyleBackColor = true;
			// 
			// numericUpDown1
			// 
			resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
			this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Tag = "00";
			// 
			// numericUpDown3
			// 
			resources.ApplyResources(this.numericUpDown3, "numericUpDown3");
			this.numericUpDown3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Tag = "02";
			// 
			// L2CheckBox
			// 
			resources.ApplyResources(this.L2CheckBox, "L2CheckBox");
			this.L2CheckBox.Name = "L2CheckBox";
			this.L2CheckBox.Tag = "01";
			this.L2CheckBox.UseVisualStyleBackColor = true;
			// 
			// numericUpDown2
			// 
			resources.ApplyResources(this.numericUpDown2, "numericUpDown2");
			this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Tag = "01";
			// 
			// L3CheckBox
			// 
			resources.ApplyResources(this.L3CheckBox, "L3CheckBox");
			this.L3CheckBox.Name = "L3CheckBox";
			this.L3CheckBox.Tag = "02";
			this.L3CheckBox.UseVisualStyleBackColor = true;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.IPLabel,
            this.PortLabel});
			resources.ApplyResources(this.statusStrip1, "statusStrip1");
			this.statusStrip1.Name = "statusStrip1";
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			resources.ApplyResources(this.StatusLabel, "StatusLabel");
			// 
			// IPLabel
			// 
			this.IPLabel.Name = "IPLabel";
			resources.ApplyResources(this.IPLabel, "IPLabel");
			// 
			// PortLabel
			// 
			this.PortLabel.Name = "PortLabel";
			resources.ApplyResources(this.PortLabel, "PortLabel");
			// 
			// ControlPanel
			// 
			this.ControlPanel.Controls.Add(this.label17);
			this.ControlPanel.Controls.Add(this.label19);
			this.ControlPanel.Controls.Add(this.L1CheckBox);
			this.ControlPanel.Controls.Add(this.numericUpDown1);
			this.ControlPanel.Controls.Add(this.numericUpDown3);
			this.ControlPanel.Controls.Add(this.L2CheckBox);
			this.ControlPanel.Controls.Add(this.numericUpDown2);
			this.ControlPanel.Controls.Add(this.L3CheckBox);
			resources.ApplyResources(this.ControlPanel, "ControlPanel");
			this.ControlPanel.Name = "ControlPanel";
			// 
			// ResetButton
			// 
			this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_Off;
			resources.ApplyResources(this.ResetButton, "ResetButton");
			this.ResetButton.Name = "ResetButton";
			this.LightControlToolTip.SetToolTip(this.ResetButton, resources.GetString("ResetButton.ToolTip"));
			this.ResetButton.UseVisualStyleBackColor = true;
			this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
			// 
			// LightConnectTimer
			// 
			this.LightConnectTimer.Interval = 1000;
			// 
			// TraceConnectTimer
			// 
			this.TraceConnectTimer.Interval = 200;
			this.TraceConnectTimer.Tick += new System.EventHandler(this.TraceConnectTimer_Tick);
			// 
			// LocationLabel
			// 
			resources.ApplyResources(this.LocationLabel, "LocationLabel");
			this.LocationLabel.Name = "LocationLabel";
			// 
			// CCSLightControlForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.LocationLabel);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.ControlPanel);
			this.Controls.Add(this.ResetButton);
			this.Name = "CCSLightControlForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CCSLightControlForm_FormClosing);
			this.Load += new System.EventHandler(this.CCSLightControlForm_Load);
			this.Shown += new System.EventHandler(this.CCSLightControlForm_Shown);
			this.LocationChanged += new System.EventHandler(this.CCSLightControlForm_LocationChanged);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ControlPanel.ResumeLayout(false);
			this.ControlPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox L1CheckBox;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDown3;
		private System.Windows.Forms.CheckBox L2CheckBox;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.CheckBox L3CheckBox;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel IPLabel;
		private System.Windows.Forms.ToolStripStatusLabel PortLabel;
		private System.Windows.Forms.Panel ControlPanel;
		private System.Windows.Forms.Button ResetButton;
		private System.Windows.Forms.ToolTip LightControlToolTip;
		private System.Windows.Forms.Timer LightConnectTimer;
		private System.Windows.Forms.Timer TraceConnectTimer;
		private System.Windows.Forms.Label LocationLabel;
	}
}