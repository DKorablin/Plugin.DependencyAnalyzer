using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI.Graph
{
	internal partial class SelectedPECtrl : UserControl
	{
		private DataObjectSelected _selectedObject;
		public DataObjectSelected SelectedObject
		{
			get => this._selectedObject;
			set
			{
				this._selectedObject = value;
				pgData.SelectedObject = value?.Library;
			}
		}

		public SelectedPECtrl()
			=> this.InitializeComponent();
	}
}