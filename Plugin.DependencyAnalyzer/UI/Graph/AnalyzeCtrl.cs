using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer.UI.Graph
{
	internal partial class AnalyzeCtrl : UserControl
	{
		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;
		private IWindow Window
		{
			get
			{
				Control ctrl = base.Parent;
				while(ctrl != null)
				{
					if(ctrl is IWindow wnd)
						return wnd;
					else
						ctrl = ctrl.Parent;
				}
				return null;
			}
		}

		private DataObjectAnalyze _dataObject;

		public DataObjectAnalyze SelectedObject
		{
			get => this._dataObject;
			set
			{
				this._dataObject = value;

				tvReferences.ParentLibrary = this.SelectedObject.Library;

				ddlChildAssemblies.Items.Clear();
				this.FillAssemblies(this.SelectedObject.ChildAssemblies);

				//this.DataBind(this.SelectedObject.ChildAssembly);
			}
		}

		public AnalyzeCtrl()
			=> this.InitializeComponent();

		protected override void OnCreateControl()
		{
			bnHilight.Checked = this.Plugin.Settings.HighlightReferencedMembers;
			base.OnCreateControl();
		}

		private static readonly ToolStripMenuItem AllItems = new ToolStripMenuItem("All");
		private void FillAssemblies(IEnumerable<Library> assemblies)
		{
			List<ToolStripMenuItem> itemsToAdd = new List<ToolStripMenuItem>();
			foreach(Library library in assemblies)
			{
				Boolean isFound = false;
				foreach(ToolStripMenuItem addedItem in ddlChildAssemblies.Items)
					if(addedItem.Tag is Library lib && lib.Name == library.Name)
					{//Cut already added assembly, if additional assemblies added from browse button
						isFound = true;
						break;
					}

				if(!isFound)
					itemsToAdd.Add(new ToolStripMenuItem(library.ShowAsString()) { Tag = library });
			}
			if(itemsToAdd.Count == 0)
				ctlMessage.ShowMessage(MessageCtrl.StatusMessageType.Failed, "Referenced libraries not found");
			else
				ctlMessage.ShowMessage(MessageCtrl.StatusMessageType.Success, $"{itemsToAdd.Count} libraries added");
			ddlChildAssemblies.Items.AddRange(itemsToAdd.ToArray());

			if(ddlChildAssemblies.Items.Count > 0 && itemsToAdd.Count > 0)
				ddlChildAssemblies.SelectedItem = itemsToAdd[0];

			if(ddlChildAssemblies.Items.Count > 1 && ddlChildAssemblies.Items[0] != AnalyzeCtrl.AllItems)
				ddlChildAssemblies.Items.Insert(0, AllItems);
		}

		private void tsMain_Resize(Object sender, EventArgs e)
		{
			Int32 paddingAndMargin = 0;
			foreach(ToolStripItem ctrl in tsMain.Items)
				paddingAndMargin += ctrl.Padding.Horizontal + ctrl.Margin.Horizontal;
			ddlChildAssemblies.Width = tsMain.Width - (bnHilight.Width + bnAdd.Width + bnRemove.Width + tsMain.Padding.Horizontal + paddingAndMargin);
		}

		private void ddlChildAssemblies_SelectedIndexChanged(Object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)ddlChildAssemblies.SelectedItem;
			if(menuItem == null)
				return;

			List<Library> childLibraries = new List<Library>();
			if(menuItem == AnalyzeCtrl.AllItems)
			{
				bnRemove.Enabled = false;
				foreach(ToolStripMenuItem item in ddlChildAssemblies.Items)
					if(item.Tag != null)
						childLibraries.Add((Library)item.Tag);
			} else
			{
				bnRemove.Enabled = true;
				childLibraries.Add((Library)menuItem.Tag);
			}

			ReferencesTreeView.ShowResult result = tvReferences.ShowReferencedMembers(childLibraries);

			List<String> status = new List<String>();
			if(result.NotUsed > 0)
				status.Add("Not used: " + result.NotUsed.ToString("N0"));
			if(result.Used > 0)
				status.Add("Used: " + result.Used.ToString("N0"));
			if(result.Missed > 0)
				status.Add("Missed: " + result.Missed.ToString("N0"));
			lblStatus.Text = String.Join(" ", status);
		}

		private void bnHighlight_CheckedChanged(Object sender, EventArgs e)
		{
			this.Plugin.Settings.HighlightReferencedMembers = tvReferences.HighlightReferencedMembers = bnHilight.Checked;

			if(ddlChildAssemblies.Items.Count > 0)
				this.ddlChildAssemblies_SelectedIndexChanged(sender, e);
		}

		private void bnAdd_Click(Object sender, EventArgs e)
		{
			String[] filesPath;
			using(OpenFileDialog dlg = new OpenFileDialog()
			{
				CheckFileExists = true,
				Multiselect = true,
				DefaultExt = "dll",
				Filter = "Executable files (*.dll;*.exe)|*.dll;*.exe|All files (*.*)|*.*",
				Title = "Open referenced libraries to scan them for dependencies",
			})
				if(dlg.ShowDialog() == DialogResult.OK)
					filesPath = dlg.FileNames;
				else
					return;

			foreach(String filePath in filesPath)
			{
				LibraryAnalyzer analyzer = new LibraryAnalyzer(this.Plugin.Trace, filePath, this.Plugin.Settings.SearchType);
				this.FillAssemblies(analyzer.GetReferencedLibraries(this.SelectedObject.Library));
			}
		}

		private void bnRemove_Click(Object sender, EventArgs e)
		{
			ToolStripMenuItem selectedItem = (ToolStripMenuItem)ddlChildAssemblies.SelectedItem;
			if(selectedItem == null)
				return;
			else if(selectedItem == AnalyzeCtrl.AllItems)
				throw new InvalidOperationException();

			ddlChildAssemblies.Items.Remove(selectedItem);
		}

		private void tvReferences_KeyDown(Object sender, KeyEventArgs e)
		{
			if(e.Control && !e.Alt)
				switch(e.KeyCode)
				{
				case Keys.O:
					this.bnAdd_Click(sender, e);
					e.Handled = true;
					break;
				case Keys.H:
					bnHilight.Checked = !bnHilight.Checked;
					e.Handled = true;
					break;
				}
		}
		private void tvReferences_MouseClick(Object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				TreeViewHitTestInfo info = tvReferences.HitTest(e.Location);
				if(info.Node != null)
				{
					tvReferences.SelectedNode = info.Node;
					this.OpenContextMenu(e.Location);
				}
			}
		}

		private void cmsReferences_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			cmsReferences.Close();

			TreeNode node = tvReferences.SelectedNode;
			if(e.ClickedItem == tsmiReferencesCopy)
				Clipboard.SetText(node.Text);
		}

		private void OpenContextMenu(Point location)
			=> cmsReferences.Show(tvReferences, location);
	}
}