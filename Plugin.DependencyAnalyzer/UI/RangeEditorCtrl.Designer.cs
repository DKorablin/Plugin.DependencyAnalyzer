namespace Plugin.DependencyAnalyzer.UI
{
	partial class RangeEditorCtrl
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
			this.lblImageSize = new System.Windows.Forms.Label();
			this.tbScale = new System.Windows.Forms.TrackBar();
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.tbScale)).BeginInit();
			this.SuspendLayout();
			// 
			// lblImageSize
			// 
			this.lblImageSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblImageSize.AutoSize = true;
			this.lblImageSize.Location = new System.Drawing.Point(96, 13);
			this.lblImageSize.Name = "lblImageSize";
			this.lblImageSize.Size = new System.Drawing.Size(59, 13);
			this.lblImageSize.TabIndex = 1;
			this.lblImageSize.Text = "Image Size";
			// 
			// tbScale
			// 
			this.tbScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbScale.Location = new System.Drawing.Point(0, 0);
			this.tbScale.Maximum = 100;
			this.tbScale.Minimum = 10;
			this.tbScale.Name = "tbScale";
			this.tbScale.Size = new System.Drawing.Size(103, 45);
			this.tbScale.TabIndex = 0;
			this.tbScale.TickFrequency = 5;
			this.tbScale.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.tbScale.Value = 10;
			this.tbScale.ValueChanged += new System.EventHandler(this.imageScale_ValueChanged);
			// 
			// RangeEditorCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblImageSize);
			this.Controls.Add(this.tbScale);
			this.Name = "RangeEditorCtrl";
			this.Size = new System.Drawing.Size(168, 44);
			((System.ComponentModel.ISupportInitialize)(this.tbScale)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblImageSize;
		private System.Windows.Forms.TrackBar tbScale;
		private System.Windows.Forms.ToolTip ttMain;
	}
}
