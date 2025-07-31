using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Plugin.DependencyAnalyzer.Data;
using Plugin.DependencyAnalyzer.Events;

namespace Plugin.DependencyAnalyzer.UI
{
	internal class ListViewLibraryNodes : SortableListView
	{
		private readonly ColumnHeader _colNodesName = new ColumnHeader() { Text = "Name", };
		private readonly ColumnHeader _colNodesPath = new ColumnHeader() { Text = "Path", };
		private readonly ColumnHeader _colNodesVersion = new ColumnHeader() { Text = "Version" };
		private readonly ColumnHeader _colNodesAssemblyName = new ColumnHeader() { Text = "Assembly Name" };
		private readonly Dictionary<Boolean, ListViewGroup> _groups = new Dictionary<Boolean, ListViewGroup>()
		{
			{ true, new ListViewGroup("Found", HorizontalAlignment.Left) { Header = "Found", } },
			{ false, new ListViewGroup("Not found", HorizontalAlignment.Left) { Header = "Not found", } }
		};

		public event EventHandler<NodeVisibilityChangedEventArgs> OnNodeVisibilityChanged;

		public Node SelectedGraphNode
		{
			get => this.SelectedItems.Count == 0
				? null
				: (Node)this.SelectedItems[0].Tag;
		}

		public ListViewLibraryNodes() {
			this.AllowColumnReorder = true;
			this.CheckBoxes = true;
			this.FullRowSelect = true;
			this.GridLines = true;
			this.View = View.Details;

			this.Columns.AddRange(new ColumnHeader[] {
				this._colNodesName,
				this._colNodesPath,
				this._colNodesVersion,
				this._colNodesAssemblyName});

			this.Groups.AddRange(this._groups.Values.ToArray());
		}

		/// <summary>Render assemblies from Graph as a list</summary>
		public void RenderAssembliesInList(PluginSettings settings, IEnumerable<Node> nodes)
		{
			base.Items.Clear();

			try
			{
				base.SuspendLayout();
				List<ListViewItem> itemsToAdd = new List<ListViewItem>();
				String[] subItems = Array.ConvertAll(new String[this.Columns.Count], a => String.Empty);
				foreach(Node node in nodes)
				{
					Library lib = (Library)node.UserData;
					ListViewItem lvLib = new ListViewItem() { Tag = node, };
					lvLib.SubItems.AddRange(subItems);
					lvLib.SubItems[this._colNodesName.Index].Text = lib.Name;
					lvLib.SubItems[this._colNodesAssemblyName.Index].Text = lib.Assembly?.ToString();
					lvLib.SubItems[this._colNodesPath.Index].Text = lib.Path;
					lvLib.SubItems[this._colNodesVersion.Index].Text = lib.Version?.ToString();
					lvLib.Group = this._groups[lib.Path != null];
					lvLib.Checked = node.IsVisible;
					if(lib.IsSystem)
						lvLib.ForeColor = settings.LibrarySystemColor;
					else if(lib.Path == null)
						lvLib.ForeColor = settings.LibraryNotFoundColor;
					//this.Plugin.Settings.LibraryDefaultColor is not used. Because in can be too light
					itemsToAdd.Add(lvLib);
				}
				base.ItemChecked -= this.OnItemChecked;
				base.Items.AddRange(itemsToAdd.ToArray());
				base.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				base.ItemChecked += this.OnItemChecked;
			} finally
			{
				base.ResumeLayout();
			}
		}

		public void RemoveAssemblyFromList(Node nodeToRemove)
		{
			foreach(ListViewItem item in this.Items)
				if(((Node)item.Tag) == nodeToRemove)
				{
					item.Remove();
					break;
				}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
			case Keys.Delete:
			case Keys.Back:
				List<Node> nodesToRemove = new List<Node>(this.SelectedItems.Count);
				foreach(ListViewItem listItem in this.SelectedItems)
					nodesToRemove.Add((Node)listItem.Tag);

				this.OnNodeVisibilityChanged?.Invoke(this, new NodeVisibilityChangedEventArgs(nodesToRemove, NodeVisibilityChangedEventArgs.ChangeType.Remove));
				break;
			}
			base.OnKeyDown(e);
		}

		private void OnItemChecked(Object sender, ItemCheckedEventArgs e)
		{
			Node node = (Node)e.Item.Tag;
			node.IsVisible = e.Item.Checked;

			// This may lead to exception on WM_PAIN while mouse cursor over the graph:
			// Object reference not set to an instance of an object.
			/*
				at Microsoft.Msagl.GraphViewerGdi.DEdge.UpdateRenderedBox()
				at Microsoft.Msagl.GraphViewerGdi.GViewer.ProcessOnPaint(Graphics g, PrintPageEventArgs printPageEvenArgs)
				at Microsoft.Msagl.GraphViewerGdi.DrawingPanel.OnPaint(PaintEventArgs e)
				at System.Windows.Forms.Control.PaintWithErrorHandling(PaintEventArgs e, Int16 layer)
				at System.Windows.Forms.Control.WmPaint(Message& m)
				at System.Windows.Forms.Control.WndProc(Message& m)
				at System.Windows.Forms.NativeWindow.Callback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)*/
			foreach(Edge edge in node.Edges)
				edge.IsVisible = node.IsVisible;
			this.OnNodeVisibilityChanged?.Invoke(this, new NodeVisibilityChangedEventArgs(new Node[] { node }, NodeVisibilityChangedEventArgs.ChangeType.Visibility));
		}
	}
}