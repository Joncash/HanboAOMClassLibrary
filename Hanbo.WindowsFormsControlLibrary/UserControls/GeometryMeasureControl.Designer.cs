namespace Hanbo.WindowsFormsControlLibrary.UserControls
{
	partial class GeometryMeasureControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeometryMeasureControl));
			this.MM_TabControl = new System.Windows.Forms.TabControl();
			this.MM_TabPage1 = new System.Windows.Forms.TabPage();
			this.MM_TabPage1_FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.checkBox13 = new System.Windows.Forms.CheckBox();
			this.checkBox14 = new System.Windows.Forms.CheckBox();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			this.checkBox7 = new System.Windows.Forms.CheckBox();
			this.checkBox8 = new System.Windows.Forms.CheckBox();
			this.checkBox9 = new System.Windows.Forms.CheckBox();
			this.checkBox10 = new System.Windows.Forms.CheckBox();
			this.checkBox11 = new System.Windows.Forms.CheckBox();
			this.checkBox12 = new System.Windows.Forms.CheckBox();
			this.MM_ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MM_TabControl.SuspendLayout();
			this.MM_TabPage1.SuspendLayout();
			this.MM_TabPage1_FlowPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// MM_TabControl
			// 
			this.MM_TabControl.Controls.Add(this.MM_TabPage1);
			resources.ApplyResources(this.MM_TabControl, "MM_TabControl");
			this.MM_TabControl.Name = "MM_TabControl";
			this.MM_TabControl.SelectedIndex = 0;
			// 
			// MM_TabPage1
			// 
			this.MM_TabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.MM_TabPage1.Controls.Add(this.MM_TabPage1_FlowPanel);
			resources.ApplyResources(this.MM_TabPage1, "MM_TabPage1");
			this.MM_TabPage1.Name = "MM_TabPage1";
			// 
			// MM_TabPage1_FlowPanel
			// 
			resources.ApplyResources(this.MM_TabPage1_FlowPanel, "MM_TabPage1_FlowPanel");
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox1);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox2);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox3);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox4);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox5);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox13);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox14);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox6);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox7);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox8);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox9);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox10);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox11);
			this.MM_TabPage1_FlowPanel.Controls.Add(this.checkBox12);
			this.MM_TabPage1_FlowPanel.Name = "MM_TabPage1_FlowPanel";
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_point_blue32;
			this.checkBox1.Name = "checkBox1";
			this.MM_ToolTip.SetToolTip(this.checkBox1, resources.GetString("checkBox1.ToolTip"));
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// checkBox2
			// 
			resources.ApplyResources(this.checkBox2, "checkBox2");
			this.checkBox2.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_line_blue32;
			this.checkBox2.Name = "checkBox2";
			this.MM_ToolTip.SetToolTip(this.checkBox2, resources.GetString("checkBox2.ToolTip"));
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
			// 
			// checkBox3
			// 
			resources.ApplyResources(this.checkBox3, "checkBox3");
			this.checkBox3.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_circle_blue32;
			this.checkBox3.Name = "checkBox3";
			this.MM_ToolTip.SetToolTip(this.checkBox3, resources.GetString("checkBox3.ToolTip"));
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
			// 
			// checkBox4
			// 
			resources.ApplyResources(this.checkBox4, "checkBox4");
			this.checkBox4.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_smart_point;
			this.checkBox4.Name = "checkBox4";
			this.MM_ToolTip.SetToolTip(this.checkBox4, resources.GetString("checkBox4.ToolTip"));
			this.checkBox4.UseVisualStyleBackColor = true;
			this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
			// 
			// checkBox5
			// 
			resources.ApplyResources(this.checkBox5, "checkBox5");
			this.checkBox5.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_smart_line;
			this.checkBox5.Name = "checkBox5";
			this.MM_ToolTip.SetToolTip(this.checkBox5, resources.GetString("checkBox5.ToolTip"));
			this.checkBox5.UseVisualStyleBackColor = true;
			this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
			// 
			// checkBox13
			// 
			resources.ApplyResources(this.checkBox13, "checkBox13");
			this.checkBox13.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_smart_circle;
			this.checkBox13.Name = "checkBox13";
			this.MM_ToolTip.SetToolTip(this.checkBox13, resources.GetString("checkBox13.ToolTip"));
			this.checkBox13.UseVisualStyleBackColor = true;
			this.checkBox13.CheckedChanged += new System.EventHandler(this.checkBox13_CheckedChanged);
			// 
			// checkBox14
			// 
			resources.ApplyResources(this.checkBox14, "checkBox14");
			this.checkBox14.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.draw_smart_arc;
			this.checkBox14.Name = "checkBox14";
			this.MM_ToolTip.SetToolTip(this.checkBox14, resources.GetString("checkBox14.ToolTip"));
			this.checkBox14.UseVisualStyleBackColor = true;
			this.checkBox14.CheckedChanged += new System.EventHandler(this.checkBox14_CheckedChanged);
			// 
			// checkBox6
			// 
			resources.ApplyResources(this.checkBox6, "checkBox6");
			this.checkBox6.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.distance_blue32;
			this.checkBox6.Name = "checkBox6";
			this.MM_ToolTip.SetToolTip(this.checkBox6, resources.GetString("checkBox6.ToolTip"));
			this.checkBox6.UseVisualStyleBackColor = true;
			this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
			// 
			// checkBox7
			// 
			resources.ApplyResources(this.checkBox7, "checkBox7");
			this.checkBox7.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources._3pointCircle_blue;
			this.checkBox7.Name = "checkBox7";
			this.MM_ToolTip.SetToolTip(this.checkBox7, resources.GetString("checkBox7.ToolTip"));
			this.checkBox7.UseVisualStyleBackColor = true;
			this.checkBox7.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
			// 
			// checkBox8
			// 
			resources.ApplyResources(this.checkBox8, "checkBox8");
			this.checkBox8.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.symmetryLine_blue32;
			this.checkBox8.Name = "checkBox8";
			this.MM_ToolTip.SetToolTip(this.checkBox8, resources.GetString("checkBox8.ToolTip"));
			this.checkBox8.UseVisualStyleBackColor = true;
			this.checkBox8.CheckedChanged += new System.EventHandler(this.checkBox8_CheckedChanged);
			// 
			// checkBox9
			// 
			resources.ApplyResources(this.checkBox9, "checkBox9");
			this.checkBox9.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.angle_blue32;
			this.checkBox9.Name = "checkBox9";
			this.MM_ToolTip.SetToolTip(this.checkBox9, resources.GetString("checkBox9.ToolTip"));
			this.checkBox9.UseVisualStyleBackColor = true;
			this.checkBox9.CheckedChanged += new System.EventHandler(this.checkBox9_CheckedChanged);
			// 
			// checkBox10
			// 
			resources.ApplyResources(this.checkBox10, "checkBox10");
			this.checkBox10.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.crossPoint_blue32;
			this.checkBox10.Name = "checkBox10";
			this.MM_ToolTip.SetToolTip(this.checkBox10, resources.GetString("checkBox10.ToolTip"));
			this.checkBox10.UseVisualStyleBackColor = true;
			this.checkBox10.CheckedChanged += new System.EventHandler(this.checkBox10_CheckedChanged);
			// 
			// checkBox11
			// 
			resources.ApplyResources(this.checkBox11, "checkBox11");
			this.checkBox11.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.distanceX_32;
			this.checkBox11.Name = "checkBox11";
			this.MM_ToolTip.SetToolTip(this.checkBox11, resources.GetString("checkBox11.ToolTip"));
			this.checkBox11.UseVisualStyleBackColor = true;
			this.checkBox11.CheckedChanged += new System.EventHandler(this.checkBox11_CheckedChanged);
			// 
			// checkBox12
			// 
			resources.ApplyResources(this.checkBox12, "checkBox12");
			this.checkBox12.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.distanceY_32;
			this.checkBox12.Name = "checkBox12";
			this.MM_ToolTip.SetToolTip(this.checkBox12, resources.GetString("checkBox12.ToolTip"));
			this.checkBox12.UseVisualStyleBackColor = true;
			this.checkBox12.CheckedChanged += new System.EventHandler(this.checkBox12_CheckedChanged);
			// 
			// GeometryMeasureControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.MM_TabControl);
			this.Name = "GeometryMeasureControl";
			this.MM_TabControl.ResumeLayout(false);
			this.MM_TabPage1.ResumeLayout(false);
			this.MM_TabPage1_FlowPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl MM_TabControl;
		private System.Windows.Forms.TabPage MM_TabPage1;
		private System.Windows.Forms.FlowLayoutPanel MM_TabPage1_FlowPanel;
		private System.Windows.Forms.ToolTip MM_ToolTip;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.CheckBox checkBox7;
		private System.Windows.Forms.CheckBox checkBox8;
		private System.Windows.Forms.CheckBox checkBox9;
		private System.Windows.Forms.CheckBox checkBox10;
		private System.Windows.Forms.CheckBox checkBox11;
		private System.Windows.Forms.CheckBox checkBox12;
		private System.Windows.Forms.CheckBox checkBox13;
		private System.Windows.Forms.CheckBox checkBox14;


	}
}
