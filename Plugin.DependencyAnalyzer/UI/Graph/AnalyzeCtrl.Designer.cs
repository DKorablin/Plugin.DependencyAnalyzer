namespace Plugin.DependencyAnalyzer.UI.Graph
{
	partial class AnalyzeCtrl
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
			if(disposing && (components != null))
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
			System.Windows.Forms.StatusStrip status;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalyzeCtrl));
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.ddlChildAssemblies = new System.Windows.Forms.ToolStripComboBox();
			this.bnAdd = new System.Windows.Forms.ToolStripButton();
			this.bnHilight = new System.Windows.Forms.ToolStripButton();
			this.tvReferences = new Plugin.DependencyAnalyzer.UI.ReferencesTreeView();
			this.ilObjects = new System.Windows.Forms.ImageList(this.components);
			this.ctlMessage = new Plugin.DependencyAnalyzer.UI.MessageCtrl();
			this.cmsReferences = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiReferencesCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.bnRemove = new System.Windows.Forms.ToolStripButton();
			status = new System.Windows.Forms.StatusStrip();
			status.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.cmsReferences.SuspendLayout();
			this.SuspendLayout();
			// 
			// status
			// 
			status.ImageScalingSize = new System.Drawing.Size(20, 20);
			status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
			status.Location = new System.Drawing.Point(0, 124);
			status.Name = "status";
			status.Size = new System.Drawing.Size(204, 26);
			status.TabIndex = 2;
			status.Text = "statusStrip1";
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(15, 20);
			this.lblStatus.Text = "_";
			// 
			// tsMain
			// 
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddlChildAssemblies,
			this.bnHilight,
			this.bnAdd,
            this.bnRemove});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(204, 28);
			this.tsMain.TabIndex = 1;
			this.tsMain.TabStop = true;
			this.tsMain.Resize += new System.EventHandler(this.tsMain_Resize);
			// 
			// ddlChildAssemblies
			// 
			this.ddlChildAssemblies.AutoSize = false;
			this.ddlChildAssemblies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlChildAssemblies.Name = "ddlChildAssemblies";
			this.ddlChildAssemblies.Size = new System.Drawing.Size(121, 28);
			this.ddlChildAssemblies.Sorted = true;
			this.ddlChildAssemblies.SelectedIndexChanged += new System.EventHandler(this.ddlChildAssemblies_SelectedIndexChanged);
			// 
			// bnAdd
			// 
			this.bnAdd.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.bnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnAdd.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconOpen;
			this.bnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnAdd.Name = "bnAdd";
			this.bnAdd.Size = new System.Drawing.Size(29, 25);
			this.bnAdd.Text = "Browse referenced assemblies for analyze (Ctrl+O)";
			this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
			// 
			// bnRemove
			// 
			this.bnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnRemove.Enabled = false;
			this.bnRemove.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconDelete;
			this.bnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnRemove.Name = "bnRemove";
			this.bnRemove.Size = new System.Drawing.Size(29, 24);
			this.bnRemove.Text = "Remove selected assembly from analyze";
			this.bnAdd.Click += new System.EventHandler(this.bnRemove_Click);
			// 
			// bnHilight
			// 
			this.bnHilight.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.bnHilight.CheckOnClick = true;
			this.bnHilight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnHilight.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.Filter;
			this.bnHilight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnHilight.Name = "bnHilight";
			this.bnHilight.Size = new System.Drawing.Size(29, 25);
			this.bnHilight.Text = "Show all members and hilight used (Ctrl+H)";
			this.bnHilight.CheckedChanged += new System.EventHandler(this.bnHilight_CheckedChanged);
			// 
			// tvReferences
			// 
			this.tvReferences.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReferences.HideSelection = false;
			this.tvReferences.HilightReferencedMembers = false;
			this.tvReferences.ImageIndex = 0;
			this.tvReferences.ImageList = this.ilObjects;
			this.tvReferences.Location = new System.Drawing.Point(0, 64);
			this.tvReferences.Name = "tvReferences";
			this.tvReferences.ParentLibrary = null;
			this.tvReferences.SelectedImageIndex = 0;
			this.tvReferences.Size = new System.Drawing.Size(204, 60);
			this.tvReferences.TabIndex = 0;
			this.tvReferences.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvReferences_KeyDown);
			this.tvReferences.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvReferences_MouseClick);
			// 
			// ilObjects
			// 
			this.ilObjects.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilObjects.ImageStream")));
			this.ilObjects.TransparentColor = System.Drawing.Color.Fuchsia;
			this.ilObjects.Images.SetKeyName(0, "iconFolder.bmp");
			this.ilObjects.Images.SetKeyName(1, "Assembly.bmp");
			this.ilObjects.Images.SetKeyName(2, "Namespace.bmp");
			this.ilObjects.Images.SetKeyName(3, "Class.Public.bmp");
			this.ilObjects.Images.SetKeyName(4, "Method.Public.bmp");
			this.ilObjects.Images.SetKeyName(5, "Property.Public.bmp");
			// 
			// ctlMessage
			// 
			this.ctlMessage.Dock = System.Windows.Forms.DockStyle.Top;
			this.ctlMessage.Location = new System.Drawing.Point(0, 28);
			this.ctlMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ctlMessage.Name = "ctlMessage";
			this.ctlMessage.Size = new System.Drawing.Size(204, 36);
			this.ctlMessage.TabIndex = 1;
			this.ctlMessage.Visible = false;
			// 
			// cmsReferences
			// 
			this.cmsReferences.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsReferences.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReferencesCopy});
			this.cmsReferences.Name = "cmsReferences";
			this.cmsReferences.Size = new System.Drawing.Size(113, 28);
			this.cmsReferences.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsReferences_ItemClicked);
			// 
			// tsmiReferencesCopy
			// 
			this.tsmiReferencesCopy.Name = "tsmiReferencesCopy";
			this.tsmiReferencesCopy.Size = new System.Drawing.Size(112, 24);
			this.tsmiReferencesCopy.Text = "&Copy";
			// 
			// AnalyzeCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tvReferences);
			this.Controls.Add(this.ctlMessage);
			this.Controls.Add(this.tsMain);
			this.Controls.Add(status);
			this.Name = "AnalyzeCtrl";
			this.Size = new System.Drawing.Size(204, 150);
			status.ResumeLayout(false);
			status.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.cmsReferences.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ReferencesTreeView tvReferences;
		private MessageCtrl ctlMessage;
		private System.Windows.Forms.ToolStripComboBox ddlChildAssemblies;
		private System.Windows.Forms.ToolStripButton bnHilight;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripButton bnAdd;
		private System.Windows.Forms.ToolStripButton bnRemove;
		private System.Windows.Forms.ImageList ilObjects;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
		private System.Windows.Forms.ContextMenuStrip cmsReferences;
		private System.Windows.Forms.ToolStripMenuItem tsmiReferencesCopy;
	}
}
