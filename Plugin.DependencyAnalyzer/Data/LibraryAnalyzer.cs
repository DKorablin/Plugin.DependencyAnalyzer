using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta.Tables;
using AlphaOmega.Debug.NTDirectory;
using AlphaOmega.Reflection;

namespace Plugin.DependencyAnalyzer.Data
{
	internal class LibraryAnalyzer
	{
		private readonly TraceSource _trace;
		private readonly String _localPath;
		private readonly LibrarySearchType _searchType;

		public Dictionary<String, Library> KnownLibraries { get; } = new Dictionary<String, Library>();

		public Library StartLibrary { get; private set; }

		public LibraryAnalyzer(TraceSource trace, String path, LibrarySearchType searchType)
			: this(trace, searchType)
		{
			_ = path ?? throw new ArgumentNullException(nameof(path));
			if(!File.Exists(path))
				throw new FileNotFoundException("Library not found", path);

			this._localPath = Path.GetDirectoryName(path);
			this.StartLibrary = this.ReadLibrary(path, false);
			if(this.StartLibrary == null)
				throw new FileLoadException("This file is unreadable", path);
		}

		public LibraryAnalyzer(TraceSource trace, Library parent, LibrarySearchType searchType)
			: this(trace, searchType)
		{
			_ = parent ?? throw new ArgumentNullException(nameof(parent));

			if(!parent.IsFound)
				throw new FileNotFoundException("Library not found", parent.Path);

			this.StartLibrary = parent;
			this._localPath = Path.GetDirectoryName(parent.Path);
		}

		private LibraryAnalyzer(TraceSource trace, LibrarySearchType searchType)
		{
			this._trace = trace;
			this._searchType = searchType;
		}

		public IEnumerable<Library> GetReferencedLibraries(Library parent)
		{
			//We have to ignore libraries without version because native libraries are added without version information (See: AddToLoaded)
			HashSet<Library> alreadyAdded = new HashSet<Library>();

			foreach(Library lib in this.KnownLibraries.Values)
				if(lib.Reference != null)
					foreach(Library reference in lib.Reference)
						if(reference.Name == parent.Name)//reference.Equals(parent)
							if(alreadyAdded.Add(lib))
								yield return lib;
		}

		/// <summary>Search for unreferenced assemblies in current folder</summary>
		/// <returns>Stream of unreferenced libraries</returns>
		public IEnumerable<Library> FindUnreferencedLibraries()
		{
			IEnumerable<String> probing = Directory.EnumerateFiles(this._localPath, "*", SearchOption.TopDirectoryOnly);
			foreach(String filePath in probing)
			{
				Library found = this.FindAssemblyInLoaded(filePath);
				if(found == null)
				{
					Library library = this.ReadLibrary(filePath, false);
					if(library != null)
					{
						if(String.Equals(library.Path, filePath, StringComparison.OrdinalIgnoreCase))
							yield return library;
						else
							this._trace.TraceEvent(TraceEventType.Warning, 10, "Double found. Original: {0} Double:  {1}", library.Path, filePath);
					}
					/*Library library = Library.Load(filePath, false);
					if(library != null)
					{
						library.Reference = new Library[] { };
						yield return library;
					}*/
				}
			}
		}

		public void FindAllLibrariesInCurrentFolder()
		{
			IEnumerable<String> probing = Directory.EnumerateFiles(this._localPath, "*", SearchOption.TopDirectoryOnly);
			foreach(String file in probing)
				this.ReadLibrary(file, false);
		}

		private Library ReadLibrary(String filePath, Boolean isSystem)
		{
			Library library = null;
			using(PEFile file = new PEFile(filePath, StreamLoader.FromFile(filePath)))
				if(file.Header.IsValid)
				{
					library = new Library(file, isSystem);
					Library loaded = this.FindAssemblyInLoaded(library.Name, library.Version);
					if(loaded == null)
					{
						this.AddToLoaded(library);
						if(file.ComDescriptor != null && !file.ComDescriptor.IsEmpty)
							library.Reference = this.ReadReferences(file);
						else if(file.Import.IsEmpty == false)//Native library
							library.Reference = this.ReadImportTable(file);
						else
							library.Reference = new Library[] { };
					} else
						library = loaded;//We found library in different location. Ex: C:\Folder\My.dll & C:\Folder\My_.dll
				}
			return library;
		}

		private Library[] ReadImportTable(PEFile file)
		{
			String rootFolder = Path.GetDirectoryName(file.Source);
			List<Library> result = new List<Library>();

			foreach(ImportModule importModule in file.Import)
			{
				Library lib = this.FindLibraryInPath2(rootFolder, importModule.ModuleName);
				if(lib != null)
					result.Add(lib);
			}

			return result.ToArray();
		}

