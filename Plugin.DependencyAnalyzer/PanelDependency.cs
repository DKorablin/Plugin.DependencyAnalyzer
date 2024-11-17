using System;
using System.Diagnostics;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;
using Plugin.DependencyAnalyzer.Properties;
using Plugin.DependencyAnalyzer.UI.Graph;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer
{
	internal partial class PanelDependency : UserControl, IPluginSettings<PanelDependencySettings>
	{
		private PanelDependencySettings _settings;

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		Object IPluginSettings.Settings => this.Settings;

		public PanelDependencySettings Settings => this._settings ?? (this._settings = new PanelDependencySettings());

		public PanelDependency()
			=> InitializeComponent();

		protected override void OnCreateControl()
		{
			this.Window.SetTabPicture(Resources.iPuzzle);
			this.Window.Closed += Window_Closed;
			this.Plugin.OnDependenciesChanged += Plugin_OnDependenciesChanged;
			base.OnCreateControl();

			//if(this.Settings.GraphFilePath != null)//Nope. It wil not be called. Because of undefined load queue
			//	return;//If it's a graph invocation then all details will be sended in the event. See: CallDependencyInfo

			this.SetInfoCtrl();
		}

		private void Plugin_OnDependenciesChanged(Object sender, EventArgsBase e)
		{
			if(e.GraphFilePath == this.Settings.GraphFilePath)
			{
				switch(e.Type)
				{
				case EventType.Info:
					this.SetInfoCtrl(e.Data);
					break;
				case EventType.Close:
					this.Window.Close();
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		private void Window_Closed(Object sender, EventArgs e)
			=> this.Plugin.OnDependenciesChanged -= Plugin_OnDependenciesChanged;

		private void SetInfoCtrl()
		{
			if(this.Settings.AssemblyPath == null || !System.IO.File.Exists(this.Settings.AssemblyPath))
			{
				String filePath = PluginWindows.OpenAnalyseFilePath();
				if(filePath == null)
				{
					this.Window.Close();
					return;
				} else
					this.Settings.AssemblyPath = filePath;
			}

			Library lib = Library.Load(this.Settings.AssemblyPath, false);
			if(lib != null)
			{
				Data.IDataObject dataObject;
				switch(this.Settings.ControlType)
				{
				case PanelDependencySettings.LibraryControlType.Info:
					dataObject = new DataObjectSelected(lib);
					break;
				case PanelDependencySettings.LibraryControlType.References:
					dataObject = new DataObjectReferences(lib);
					break;
				case PanelDependencySettings.LibraryControlType.Analyze:
					LibraryAnalyzer analyzer;
					if(this.Settings.GraphFilePath != null)
						analyzer = new LibraryAnalyzer(this.Plugin.Trace, this.Settings.GraphFilePath, this.Plugin.Settings.SearchType);
					else
					{
						analyzer = new LibraryAnalyzer(this.Plugin.Trace, lib, this.Plugin.Settings.SearchType);
						analyzer.FindAllLibrariesInCurrentFolder();//This is used to cache all libraries in current folder
					}
					dataObject = new DataObjectAnalyze(analyzer, lib);
					break;
				default:
					throw new NotImplementedException();
				}

				this.SetInfoCtrl(dataObject);
			}
		}

		private void SetInfoCtrl(Data.IDataObject dataObject)
		{
			this.Window.Caption = dataObject.Name;

			Stopwatch sw = new Stopwatch();
			sw.Start();
			if(dataObject is DataObjectSave save)
				this.EnsureInfoCtrl<SaveDataCtrl>().SelectedObject = save;
			else if(dataObject is DataObjectSelected selectedPe)
			{
				this.Settings.ControlType = PanelDependencySettings.LibraryControlType.Info;
				this.Settings.AssemblyPath = selectedPe.Library.Path;
				this.EnsureInfoCtrl<SelectedPECtrl>().SelectedObject = selectedPe;
			}else if(dataObject is DataObjectReferences references)
			{
				this.Settings.ControlType = PanelDependencySettings.LibraryControlType.References;
				this.Settings.AssemblyPath = references.Library.Path;
				this.EnsureInfoCtrl<ReferencesCtrl>().SelectedObject = references;
			}
			else if(dataObject is DataObjectAnalyze analyze)
			{
				this.Settings.ControlType = PanelDependencySettings.LibraryControlType.Analyze;
				this.Settings.AssemblyPath = analyze.Library.Path;
				this.EnsureInfoCtrl<AnalyzeCtrl>().SelectedObject = analyze;
			} else if(dataObject == null)
			{
				Control ctrlChk = base.Controls.Count == 0
					? null
					: base.Controls[0];
				if(ctrlChk != null)
				{
					base.Controls.Clear();
					ctrlChk.Dispose();
				}
			} else throw new NotImplementedException();

			if(dataObject == null)
				this.Window.Close();
			else
			{
				sw.Stop();
				this.Plugin.Trace.TraceInformation("Library {0} from path {1} loaded in {2}.", this.Settings.ControlType, this.Settings.AssemblyPath, sw.Elapsed);
			}
		}

		private T GetInfoCtrl<T>() where T : class, Data.IDataObject
		{
			UserControl ctrl = base.Controls.Count == 0
				? null
				: (UserControl)base.Controls[0];

			if(ctrl == null)
				return default;
			else if(ctrl is SaveDataCtrl save)
				return save.SelectedObject as T;
			else if(ctrl is SelectedPECtrl selected)
				return selected.SelectedObject as T;
			else if(ctrl is AnalyzeCtrl analyze)
				return analyze.SelectedObject as T;
			else if(ctrl is ReferencesCtrl references)
				return references.SelectedObject as T;
			else throw new NotImplementedException();
		}

		private T EnsureInfoCtrl<T>() where T : UserControl, new()
		{
			UserControl ctrlChk = base.Controls.Count == 0
				? null
				: (UserControl)base.Controls[0];

			if(ctrlChk == null)
			{
				T ctrl = new T() { Dock = DockStyle.Fill, Margin = new Padding(4), };
				base.Controls.Add(ctrl);
				return ctrl;
			} else if(ctrlChk is T)
				return (T)ctrlChk;
			else
			{
				base.Controls.Clear();
				ctrlChk.Dispose();
				T ctrl = new T() { Dock = DockStyle.Fill, Margin = new Padding(4), };
				base.Controls.Add(ctrl);
				return ctrl;
			}
		}
	}
}