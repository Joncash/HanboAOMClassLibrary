namespace Hanbo.WindowsFormsControlLibrary.UserControls
{
	partial class GeoDataUserControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeoDataUserControl));
			this.ContentPanel = new System.Windows.Forms.Panel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.GeoDataGridView = new System.Windows.Forms.DataGridView();
			this.GeoTreeViewGroupBox = new System.Windows.Forms.GroupBox();
			this.GeoTreeView = new System.Windows.Forms.TreeView();
			this.GeoToolStrip = new System.Windows.Forms.ToolStrip();
			this.UnitComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.UnitComboBoxLabel = new System.Windows.Forms.ToolStripLabel();
			this.CoordinateComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.CoordinateLabel = new System.Windows.Forms.ToolStripLabel();
			this.ExportButton = new System.Windows.Forms.ToolStripButton();
			this.ClearListButton = new System.Windows.Forms.ToolStripButton();
			this.GeoContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SetCoordinateSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContentPanel.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.GeoDataGridView)).BeginInit();
			this.GeoTreeViewGroupBox.SuspendLayout();
			this.GeoToolStrip.SuspendLayout();
			this.GeoContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// ContentPanel
			// 
			this.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
			this.ContentPanel.Controls.Add(this.statusStrip1);
			this.ContentPanel.Controls.Add(this.splitContainer1);
			this.ContentPanel.Controls.Add(this.GeoToolStrip);
			resources.ApplyResources(this.ContentPanel, "ContentPanel");
			this.ContentPanel.Name = "ContentPanel";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			resources.ApplyResources(this.statusStrip1, "statusStrip1");
			this.statusStrip1.Name = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Green;
			this.splitContainer1.Panel1.Controls.Add(this.GeoDataGridView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.GeoTreeViewGroupBox);
			// 
			// GeoDataGridView
			// 
			this.GeoDataGridView.AllowUserToAddRows = false;
			this.GeoDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.GeoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.GeoDataGridView, "GeoDataGridView");
			this.GeoDataGridView.Name = "GeoDataGridView";
			// 
			// GeoTreeViewGroupBox
			// 
			this.GeoTreeViewGroupBox.Controls.Add(this.GeoTreeView);
			resources.ApplyResources(this.GeoTreeViewGroupBox, "GeoTreeViewGroupBox");
			this.GeoTreeViewGroupBox.Name = "GeoTreeViewGroupBox";
			this.GeoTreeViewGroupBox.TabStop = false;
			// 
			// GeoTreeView
			// 
			resources.ApplyResources(this.GeoTreeView, "GeoTreeView");
			this.GeoTreeView.Name = "GeoTreeView";
			// 
			// GeoToolStrip
			// 
			this.GeoToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.GeoToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UnitComboBox,
            this.UnitComboBoxLabel,
            this.CoordinateComboBox,
            this.CoordinateLabel,
            this.ExportButton,
            this.ClearListButton});
			resources.ApplyResources(this.GeoToolStrip, "GeoToolStrip");
			this.GeoToolStrip.Name = "GeoToolStrip";
			// 
			// UnitComboBox
			// 
			this.UnitComboBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.UnitComboBox.Name = "UnitComboBox";
			resources.ApplyResources(this.UnitComboBox, "UnitComboBox");
			// 
			// UnitComboBoxLabel
			// 
			this.UnitComboBoxLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.UnitComboBoxLabel.AutoToolTip = true;
			this.UnitComboBoxLabel.Margin = new System.Windows.Forms.Padding(0, 1, 3, 2);
			this.UnitComboBoxLabel.Name = "UnitComboBoxLabel";
			resources.ApplyResources(this.UnitComboBoxLabel, "UnitComboBoxLabel");
			// 
			// CoordinateComboBox
			// 
			this.CoordinateComboBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.CoordinateComboBox.Margin = new System.Windows.Forms.Padding(1, 0, 15, 0);
			this.CoordinateComboBox.Name = "CoordinateComboBox";
			resources.ApplyResources(this.CoordinateComboBox, "CoordinateComboBox");
			// 
			// CoordinateLabel
			// 
			this.CoordinateLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.CoordinateLabel.Margin = new System.Windows.Forms.Padding(0, 1, 3, 2);
			this.CoordinateLabel.Name = "CoordinateLabel";
			resources.ApplyResources(this.CoordinateLabel, "CoordinateLabel");
			// 
			// ExportButton
			// 
			this.ExportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ExportButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.ExportFile;
			resources.ApplyResources(this.ExportButton, "ExportButton");
			this.ExportButton.Name = "ExportButton";
			// 
			// ClearListButton
			// 
			this.ClearListButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ClearListButton.Image = global::Hanbo.WindowsFormsControlLibrary.Properties.Resources.clearList_32;
			resources.ApplyResources(this.ClearListButton, "ClearListButton");
			this.ClearListButton.Name = "ClearListButton";
			// 
			// GeoContextMenuStrip
			// 
			this.GeoContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetCoordinateSystemToolStripMenuItem});
			this.GeoContextMenuStrip.Name = "GeoContextMenuStrip";
			resources.ApplyResources(this.GeoContextMenuStrip, "GeoContextMenuStrip");
			// 
			// SetCoordinateSystemToolStripMenuItem
			// 
			this.SetCoordinateSystemToolStripMenuItem.Name = "SetCoordinateSystemToolStripMenuItem";
			resources.ApplyResources(this.SetCoordinateSystemToolStripMenuItem, "SetCoordinateSystemToolStripMenuItem");
			this.SetCoordinateSystemToolStripMenuItem.Click += new System.EventHandler(this.SetCoordinateSystemToolStripMenuItem_Click);
			// 
			// GeoDataUserControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ContentPanel);
			this.Name = "GeoDataUserControl";
			this.ContentPanel.ResumeLayout(false);
			this.ContentPanel.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.GeoDataGridView)).EndInit();
			this.GeoTreeViewGroupBox.ResumeLayout(false);
			this.GeoToolStrip.ResumeLayout(false);
			this.GeoToolStrip.PerformLayout();
			this.GeoContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel ContentPanel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView GeoTreeView;
		private System.Windows.Forms.DataGridView GeoDataGridView;
		private System.Windows.Forms.ToolStrip GeoToolStrip;
		private System.Windows.Forms.ToolStripComboBox UnitComboBox;
		private System.Windows.Forms.ToolStripLabel UnitComboBoxLabel;
		private System.Windows.Forms.ToolStripComboBox CoordinateComboBox;
		private System.Windows.Forms.ToolStripLabel CoordinateLabel;
		private System.Windows.Forms.ToolStripButton ExportButton;
		private System.Windows.Forms.ToolStripButton ClearListButton;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.GroupBox GeoTreeViewGroupBox;
		private System.Windows.Forms.ContextMenuStrip GeoContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem SetCoordinateSystemToolStripMenuItem;
	}
}
