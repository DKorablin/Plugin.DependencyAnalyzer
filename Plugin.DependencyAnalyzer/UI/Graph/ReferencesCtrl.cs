using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta;
using Plugin.DependencyAnalyzer.Data;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer.UI.Graph
{
	internal partial class ReferencesCtrl : UserControl
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

		private DataObjectReferences _object;
		public DataObjectReferences SelectedObject
		{
			get => this._object;
			set
			{
				this._object = value;
				if(this._object != null)
					this.ShowReferences(this._object.Library);
			}
		}

		public ReferencesCtrl()
			=> InitializeComponent();

		private void ShowReferences(Library lib)
		{
			lvLibraries.Items.Clear();
			if(lib == null)
				return;

			using(PEFile file = lib.OpenPE())
				if(file.Header.IsValid)
				{
					Regex constRegex = this.Plugin.Settings.ConstantsRegex == null
						? null
						: new Regex(this.Plugin.Settings.ConstantsRegex, RegexOptions.Compiled);

					List<ListViewItem> itemsToAdd = new List<ListViewItem>();
					if(file.ComDescriptor?.IsEmpty == false)
					{
						var metaData = file.ComDescriptor.MetaData;
						StreamTables tables = metaData.StreamTables;
						foreach(var assembly in tables.AssemblyRef)
							itemsToAdd.Add(CreateItem("Assembly Reference", assembly.AssemblyName.FullName));

						foreach(var module in tables.ModuleRef)
							itemsToAdd.Add(CreateItem("Module Reference", module.Name));

						if(constRegex != null)
						{
							foreach(String str in metaData.StringHeap.Data)
								if(constRegex.IsMatch(str))
									itemsToAdd.Add(CreateItem(constRegex.ToString(), str));
							foreach(var str in metaData.USHeap.GetDataString())
								if(constRegex.IsMatch(str.Value))
									itemsToAdd.Add(CreateItem(constRegex.ToString(), str.Value));
							foreach(var constant in tables.Constant)
								if(constant.Type == Cor.ELEMENT_TYPE.STRING && constRegex.IsMatch((String)constant.ValueTyped))
									itemsToAdd.Add(CreateItem(constRegex.ToString(), (String)constant.ValueTyped));
						}
					}

					if(file.Import?.IsEmpty == false)
					{
						foreach(AlphaOmega.Debug.NTDirectory.ImportModule module in file.Import)
							itemsToAdd.Add(CreateItem("Module Reference", module.ModuleName));
					}
					if(constRegex != null && file.Resource?.IsEmpty == false)
					{
						foreach(var rt_strings in file.Resource.GetStrings())
							foreach(var str in rt_strings)
								if(constRegex.IsMatch(str.Value))
									itemsToAdd.Add(CreateItem(constRegex.ToString(), str.Value));
					}

					lvLibraries.Items.AddRange(itemsToAdd.ToArray());
					lvLibraries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				}
		}

		private ListViewGroup GetGroup(String groupName)
		{
			ListViewGroup result = lvLibraries.Groups[groupName]
				?? lvLibraries.Groups.Add(groupName, groupName);
			return result;
		}

		private ListViewItem CreateItem(String groupName, String name)
		{
			ListViewItem result = new ListViewItem()
			{
				Group = GetGroup(groupName),
			};

			String[] subItems = Array.ConvertAll<String, String>(new String[lvLibraries.Columns.Count], delegate (String a) { return String.Empty; });
			result.SubItems.AddRange(subItems);

			result.SubItems[colName.Index].Text = name;

			return result;
		}
	}
}
