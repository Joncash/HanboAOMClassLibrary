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
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.ControlPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(3, 11);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(31, 13);
			this.label17.TabIndex = 54;
			this.label17.Text = "頻道";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(3, 36);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(31, 13);
			this.label19.TabIndex = 55;
			this.label19.Text = "亮度";
			// 
			// L1CheckBox
			// 
			this.L1CheckBox.AutoSize = true;
			this.L1CheckBox.Location = new System.Drawing.Point(42, 11);
			this.L1CheckBox.Name = "L1CheckBox";
			this.L1CheckBox.Size = new System.Drawing.Size(38, 17);
			this.L1CheckBox.TabIndex = 56;
			this.L1CheckBox.Tag = "00";
			this.L1CheckBox.Text = "L1";
			this.L1CheckBox.UseVisualStyleBackColor = true;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(42, 34);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(44, 20);
			this.numericUpDown1.TabIndex = 57;
			this.numericUpDown1.Tag = "00";
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(163, 34);
			this.numericUpDown3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(44, 20);
			this.numericUpDown3.TabIndex = 61;
			this.numericUpDown3.Tag = "02";
			// 
			// L2CheckBox
			// 
			this.L2CheckBox.AutoSize = true;
			this.L2CheckBox.Location = new System.Drawing.Point(104, 11);
			this.L2CheckBox.Name = "L2CheckBox";
			this.L2CheckBox.Size = new System.Drawing.Size(38, 17);
			this.L2CheckBox.TabIndex = 58;
			this.L2CheckBox.Tag = "01";
			this.L2CheckBox.Text = "L2";
			this.L2CheckBox.UseVisualStyleBackColor = true;
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(104, 34);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(44, 20);
			this.numericUpDown2.TabIndex = 60;
			this.numericUpDown2.Tag = "01";
			// 
			// L3CheckBox
			// 
			this.L3CheckBox.AutoSize = true;
			this.L3CheckBox.Location = new System.Drawing.Point(163, 11);
			this.L3CheckBox.Name = "L3CheckBox";
			this.L3CheckBox.Size = new System.Drawing.Size(38, 17);
			this.L3CheckBox.TabIndex = 59;
			this.L3CheckBox.Tag = "02";
			this.L3CheckBox.Text = "L3";
			this.L3CheckBox.UseVisualStyleBackColor = true;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.IPLabel,
            this.PortLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 75);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(296, 22);
			this.statusStrip1.TabIndex = 66;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(50, 17);
			this.StatusLabel.Text = "Loading";
			// 
			// IPLabel
			// 
			this.IPLabel.Name = "IPLabel";
			this.IPLabel.Size = new System.Drawing.Size(90, 17);
			this.IPLabel.Text = "IP Address: N/A";
			// 
			// PortLabel
			// 
			this.PortLabel.Name = "PortLabel";
			this.PortLabel.Size = new System.Drawing.Size(57, 17);
			this.PortLabel.Text = "Port: N/A";
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
			this.ControlPanel.Location = new System.Drawing.Point(-3, 0);
			this.ControlPanel.Name = "ControlPanel";
			this.ControlPanel.Size = new System.Drawing.Size(210, 70);
			this.ControlPanel.TabIndex = 68;
			// 
			// ResetButton
			// 
			this.ResetButton.Image = global::PD3_Ethernet_LightControl.Properties.Resources.power_Off;
			this.ResetButton.Location = new System.Drawing.Point(213, 1);
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.Size = new System.Drawing.Size(75, 68);
			this.ResetButton.TabIndex = 67;
			this.LightControlToolTip.SetToolTip(this.ResetButton, "開啟連線");
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
			// CCSLightControlForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 97);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.ControlPanel);
			this.Controls.Add(this.ResetButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CCSLightControlForm";
			this.Text = "光源控制器";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CCSLightControlForm_FormClosing);
			this.Load += new System.EventHandler(this.CCSLightControlForm_Load);
			this.Shown += new System.EventHandler(this.CCSLightControlForm_Shown);
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
	}
}