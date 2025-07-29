using System;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	internal class LibrarySearchDropDownButton : ToolStripDropDownButton
	{
		public event EventHandler<EventArgs> OnSearchTypeCheckedChanged;

		public LibrarySearchType SearchType
		{
			get
			{
				LibrarySearchType result = 0;
				foreach(ToolStripMenuItem item in this.DropDownItems)
					if(item.Checked)
						result = result | ((LibrarySearchType)item.Tag);

				if(result == 0)
				{
					result = LibrarySearchType.AssemblyRef;
					this.SearchType = result;
				}
				return result;
			}
			set
			{
				foreach(ToolStripMenuItem item in this.DropDownItems)
				{
					LibrarySearchType type = (LibrarySearchType)item.Tag;
					item.CheckedChanged -= this.OnCheckedChanged;
					item.Checked = (value & type) == type;
					item.CheckedChanged += this.OnCheckedChanged;
				}
			}
		}

		public LibrarySearchDropDownButton()
		{
			this.Text = "Search Type";
			foreach(LibrarySearchType searchType in Enum.GetValues(typeof(LibrarySearchType)))
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(searchType.ToString())
				{
					Checked = false,
					Tag = searchType,
					CheckOnClick = true,
				};
				menuItem.CheckedChanged += this.OnCheckedChanged;
				this.DropDownItems.Add(menuItem);
			}
		}

		private void OnCheckedChanged(Object sender, EventArgs e)
			=> this.OnSearchTypeCheckedChanged?.Invoke(this, e);
	}
}