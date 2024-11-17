using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Plugin.DependencyAnalyzer.Data;
using Plugin.DependencyAnalyzer.Properties;
using Plugin.DependencyAnalyzer.UI.Graph;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer
{
	public partial class DocumentDependencies : UserControl, IPluginSettings<DocumentDependenciesSettings>
	{//https://github.com/Microsoft/automatic-graph-layout

		private const String Caption = "Dependencies";
		private Int32 _currentRecursiveLevel = 0;
		private HashSet<String> _uniqueEdges;

		private Object _selectedObject;
		private Object _selectedObjectAttr;
		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;
		private IWindow Window => (IWindow)base.Parent;

		private DocumentDependenciesSettings _settings;
		Object IPluginSettings.Settings => this.Settings;

		public DocumentDependenciesSettings Settings => this._settings ?? (this._settings = new DocumentDependenciesSettings());

		internal LibraryAnalyzer Analyzer
		{
			get
			{
				if(graphView.Graph == null || graphView.Graph.UserData == null)
					return null;

				LibraryAnalyzer result = graphView.Graph.UserData as LibraryAnalyzer;
				return result;
			}
		}

		public DocumentDependencies()
			=> InitializeComponent();

		protected override void OnCreateControl()
		{
			this.Plugin.Settings.PropertyChanged += Settings_PropertyChanged;
			this.Window.Caption = Caption;
			this.Window.SetTabPicture(Resources.iPuzzles);
			this.Window.Closed += new EventHandler(Window_Closed);
			splitVertical.Panel2Collapsed = true;
			splitHorizontal.Panel2Collapsed = true;

			graphView.CurrentLayoutMethod = this.Plugin.Settings.LayoutMethod;
			graphView.MouseHitDistance = this.Plugin.Settings.MouseHitDistance;
			graphView.LooseOffsetForRouting = this.Plugin.Settings.LooseOffsetForRouting;
			graphView.ArrowheadLength = this.Plugin.Settings.ArrowheadLength;
			graphView.InsertingEdge = this.Plugin.Settings.InsertingEdge;
			graphView.OffsetForRelaxingInRouting = this.Plugin.Settings.OffsetForRelaxingInRouting;
			graphView.PaddingForEdgeRouting = this.Plugin.Settings.PaddingForEdgeRouting;
			graphView.TightOffsetForRouting = this.Plugin.Settings.TightOffsetForRouting;
			graphView.ZoomF = this.Plugin.Settings.ZoomF;
			graphView.ZoomWindowThreshold = this.Plugin.Settings.ZoomWindowThreshold;

			foreach(LibrarySearchType searchType in Enum.GetValues(typeof(LibrarySearchType)))
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(searchType.ToString())
				{
					Checked = false,
					Tag = searchType,
					CheckOnClick = true,
				};
				menuItem.CheckedChanged += ddlSearchType_CheckedChanged;
				ddlSearchType.DropDownItems.Add(menuItem);
			}
			base.OnCreateControl();

			this.LoadFile();
		}

		private void Window_Closed(Object sender, EventArgs e)
		{
			this.Plugin.Settings.PropertyChanged -= Settings_PropertyChanged;
			this.Plugin.CallDependencyInfo(this, null);
		}

		private void Settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			if(graphView.Graph == null)
				return;

			switch(e.PropertyName)
			{
			case nameof(PluginSettings.RecursiveLevel):
			case nameof(PluginSettings.SearchType):
				this.LoadFile();//Заново загружаем файл для обновления параметров. В идеале, конечно, перерисовывать недостающие элементы, а не перерисовывать всё (Иначе может произойти сброс того, что было у пользователя на экране)
				break;
			default:
				this.Plugin.Settings.SetObjectData(graphView.Graph.Attr);//Reflected Attribute properties
				graphView.Refresh();
				break;
			}
		}

		private void graphView_MouseUp(Object sender, MouseEventArgs e)
		{
			if(graphView.Graph == null)
				return;

			Object obj = graphView.GetObjectAt(e.X, e.Y);
			DNode dNode = obj as DNode;
			DEdge dEdge = dNode != null ? null : obj as DEdge;
			DLabel dLabel = dEdge != null || dNode != null ? null : obj as DLabel;

			/*Node node = null;
			Edge edge = null;
			if(dNode != null)
				node = dNode.DrawingNode;
			else if(dEdge != null)
				edge = dEdge.DrawingEdge;
			else if(dLabel != null)
			{
				if(dLabel.Parent is DNode)
					node = ((DNode)dLabel.Parent).DrawingNode;
				else if(dLabel.Parent is DEdge)
					edge = ((DEdge)dLabel.Parent).DrawingEdge;
			}*/

			switch(e.Button)
			{
			case System.Windows.Forms.MouseButtons.Left:
				if(dNode != null)
					this.ShowInfoCtrl(dNode, false);
				else if(dEdge != null)
					this.ShowAnalyzeCtrl(dEdge, false);
				break;
			case System.Windows.Forms.MouseButtons.Right:
				if(dNode != null)
				{
					cmsGraphNode.Tag = dNode;
					cmsGraphNode.Show(graphView, new System.Drawing.Point(e.X, e.Y));
				} else if(dEdge != null)
				{
					cmsGraphEdge.Tag = dEdge;
					cmsGraphEdge.Show(graphView, new System.Drawing.Point(e.X, e.Y));
				}
				break;
			}
		}

		private void cmsGraphNode_Opening(Object sender, CancelEventArgs e)
		{
			DNode dNode = (DNode)cmsGraphNode.Tag;
			Library lib = (Library)dNode.DrawingNode.UserData;
			tsmiGraphDependencies.Enabled = lib.IsFound;
			tsmiGraphOpenLocation.Enabled = lib.IsFound;
			tsmiGraphNodeReferences.Enabled = lib.IsFound;
		}

		private void cmsGraphNode_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			DNode dNode = (DNode)cmsGraphNode.Tag;
			if(e.ClickedItem == tsmiGraphNodeInfo)
				this.ShowInfoCtrl(dNode, true);
			else if(e.ClickedItem == tsmiGraphNodeReferences)
				this.ShowReferencesCtrl(dNode, true);
			else if(e.ClickedItem == tsmiGraphDependencies)
			{
				Library lib = (Library)dNode.DrawingNode.UserData;
				this.Plugin.OpenDependenciesWindow(lib.Path);
			} else if(e.ClickedItem == tsmiGraphNodeRemove)
				dNode.DrawingNode.IsVisible = false;
			else if(e.ClickedItem == tsmiGraphOpenLocation)
			{
				Library lib = (Library)dNode.DrawingNode.UserData;
				Shell32.OpenFolderAndSelectItem(lib.Path);
			}
			cmsGraphNode.Tag = null;
		}

		private void cmsGraphEdge_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			DEdge dEdge = (DEdge)cmsGraphEdge.Tag;
			if(e.ClickedItem == tsmiGraphEdgeAnalyze)
				this.ShowAnalyzeCtrl(dEdge, true);
			cmsGraphEdge.Tag = null;
		}

		private void cmsListNodes_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(lvNodes.SelectedItems.Count > 0)
				if(e.ClickedItem == tsmiListNodes_Focus)
				{
					Node node = (Node)lvNodes.SelectedItems[0].Tag;
					graphView.ShowBBox(new Microsoft.Msagl.Core.Geometry.Rectangle()
					{
						Center = node.Pos,
						Width = graphView.Width,
						Height = graphView.Height
					});
				}
		}

		private void ShowInfoCtrl(DNode dNode, Boolean isShow)
		{
			Library lib = (Library)dNode.DrawingNode.UserData;
			Data.IDataObject data = new DataObjectSelected(lib);
			//this.SetInfoCtrl(data);
			this.Plugin.CallDependencyInfo(this, data, isShow);
		}

		private void ShowReferencesCtrl(DNode dNode,Boolean isShow)
		{
			Library lib = (Library)dNode.DrawingNode.UserData;
			Data.IDataObject data = new DataObjectReferences(lib);
			this.Plugin.CallDependencyInfo(this, data, isShow);
		}

		private void ShowAnalyzeCtrl(DEdge dEdge, Boolean isShow)
		{
			Library libChild = (Library)dEdge.Edge.SourceNode.UserData;
			Library libParent = (Library)dEdge.Edge.TargetNode.UserData;
			if(libChild.Path != null && libParent.Path != null)//TODO: Add confirmation dialog for choosing assembly manually
			{
				Data.IDataObject data = new DataObjectAnalyze(this.Analyzer, libParent, libChild);
				//this.SetInfoCtrl(data);
				this.Plugin.CallDependencyInfo(this, data, isShow);
			}
		}

		private void graphView_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
			case Keys.F12:
				bnGraphAsList_Click(sender, e);
				break;
			case Keys.O:
				if(e.Control && !e.Alt)
					tsbnOpen_Click(sender, e);
				break;
			case Keys.S:
				if(e.Control && !e.Alt)
					tsbnSave_Click(sender, e);
				break;
			case Keys.P:
				if(e.Control && !e.Alt)
					tsbnPrint_Click(sender, e);
				break;
			case Keys.C:
				if(e.Control && !e.Alt)
				{
					DataObjectSave obj = this.GetInfoCtrl<DataObjectSave>()
						?? new DataObjectSave(this, graphView);
					obj.SaveBitmatToClipboard();
				}
				break;
			default:
				if(e.Control && !e.Alt)
				{
					graphView.PanButtonPressed = true;
					this.SyncPanButton();
				}
				break;
			}
		}

		private void graphView_KeyUp(Object sender, KeyEventArgs e)
		{
			if(!e.Control)
			{
				graphView.PanButtonPressed = false;
				this.SyncPanButton();
			}
		}

		private void graphView_GraphChanged(Object sender, EventArgs e)
		{
			tsbnRedo.Enabled = graphView.CanRedo();
			tsbnUndo.Enabled = graphView.CanUndo();

			tsbnBackward.Enabled = graphView.BackwardEnabled;
			tsbnForward.Enabled = graphView.ForwardEnabled;

			tsbnSave.Enabled = graphView.Graph != null;
			if(bnGraphAsList.Checked)
				lvNodes.RenderAssembliesInList(this.Plugin.Settings, graphView.Graph.Nodes);
		}

		private void graphView_ObjectUnderMouseCursorChanged(Object sender, ObjectUnderMouseCursorChangedEventArgs e)
		{
			this._selectedObject = e.OldObject != null ? e.OldObject.DrawingObject : null;

			if(this._selectedObject != null)
			{
				if(this._selectedObject is Edge)
					(this._selectedObject as Edge).Attr = this._selectedObjectAttr as EdgeAttr;
				else if(this._selectedObject is Node)
					(this._selectedObject as Node).Attr = this._selectedObjectAttr as NodeAttr;

				this._selectedObject = null;
			}

			if(graphView.SelectedObject == null)
				graphView.SetToolTip(graphToolTip, String.Empty);
			else
			{
				this._selectedObject = graphView.SelectedObject;
				Edge edge = this._selectedObject as Edge;
				Node node = edge == null ? this._selectedObject as Node : null;
				List<String> tipText = new List<String>();
				if(edge != null && edge.IsVisible)
				{
					this._selectedObjectAttr = edge.Attr.Clone();
					edge.Attr.Color = Color.Magenta;
					Library libSource = (Library)edge.SourceNode.UserData;
					Library libTarget = (Library)edge.UserData;

					tipText.Add(libSource.Assembly == null ? libSource.Name : libSource.Assembly.FullName);
					tipText.Add(libTarget.Assembly == null ? libTarget.Name : libTarget.Assembly.FullName);

				} else if(node != null && node.IsVisible)
				{
					this._selectedObjectAttr = node.Attr.Clone();
					node.Attr.Color = Color.Magenta;
					Library lib = (Library)node.UserData;
					if(lib.Assembly == null)
						tipText.Add("Name: " + lib.Name);
					else
						tipText.Add("FullName: " + lib.Assembly.FullName);
					if(lib.Path != null)
						tipText.Add("Path: " + lib.Path);
					if(lib.Version != null)
						tipText.Add("Version: " + lib.Version);
					tipText.Add("System: " + lib.IsSystem);
					//graphView.Invalidate(e.NewObject);
				}

				if(tipText.Count > 0)
					graphView.SetToolTip(graphToolTip, String.Join(Environment.NewLine, tipText));
			}
			graphView.Invalidate();
		}

		private void LoadFile()
		{
			if(this.Settings.GraphFilePath == null || !File.Exists(this.Settings.GraphFilePath))
			{
				String filePath = PluginWindows.OpenGraphFilePath();
				if(filePath == null)
				{
					this.Plugin.CallDependencyInfo(this, null, EventType.Close);
					this.Window.Close();
					return;
				}
				this.Settings.GraphFilePath = filePath;
			}

			LibrarySearchType selectedSearch = this.Plugin.Settings.SearchType;
			foreach(ToolStripMenuItem item in ddlSearchType.DropDownItems)
			{
				LibrarySearchType type = (LibrarySearchType)item.Tag;
				item.Checked = (selectedSearch & type) == type;
			}

			base.Cursor = Cursors.WaitCursor;
			this.Window.Caption = String.Join(" - ", DocumentDependencies.Caption, "Loading...");
			try
			{
				Graph graph;
				LibraryAnalyzer analyzer;
				Stopwatch sw = new Stopwatch();
				sw.Start();
				switch(Path.GetExtension(this.Settings.GraphFilePath).ToLowerInvariant())
				{
				case ".msagl":
					graph = Graph.Read(this.Settings.GraphFilePath);
					analyzer = new LibraryAnalyzer(this.Plugin.Trace, (String)graph.UserData, selectedSearch);//Must be path to the original assembly
					break;
				//case ".dll":
				//case ".exe":
				default:
					graph = new Graph(this.Settings.GraphFilePath);
					analyzer = new LibraryAnalyzer(this.Plugin.Trace, this.Settings.GraphFilePath, selectedSearch);

					this._uniqueEdges = new HashSet<String>();
					this.RenderAssembliesInGraph(analyzer.StartLibrary, graph);
					if((this.Plugin.Settings.SearchType & LibrarySearchType.UnreferencedInFolder) == LibrarySearchType.UnreferencedInFolder)
						foreach(Library library in analyzer.FindUnreferencedLibraries())
							this.RenderAssembliesInGraph(library, graph);
					this._uniqueEdges = null;
					break;
				}
				sw.Stop();

				if(analyzer.StartLibrary.IsFound == false)
				{
					graph.UserData = null;
					this.Window.Caption = String.Join(" - ", DocumentDependencies.Caption, "Error");
					this.Plugin.Trace.TraceInformation("File {0} grapg NOT loaded in {1}. References: {2:N0}", this.Settings.GraphFilePath, sw.Elapsed, analyzer.KnownLibraries.Count);
					graphView.Graph = null;
				} else
				{
					graph.UserData = analyzer;
					if(analyzer.StartLibrary.Assembly != null)
					{
						this.Window.Caption = String.Join(" - ", DocumentDependencies.Caption, analyzer.StartLibrary.Assembly.FullName);
						this.Plugin.Trace.TraceInformation("Assembly {0} graph loaded in {1}. References: {2:N0}", analyzer.StartLibrary.Assembly.FullName, sw.Elapsed, analyzer.KnownLibraries.Count);
					} else
					{
						this.Window.Caption = String.Join(" - ", DocumentDependencies.Caption, analyzer.StartLibrary.ShowAsString());
						this.Plugin.Trace.TraceInformation("Excutable {0} graph loaded in {1}. References: {2:N0}", analyzer.StartLibrary.Name, sw.Elapsed, analyzer.KnownLibraries.Count);
					}

					this._selectedObject = this._selectedObjectAttr = null;
					this.Plugin.Settings.SetObjectData(graph.Attr);//Reflected Attribute properties
					graphView.Graph = graph;
				}

				// Toggle controls
				foreach(ToolStripItem item in tsMain.Items)
					item.Enabled = true;

				this.graphView_GraphChanged(this, EventArgs.Empty);
				// Toggle controls
			} catch(Exception exc)
			{
				this.Plugin.Trace.TraceData(TraceEventType.Error, 1, exc);
				this.Window.Caption = String.Join(" - ", DocumentDependencies.Caption, "Error");
			} finally
			{
				base.Cursor = Cursors.Default;
			}
		}

		private void ddlSearchType_CheckedChanged(Object sender, EventArgs e)
		{
			LibrarySearchType searchType = 0;
			foreach(ToolStripMenuItem item in ddlSearchType.DropDownItems)
				if(item.Checked)
					searchType = searchType | ((LibrarySearchType)item.Tag);

			if(this.Plugin.Settings.SearchType != searchType)
				this.Plugin.Settings.SearchType = searchType;
		}

		private void tsbnOpen_Click(Object sender, EventArgs e)
		{
			String filePath = PluginWindows.OpenGraphFilePath();
			if(filePath == null)
				return;

			this.Plugin.CallDependencyInfo(this, null);
			this.Settings.GraphFilePath = filePath;
			this.LoadFile();
		}

		internal void tsbnSave_Click(Object sender, EventArgs e)
		{
			if(tsbnSave.Checked)
				this.SetInfoCtrl(null);
			else
				this.SetInfoCtrl(new DataObjectSave(this, graphView));
		}

		private void bnGraphAsList_Click(Object sender, EventArgs e)
		{
			bnGraphAsList.Checked = !bnGraphAsList.Checked;
			if(bnGraphAsList.Checked)
			{
				lvNodes.RenderAssembliesInList(this.Plugin.Settings, graphView.Graph.Nodes);
				splitHorizontal.Panel2Collapsed = false;
			} else
			{
				lvNodes.Items.Clear();
				splitHorizontal.Panel2Collapsed = true;
			}
		}

		private void splitVertical_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitVertical.SplitterRectangle.Contains(e.Location))
				this.SetInfoCtrl(null);
		}

		private void splitHorizontal_MouseDoubleClick(Object sender,MouseEventArgs e)
		{
			if(splitHorizontal.SplitterRectangle.Contains(e.Location))
			{
				bnGraphAsList.Checked = false;
				splitHorizontal.Panel2Collapsed = true;
			}
		}

		private void tsbnPrint_Click(Object sender, EventArgs e)
			=> graphView.PrintButtonPressed();

		private void tsbnZoomIn_Click(Object sender, EventArgs e)
			=> graphView.ZoomInPressed();

		private void tsbnZoomOut_Click(Object sender, EventArgs e)
			=> graphView.ZoomOutPressed();

		private void SyncPanButton()
			=> graphView.LayoutEditingEnabled = !graphView.PanButtonPressed;

		private void tsbnUndo_Click(Object sender, EventArgs e)
			=> graphView.Undo();

		private void tsbnRedo_Click(Object sender, EventArgs e)
			=> graphView.Redo();

		private void tsbnBackward_Click(Object sender, EventArgs e)
			=> graphView.BackwardButtonPressed();

		private void tsbnForward_Click(Object sender, EventArgs e)
			=> graphView.ForwardButtonPressed();

		private void graphView_DragEnter(Object sender, DragEventArgs e)
			=> e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;

		private void graphView_DragDrop(Object sender, DragEventArgs e)
		{
			foreach(String filePath in (String[])e.Data.GetData(DataFormats.FileDrop))
			{
				this.Plugin.CallDependencyInfo(this, null);
				this.Settings.GraphFilePath = filePath;
				this.LoadFile();
				break;
			}
		}

		private void lvNodes_OnNodeCheckedChanged(Object sender, EventArgs e)
			=> graphView.Refresh();

		private void SetInfoCtrl(Data.IDataObject dataObject)
		{
			tsbnSave.Checked = false;
			if(dataObject is DataObjectSave save)
			{
				tsbnSave.Checked = true;
				this.EnsureInfoCtrl<SaveDataCtrl>().SelectedObject = save;
			} else if(dataObject is DataObjectSelected selected)
				this.EnsureInfoCtrl<SelectedPECtrl>().SelectedObject = selected;
			else if(dataObject is DataObjectAnalyze analyze)
				this.EnsureInfoCtrl<AnalyzeCtrl>().SelectedObject = analyze;
			else if(dataObject == null)
			{
				Control ctrlChk = splitVertical.Panel2.Controls.Count == 0
					? null
					: splitVertical.Panel2.Controls[0];
				if(ctrlChk != null)
				{
					splitVertical.Panel2.Controls.Clear();
					ctrlChk.Dispose();
				}
			} else throw new NotSupportedException();

			splitVertical.Panel2Collapsed = dataObject == null;
		}

		private T GetInfoCtrl<T>() where T : class, Data.IDataObject
		{
			UserControl ctrl = splitVertical.Panel2.Controls.Count == 0
				? null
				: (UserControl)splitVertical.Panel2.Controls[0];
			if(ctrl == null)
				return default;
			else if(ctrl is SaveDataCtrl save)
				return save.SelectedObject as T;
			else if(ctrl is SelectedPECtrl selected)
				return selected.SelectedObject as T;
			else if(ctrl is AnalyzeCtrl analyze)
				return analyze.SelectedObject as T;
			else throw new NotSupportedException();
		}

		private T EnsureInfoCtrl<T>() where T : UserControl, new()
		{
			UserControl ctrlChk = splitVertical.Panel2.Controls.Count == 0
				? null
				: (UserControl)splitVertical.Panel2.Controls[0];

			if(ctrlChk == null)
			{
				T ctrl = new T() { Dock = DockStyle.Fill, Margin = new Padding(4), };
				splitVertical.Panel2.Controls.Add(ctrl);
				return ctrl;
			} else if(ctrlChk is T exact)
				return exact;
			else
			{
				splitVertical.Panel2.Controls.Clear();
				ctrlChk.Dispose();
				T ctrl = new T() { Dock = DockStyle.Fill, Margin = new Padding(4), };
				splitVertical.Panel2.Controls.Add(ctrl);
				return ctrl;
			}
		}

		private void RenderAssembliesInGraph(Library library, Graph graph)
		{
			Node nLibrary = new Node(library.Name) { UserData = library };
			nLibrary.Attr.FillColor = Color.LightYellow;
			graph.AddNode(nLibrary);
				
			foreach(Library reference in library.Reference)
			{
				if(this._uniqueEdges.Add(library.Name + reference.Name))
				{
					Node nAssembly = graph.FindNode(reference.Name);
					if(nAssembly == null)
					{
						nAssembly = new Node(reference.Name) { UserData = reference };
						graph.AddNode(nAssembly);
						if(reference.IsSystem == false && reference.Path != null)
							nAssembly.Attr.FillColor = PluginSettings.Convert(this.Plugin.Settings.LibraryColor);
					} else
					{
						Library refOld = (Library)nAssembly.UserData;
						if(refOld.Path == null && reference.Path != null)
						{
							nAssembly.UserData = reference;
							nAssembly.Attr.FillColor = PluginSettings.Convert(this.Plugin.Settings.LibraryColor);
						}
					}

					//Edge edge = new Edge(nLibrary, nAssembly, ConnectionToGraph.Disconnected);
					//graph.AddPrecalculatedEdge(edge);
					Edge edge = graph.AddEdge(library.Name, reference.Name);
					edge.UserData = reference;
					if(reference.IsSystem)
						edge.Attr.Color = PluginSettings.Convert(this.Plugin.Settings.LibrarySystemColor);
					else if(reference.Path == null)
						edge.Attr.Color = PluginSettings.Convert(this.Plugin.Settings.LibraryNotFoundColor);
				}

				if(reference.Reference != null && this._currentRecursiveLevel < this.Plugin.Settings.RecursiveLevel)
				{
					this._currentRecursiveLevel++;
					this.RenderAssembliesInGraph(reference, graph);
					this._currentRecursiveLevel--;
				}
			}
		}
	}
}