
namespace Plugin.DependencyAnalyzer.UI.Graph
{
	partial class ReferencesCtrl
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
			this.lvLibraries = new System.Windows.Forms.ListView();
			this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// lvLibraries
			// 
			this.lvLibraries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
			this.lvLibraries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvLibraries.FullRowSelect = true;
			this.lvLibraries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvLibraries.HideSelection = false;
			this.lvLibraries.Location = new System.Drawing.Point(0, 0);
			this.lvLibraries.Name = "lvLibraries";
			this.lvLibraries.Size = new System.Drawing.Size(204, 150);
			this.lvLibraries.TabIndex = 0;
			this.lvLibraries.UseCompatibleStateImageBehavior = false;
			this.lvLibraries.View = System.Windows.Forms.View.Details;
			// 
			// colName
			// 
			this.colName.Text = "Name";
			// 
			// ReferencesCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lvLibraries);
			this.Name = "ReferencesCtrl";
			this.Size = new System.Drawing.Size(204, 150);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView lvLibraries;
		private System.Windows.Forms.ColumnHeader colName;
	}
}
