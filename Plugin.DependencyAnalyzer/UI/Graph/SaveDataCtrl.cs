using System;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI.Graph
{
	internal partial class SaveDataCtrl : UserControl
	{
		public DataObjectSave SelectedObject
		{
			get => (DataObjectSave)pgData.SelectedObject;
			set => pgData.SelectedObject = value;
		}

		public SaveDataCtrl()
			=> this.InitializeComponent();
	}
}