namespace Plugin.DependencyAnalyzer.UI
{
	partial class MessageCtrl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageCtrl));
			this.bnClose = new System.Windows.Forms.Button();
			this.ilIcons = new System.Windows.Forms.ImageList(this.components);
			this.lblMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bnClose
			// 
			this.bnClose.Dock = System.Windows.Forms.DockStyle.Right;
			this.bnClose.FlatAppearance.BorderSize = 0;
			this.bnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.bnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.bnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.bnClose.ImageIndex = 0;
			this.bnClose.ImageList = this.ilIcons;
			this.bnClose.Location = new System.Drawing.Point(366, 0);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(30, 29);
			this.bnClose.TabIndex = 7;
			this.bnClose.TabStop = false;
			this.bnClose.UseVisualStyleBackColor = true;
			this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
			this.bnClose.MouseLeave += new System.EventHandler(this.bnClose_MouseLeave);
			this.bnClose.MouseHover += new System.EventHandler(this.bnClose_MouseHover);
			// 
			// ilIcons
			// 
			this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
			this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.ilIcons.Images.SetKeyName(0, "findClose.ico");
			this.ilIcons.Images.SetKeyName(1, "findClose_Hover.ico");
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Location = new System.Drawing.Point(3, 8);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(0, 13);
			this.lblMessage.TabIndex = 8;
			// 
			// MessageCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.bnClose);
			this.Name = "MessageCtrl";
			this.Size = new System.Drawing.Size(396, 29);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnClose;
		private System.Windows.Forms.ImageList ilIcons;
		private System.Windows.Forms.Label lblMessage;
	}
}
