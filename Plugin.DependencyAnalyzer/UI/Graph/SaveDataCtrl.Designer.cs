namespace Plugin.DependencyAnalyzer.UI.Graph
{
	partial class SaveDataCtrl
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
			this.pgData = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// pgData
			// 
			this.pgData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgData.Location = new System.Drawing.Point(0, 0);
			this.pgData.Name = "pgData";
			this.pgData.Size = new System.Drawing.Size(150, 150);
			this.pgData.TabIndex = 0;
			this.pgData.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.pgData.ToolbarVisible = false;
			// 
			// SaveDataCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pgData);
			this.Name = "SaveDataCtrl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid pgData;
	}
}
