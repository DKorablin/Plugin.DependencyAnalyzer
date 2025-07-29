using System;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	internal partial class RangeEditorCtrl : UserControl
	{
		private DataObjectSave _ctrlMain;

		public Double Value => this.Convert(tbScale.Value);

		public RangeEditorCtrl(DataObjectSave ctrlMain)
		{
			this._ctrlMain = ctrlMain;
			this.InitializeComponent();
			tbScale.Value = this.ReverseConvert(this._ctrlMain.ImageScale);
			this.SetScaleLabelTexts();
		}

		private void imageScale_ValueChanged(Object sender, EventArgs e)
		{
			Double a = this.Convert(tbScale.Value);
			Int32 b = this.ReverseConvert(a);
			if(b != tbScale.Value)
				throw new InvalidOperationException();

			this.SetScaleLabelTexts();
		}

		private Int32 ReverseConvert(Double value)
			=> (Int32)Math.Round((((Double)((value - 1) / 9) * (tbScale.Maximum - tbScale.Minimum)) + tbScale.Minimum));

		private Double Convert(Int32 value)
		{
			Double percent = (Double)(value - tbScale.Minimum) / (tbScale.Maximum - tbScale.Minimum);
			return 1.0 + percent * 9.0;
		}

		private void SetScaleLabelTexts()
		{
			Double scale = this.Value;
			System.Drawing.Size size = this._ctrlMain.GetScale(scale);
			ttMain.SetToolTip(tbScale, $"Image scale is {scale}");

			lblImageSize.Text = String.Join(" x ", size.Width, size.Height);
		}
	}
}