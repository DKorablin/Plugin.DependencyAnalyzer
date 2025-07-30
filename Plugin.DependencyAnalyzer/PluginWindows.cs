using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.DependencyAnalyzer
{
	/// <summary>The main entry point for dependency analyzer plugin</summary>
	public class PluginWindows : IPlugin,IPluginSettings<PluginSettings>
	{
		private readonly IHostWindows _hostWindows;
		private TraceSource _trace;
		private Dictionary<String, DockState> _documentTypes;
		private IMenuItem _menuReferencesGraph;
		private IMenuItem _menuReferencesAnalyze;
		private PluginSettings _settings;

		internal event EventHandler<EventArgsBase> OnDependenciesChanged;

		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		private IMenuItem MenuWinApi { get; set; }

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(DocumentDependencies).ToString(),DockState.Document },
						{ typeof(PanelDependency).ToString(),DockState.DockRight },
					};
				return this._documentTypes;
			}
		}

		/// <summary>Settings for interaction from the plugin</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings();
					this._hostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Create instance of dependency analyzer plugin and specify host interface</summary>
		/// <param name="hostWindows">The host interface</param>
		/// <exception cref="ArgumentNullException"><paramref name="hostWindows"/> is required</exception>
		public PluginWindows(IHostWindows hostWindows)
			=> this._hostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuView = this._hostWindows.MainMenu.FindMenuItem("View");
			if(menuView == null)
			{
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'View' not found");
				return false;
			}

			this.MenuWinApi = menuView.FindMenuItem("Executables");
			if(this.MenuWinApi == null)
			{
				this.MenuWinApi = menuView.Create("Executables");
				this.MenuWinApi.Name = "View.Executable";
				menuView.Items.Add(this.MenuWinApi);
			}

			this._menuReferencesGraph = menuView.Create("References Graph");
			this._menuReferencesGraph.Name = "View.Executable.References.Graph";
			this._menuReferencesGraph.Click += (sender, e) => this.OpenDependenciesWindow(OpenGraphFilePath());

			this._menuReferencesAnalyze = menuView.Create("Reference Analyze");
			this._menuReferencesAnalyze.Name = "View.Executable.References.Analyze";
			this._menuReferencesAnalyze.Click += new EventHandler(this.menuReferencesAnalyze_Click);

			this.MenuWinApi.Items.Add(this._menuReferencesGraph);
			this.MenuWinApi.Items.Add(this._menuReferencesAnalyze);
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this._menuReferencesGraph != null)
				this._hostWindows.MainMenu.Items.Remove(this._menuReferencesGraph);
			if(this._menuReferencesAnalyze != null)
				this._hostWindows.MainMenu.Items.Remove(this._menuReferencesAnalyze);
			if(this.MenuWinApi != null && this.MenuWinApi.Items.Count == 0)
				this._hostWindows.MainMenu.Items.Remove(this.MenuWinApi);

			return true;
		}

		/// <summary>Dynamic public method for creating plugin control from plugin itself.</summary>
		/// <param name="typeName">The name of control to create.</param>
		/// <param name="args">Dynamic arguments for specific plugin control</param>
		/// <returns>Created plugin control with specific arguments.</returns>
		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		internal void CallDependencyInfo(DocumentDependencies document, Data.IDataObject data, Boolean isOpen = false)
			=> this.CallDependencyInfo(document, data, data == null ? EventType.Close : EventType.Info, isOpen);

		internal void CallDependencyInfo(DocumentDependencies document, Data.IDataObject data, EventType type, Boolean isOpen = false)
		{
			if(isOpen)
			{
				_ = System.Threading.Tasks.Task.Factory.StartNew(() =>
				{
					var settings = new PanelDependencySettings(data)
					{
						GraphFilePath = document.Settings.GraphFilePath,
					};
					this.CreateWindow<PanelDependency, PanelDependencySettings>(settings);
				});
			}
		}

		internal void CallDependenciesChanged(DocumentDependencies document, EventType eventType,Data.IDataObject data)
		{
			EventArgsBase args = new EventArgsBase()
			{
				GraphFilePath = document.Settings.GraphFilePath,
				Type = eventType,
				Data = data,
			};

			_ = System.Threading.Tasks.Task.Factory.StartNew(() => this.OnDependenciesChanged?.Invoke(document, args));
		}

		internal static String OpenGraphFilePath()
		{
			const String filter = "Executable files (*.dll;*.exe)|*.dll;*.exe|Graph file (*.msagl)|*.msagl|All files (*.*)|*.*";
			const String title = "Open executable file to draw dependency diagram";
			return OpenFilePath(filter, title);
		}

		internal static String OpenAnalyzeFilePath()
		{
			const String filter = "Executable files (*.dll;*.exe)|*.dll;*.exe|All files (*.*)|*.*";
			const String title = "Open executable file to analyze for (un)used assembly members";
			return OpenFilePath(filter, title);
		}

		private static String OpenFilePath(String filter, String title)
		{
			using(OpenFileDialog dlg = new OpenFileDialog() { CheckFileExists = true, DefaultExt = "dll", Filter = filter, Title = title, })
				return dlg.ShowDialog() == DialogResult.OK
					? dlg.FileName
					: null;
		}

		internal void OpenDependenciesWindow(String filePath)
		{
			if(filePath == null)
				return;

			this.CreateWindow<DocumentDependencies, DocumentDependenciesSettings>(new DocumentDependenciesSettings() { GraphFilePath = filePath, });
		}

		private IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this._hostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		private IWindow CreateWindow<T,A>(A args) where T : IPluginSettings<A>
		{
			String typeName = typeof(T).ToString();
			return this.CreateWindow(typeName, true, args);
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		#region Event Handlers
		private void menuReferencesAnalyze_Click(Object sender, EventArgs e)
		{
			String filePath = OpenAnalyzeFilePath();
			if(filePath == null)
				return;

			this.CreateWindow<PanelDependency, PanelDependencySettings>(new PanelDependencySettings()
			{
				AssemblyPath = filePath,
				ControlType = PanelDependencySettings.LibraryControlType.Analyze
			});
		}
		#endregion Event Handlers
	}
}