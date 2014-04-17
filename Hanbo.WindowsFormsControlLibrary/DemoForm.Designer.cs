namespace Hanbo.WindowsFormsControlLibrary
{
	partial class DemoForm
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
			this.WinStatusStrip = new Hanbo.WindowsFormsControlLibrary.HalconStatusStrip();
			this.customHalconControl1 = new Hanbo.WindowsFormsControlLibrary.CustomHalconControl();
			this.customHalconControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// WinStatusStrip
			// 
			this.WinStatusStrip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.WinStatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.WinStatusStrip.Location = new System.Drawing.Point(0, 387);
			this.WinStatusStrip.Name = "WinStatusStrip";
			this.WinStatusStrip.Size = new System.Drawing.Size(764, 22);
			this.WinStatusStrip.TabIndex = 0;
			// 
			// customHalconControl1
			// 
			this.customHalconControl1.AutoSize = true;
			// 
			// customHalconControl1.Panel_Halcon
			// 
			this.customHalconControl1.HalconContainer.BackColor = System.Drawing.Color.CornflowerBlue;
			this.customHalconControl1.HalconContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.customHalconControl1.HalconContainer.Location = new System.Drawing.Point(0, 31);
			this.customHalconControl1.HalconContainer.Name = "Panel_Halcon";
			this.customHalconControl1.HalconContainer.Padding = new System.Windows.Forms.Padding(3);
			this.customHalconControl1.HalconContainer.Size = new System.Drawing.Size(726, 346);
			this.customHalconControl1.HalconContainer.TabIndex = 1;
			this.customHalconControl1.Location = new System.Drawing.Point(0, 0);
			this.customHalconControl1.Name = "customHalconControl1";
			this.customHalconControl1.Size = new System.Drawing.Size(726, 377);
			this.customHalconControl1.TabIndex = 1;
			// 
			// DemoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(764, 409);
			this.Controls.Add(this.customHalconControl1);
			this.Controls.Add(this.WinStatusStrip);
			this.Name = "DemoForm";
			this.Text = "DemoForm";
			this.customHalconControl1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private HalconStatusStrip WinStatusStrip;
		private CustomHalconControl customHalconControl1;
	}
}