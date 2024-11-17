using System;
using System.Drawing;
using System.Windows.Forms;

namespace Plugin.DependencyAnalyzer.UI
{
	internal partial class MessageCtrl : UserControl
	{
		public enum StatusMessageType
		{
			Success = 0,
			Progress = 1,
			Failed = 2,
		}

		private static Color[] StatusMessageColor = new Color[] { Color.LightCyan, Color.AntiqueWhite, Color.Pink, };

		public MessageCtrl()
		{
			InitializeComponent();
			this.Visible = false;
		}

		public void ShowMessage(StatusMessageType type, String message)
		{
			if(message == null)
				this.Visible = false;
			else
			{
				this.Visible = true;
				base.BackColor = MessageCtrl.StatusMessageColor[(Int32)type];
				lblMessage.Text = message;
			}
		}

		private void bnClose_MouseHover(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 1;

		private void bnClose_MouseLeave(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 0;

		private void bnClose_Click(Object sender, EventArgs e)
			=> this.Visible = false;
	}
}