using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	internal class ListViewLibraryNodes : AlphaOmega.Windows.Forms.SortableListView
	{
		private ColumnHeader colNodesName = new ColumnHeader() { Text = "Name", };
		private ColumnHeader colNodesPath = new ColumnHeader() { Text = "Path", };
		private ColumnHeader colNodesVersion = new ColumnHeader() { Text = "Version" };
		private	ColumnHeader colNodesAssemblyName = new ColumnHeader() { Text = "Assembly Name" };

		public event EventHandler<EventArgs> OnNodeVisibilityChanged;

		public ListViewLibraryNodes() {
			this.AllowColumnReorder = true;
			this.CheckBoxes = true;
			this.FullRowSelect = true;
			this.GridLines = true;
			this.View = View.Details;

			this.Columns.AddRange(new ColumnHeader[] {
				this.colNodesName,
				this.colNodesPath,
				this.colNodesVersion,
				this.colNodesAssemblyName});

			this.Groups.AddRange(new ListViewGroup[] {
				new ListViewGroup("Found", HorizontalAlignment.Left) { Header = "Found", },
				new ListViewGroup("Not found", HorizontalAlignment.Left) { Header = "Not found", }
			});
		}

		/// <summary>Render assemblies from Graph as a list</summary>
		public void RenderAssembliesInList(PluginSettings settings, IEnumerable<Node> nodes)
		{
			base.Items.Clear();

			try
			{
				base.SuspendLayout();
				List<ListViewItem> itemsToAdd = new List<ListViewItem>();
				foreach(Node node in nodes)
				{
					Library lib = (Library)node.UserData;
					ListViewItem lvLib = new ListViewItem() { Tag = node, };
					String[] subItems = Array.ConvertAll(new String[Columns.Count], delegate (String a) { return String.Empty; });
					lvLib.SubItems.AddRange(subItems);
					lvLib.SubItems[colNodesName.Index].Text = lib.Name;
					lvLib.SubItems[colNodesAssemblyName.Index].Text = lib.Assembly?.ToString();
					lvLib.SubItems[colNodesPath.Index].Text = lib.Path;
					lvLib.SubItems[colNodesVersion.Index].Text = lib.Version?.ToString();
					lvLib.Checked = node.IsVisible;
					if(lib.IsSystem)
						lvLib.ForeColor = settings.LibrarySystemColor;
					else if(lib.Path == null)
						lvLib.ForeColor = settings.LibraryNotFoundColor;
					//this.Plugin.Settings.LibraryDefaultColor is not used. Because in can be too light
					itemsToAdd.Add(lvLib);
				}
				base.ItemChecked -= this.lvNodes_ItemChecked;
				base.Items.AddRange(itemsToAdd.ToArray());
				base.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				base.ItemChecked += this.lvNodes_ItemChecked;
			} finally
			{
				base.ResumeLayout();
			}
		}

		private void lvNodes_ItemChecked(Object sender, ItemCheckedEventArgs e)
		{
			Node node = (Node)e.Item.Tag;
			node.IsVisible = e.Item.Checked;

			foreach(Edge edge in node.Edges)
				edge.IsVisible = node.IsVisible;
			OnNodeVisibilityChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}