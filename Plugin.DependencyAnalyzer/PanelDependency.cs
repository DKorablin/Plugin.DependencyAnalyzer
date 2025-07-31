using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;
using Plugin.DependencyAnalyzer.Extensions;
using Plugin.DependencyAnalyzer.Properties;
using Plugin.DependencyAnalyzer.UI.Graph;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer
{
	internal partial class PanelDependency : UserControl, IPluginSettings<PanelDependencySettings>
	{
		private readonly Stopwatch _loadLibraryElapsed = new Stopwatch();

		private PanelDependencySettings _settings;

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		Object IPluginSettings.Settings => this.Settings;

		public PanelDependencySettings Settings => this._settings ?? (this._settings = new PanelDependencySettings());

		public PanelDependency()
			=> this.InitializeComponent();

		protected override void OnCreateControl()
		{
			this.Window.SetTabPicture(Resources.iPuzzle);
			this.Window.Closed += this.Window_Closed;
			this.Plugin.OnDependenciesChanged += this.Plugin_OnDependenciesChanged;
			base.OnCreateControl();

			//if(this.Settings.GraphFilePath != null)//Nope. It wil not be called. Because of undefined load queue
			//	return;//If it's a graph invocation then all details will be sent in the event. See: CallDependencyInfo

			this.CreateInfoCtrl();
		}

		private void Plugin_OnDependenciesChanged(Object sender, EventArgsBase e)
		{
			if(e.GraphFilePath == this.Settings.GraphFilePath)
			{
				switch(e.Type)
				{
				case EventType.Info:
					this.Settings.ConsumeDataObject(e.Data);
					this.ShowInfoCtrl(e.Data);
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
			=> this.Plugin.OnDependenciesChanged -= this.Plugin_OnDependenciesChanged;

		private Data.IDataObject LoadDataObjectFromLibrary(Library lib)
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

			return dataObject;
		}

		private void CreateInfoCtrl()
		{
			if(this.Settings.AssemblyPath == null || !System.IO.File.Exists(this.Settings.AssemblyPath))
			{
				String filePath = PluginWindows.OpenAnalyzeFilePath();
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
				this.Window.Caption = "Loading...";
				this.Cursor = Cursors.WaitCursor;
				this._loadLibraryElapsed.Restart();

				Task.Factory.StartNew(() => this.LoadDataObjectFromLibrary(lib))
					.ContinueWith(task => this.ShowInfoCtrl(task.Result),
						TaskContinuationOptions.OnlyOnRanToCompletion)
					.ContinueWith(task => this.Plugin.Trace.TraceData(TraceEventType.Error, 10, task.Exception.Flatten()),
						TaskContinuationOptions.OnlyOnFaulted)
					.ContinueWith(task =>
					{
						this._loadLibraryElapsed.Stop();

						this.InvokeWithCheck(() => this.Cursor = Cursors.Default);
						this.Plugin.Trace.TraceInformation("Library {0} from path {1} loaded in {2}.", this.Settings.ControlType, this.Settings.AssemblyPath, this._loadLibraryElapsed.Elapsed);
					});
			}
		}

		private void ShowInfoCtrl(Data.IDataObject dataObject)
		{
			if(this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(() => this.ShowInfoCtrl(dataObject)));
				return;
			}

			this.Window.Caption = dataObject.Name;

			if(dataObject is DataObjectSave save)
				this.EnsureInfoCtrl<SaveDataCtrl>().SelectedObject = save;
			else if(dataObject is DataObjectSelected selectedPe)
				this.EnsureInfoCtrl<SelectedPECtrl>().SelectedObject = selectedPe;
			else if(dataObject is DataObjectReferences references)
				this.EnsureInfoCtrl<ReferencesCtrl>().SelectedObject = references;
			else if(dataObject is DataObjectAnalyze analyze)
				this.EnsureInfoCtrl<AnalyzeCtrl>().SelectedObject = analyze;
			else if(dataObject == null)
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