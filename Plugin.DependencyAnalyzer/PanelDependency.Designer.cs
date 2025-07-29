namespace Plugin.DependencyAnalyzer
{
	partial class PanelDependency
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
			components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.bwLoadDataObjectFromLibrary = new System.ComponentModel.BackgroundWorker();

			//
			// bwLoadDataObjectFromLibrary
			//
			this.bwLoadDataObjectFromLibrary.WorkerSupportsCancellation = true;
			this.bwLoadDataObjectFromLibrary.DoWork += this.bwLoadDataObjectFromLibrary_DoWork;
			this.bwLoadDataObjectFromLibrary.RunWorkerCompleted += this.bwLoadDataObjectFromLibrary_RunWorkerCompleted;
		}

		#endregion

		private System.ComponentModel.BackgroundWorker bwLoadDataObjectFromLibrary;
	}
}