		private Library[] ReadReferences(PEFile file)
		{
			var tables = file.ComDescriptor.MetaData.StreamTables;
			List<Library> result = new List<Library>((Int32)(tables.AssemblyRef.Table.RowsCount + tables.ModuleRef.Table.RowsCount));
			String rootFolder = Path.GetDirectoryName(file.Source);

			if((this._searchType & LibrarySearchType.AssemblyRef) == LibrarySearchType.AssemblyRef)
				foreach(AssemblyRefRow asmRef in tables.AssemblyRef)
				{//Search for assemblies added via References
					AssemblyName assembly = asmRef.AssemblyName;

					Library lib = this.FindAssemblyInLoaded(assembly.Name, assembly.Version)
						?? this.FindAssemblyInPath(rootFolder, assembly);

					if(lib == null)//Assembly not found. Adding empty reference
					{
						lib = new Library(null, asmRef.Name, asmRef.Version, assembly, false);
						this.AddToLoaded(lib);
					} else if(lib.IsSystem && (this._searchType & LibrarySearchType.Gac) != LibrarySearchType.Gac)
						continue;//Assembly from GAC, but such assemblies, according to the search condition - skip

					result.Add(lib);
				}

			if((this._searchType & LibrarySearchType.ModuleRef) == LibrarySearchType.ModuleRef)
				foreach(ModuleRefRow module in tables.ModuleRef)
				{//Search for libraries added via DllImportAttribute
					if(String.IsNullOrWhiteSpace(module.Name))
						continue;

					Library lib = this.FindLibraryInPath2(rootFolder, module.Name);
					if(lib != null)
						result.Add(lib);
				}

			return result.ToArray();
		}

		private Library FindAssemblyInLoaded(String name, Version version)
		{
			String key = version == null
				? name
				: String.Join(" ", new String[] { name, version.ToString(), });

			Library result = this.KnownLibraries.TryGetValue(key, out result) ? result : null;
			return result;
		}

		private Library FindAssemblyInLoaded(String path)
		{
			foreach(Library loaded in this.KnownLibraries.Values)
				if(loaded.Path == path)
					return loaded;
			return null;
		}

		private void AddToLoaded(Library library)
		{
			String key = library.Version == null
				? library.Name
				: String.Join(" ", new String[] { library.Name, library.Version.ToString(), });

			this.KnownLibraries.Add(key, library);

			//We're adding libraries without version, because native libraries are referenced only by name
			if(library.IsFound && library.Version != null && library.Assembly == null)
				this.KnownLibraries.Add(library.Name, library);
		}

		private Library FindAssemblyInPath(String rootFolder, AssemblyName assembly)
		{
			IEnumerable<String> probing = Directory.EnumerateFiles(rootFolder, assembly.Name + ".dll", SearchOption.AllDirectories);
			foreach(String filePath in probing)
			{
				Library refAssembly = Library.Load(filePath, false);
				if(refAssembly != null && refAssembly.Assembly != null)
				{
					if(refAssembly.Equals(assembly))
						return this.ReadLibrary(filePath, false);
				}
			}

			String path;
			try
			{
				if(assembly.CultureInfo != null && assembly.CultureInfo.LCID == CultureInfo.InvariantCulture.LCID)
					assembly.CultureInfo = null;//HACK: There is a cultureless assembly in GAC

				path = AssemblyCache.QueryAssemblyInfo(assembly.FullName);
			} catch(FileNotFoundException)
			{
				return null;
			} catch(FileLoadException)
			{
				return null;
			}

			return this.ReadLibrary(path, true);
		}

		private Library FindLibraryInPath2(String rootFolder, String moduleName)
		{
			moduleName = moduleName.ToLowerInvariant();
			Library lib = this.FindAssemblyInLoaded(moduleName, null)
				?? this.FindLibraryInPath(rootFolder, moduleName);

			if(lib == null)
			{
				if(Path.GetExtension(moduleName) == String.Empty)//If library without extension, then trying to add extension and search with it
					lib = this.FindLibraryInPath2(rootFolder, moduleName + ".dll");//Trying to add recursively and it will be added to loaded collection
				else if(!String.Equals(rootFolder, this._localPath, StringComparison.OrdinalIgnoreCase))
					lib = this.FindLibraryInPath2(this._localPath, moduleName);//System library can reference to library that stored locally. Ex:kernel32.dll -> api-ms-win-core-file-l1-2-0.dll; But location is near devenv.exe
				else
				{//Library not found. Adding empty reference
					lib = new Library(null, moduleName, null, null, false);
					this.AddToLoaded(lib);
				}
			} else if(lib.IsSystem && (this._searchType & LibrarySearchType.NativeSystem) != LibrarySearchType.NativeSystem)
				return null;//Libraries from %WINDIR%, but we skip such libraries because search terms
			return lib;
		}

		private Library FindLibraryInPath(String rootFolder, String fileName)
		{
			String path = Path.Combine(rootFolder, fileName);
			if(File.Exists(path))
			{
				Boolean isSystem = String.Equals(rootFolder, Environment.GetFolderPath(Environment.SpecialFolder.System), StringComparison.OrdinalIgnoreCase);
				return this.ReadLibrary(path, isSystem);
			} else
			{
				path = Shared.NativeWrapper.SearchPath(fileName);
				if(path == null)
					return null;
				else
				{
					Boolean isSystem = String.Equals(Path.GetDirectoryName(path), Environment.GetFolderPath(Environment.SpecialFolder.System), StringComparison.OrdinalIgnoreCase);
					return this.ReadLibrary(path, isSystem);
				}
			}
		}
	}
}