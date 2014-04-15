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
			this.SuspendLayout();
			// 
			// WinStatusStrip
			// 
			this.WinStatusStrip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.WinStatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.WinStatusStrip.Location = new System.Drawing.Point(0, 239);
			this.WinStatusStrip.Name = "WinStatusStrip";
			this.WinStatusStrip.Size = new System.Drawing.Size(938, 22);
			this.WinStatusStrip.TabIndex = 0;
			// 
			// DemoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(938, 261);
			this.Controls.Add(this.WinStatusStrip);
			this.Name = "DemoForm";
			this.Text = "DemoForm";
			this.ResumeLayout(false);

		}

		#endregion

		private HalconStatusStrip WinStatusStrip;
	}
}