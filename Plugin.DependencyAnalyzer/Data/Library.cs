using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using AlphaOmega.Debug;
using AlphaOmega.Debug.NTDirectory.Resources;
using AlphaOmega.Debug.CorDirectory.Meta.Tables;
using System.ComponentModel;

namespace Plugin.DependencyAnalyzer.Data
{
	/// <summary>DTO of executable description</summary>
	[DebuggerDisplay("Name={" + nameof(Name) + "} Version={" + nameof(Version) + "}")]
	[Serializable]
	public class Library : ISerializable
	{
		[NonSerialized]
		private Library[] _reference;

		/// <summary>Path to executable file</summary>
		[Description("Path to executable file")]
		public String Path { get; private set; }

		/// <summary>Executable name</summary>
		[Description("Executable or file name")]
		public String Name { get; private set; }

		/// <summary>Describes an assembly's unique identity in full</summary>
		[Description("Describes an assembly's unique identity in full")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public AssemblyName Assembly { get; set; }

		/// <summary>Assembly/Executable version number</summary>
		[Description("Executable version")]
		public Version Version { get; private set; }

		/// <summary>GAC assembly or executable from system folder (last is not implemented)</summary>
		[DisplayName("System executable")]
		[Description("File located in System or in GAC folder")]
		public Boolean IsSystem { get; private set; }

		/// <summary>Executable file not found</summary>
		[DisplayName("File found")]
		[Description("File found in local folder or GAC folder or system folders")]
		public Boolean IsFound { get { return this.Path != null; } }

		/// <summary>Referenced executables</summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public Library[] Reference
		{
			get => this._reference;
			set => this._reference = value;
		}

		/// <summary>Load PE file for analyzing.</summary>
		/// <param name="filePath">The file path to file to load from.</param>
		/// <param name="isFromGac">PE file stored in shared location.</param>
		/// <returns>Loaded file or null if PE file not found</returns>
		public static Library Load(String filePath, Boolean isFromGac)
		{
			using(PEFile file = new PEFile(filePath, StreamLoader.FromFile(filePath)))
				return file.Header.IsValid
					? new Library(file, isFromGac)
					: null;
		}

		/// <summary>Create instance of non system executable description</summary>
		/// <param name="path">Path to executable</param>
		/// <param name="name">Name of executable</param>
		/// <param name="version">Executable version</param>
		public Library(String path, String name, Version version)
			: this(path, name, version, null, false)
		{ }

		/// <summary>Create instance iff library analyzer class with required parameters.</summary>
		/// <param name="file">The loaded PE file information.</param>
		/// <param name="isFromGac">File was loaded from shared location (All dependencies will be loaded from shared location)</param>
		/// <exception cref="ArgumentException"></exception>
		public Library(PEFile file, Boolean isFromGac)
		{
			if(!file.Header.IsValid)
				throw new ArgumentException("Invalid PE header");

			String path = file.Source;
			String name = System.IO.Path.GetFileName(path);
			Version version;
			AssemblyName assembly = null;

			if(file.ComDescriptor == null || file.ComDescriptor.IsEmpty
				|| file.ComDescriptor.MetaData.StreamTables.Assembly.Table.RowsCount == 0)//There are assemblies without Managed Code: C:\Windows\Microsoft.NET\assembly\GAC_32\System.EnterpriseServices\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.EnterpriseServices.Wrapper.dll
			{//Native assembly
				ResourceVersion resVersion = file.Resource.GetVersion();

				version = resVersion?.FileInfo?.FileVersion;
			} else
			{//Managed assembly
				AssemblyRow row = file.ComDescriptor.MetaData.StreamTables.Assembly[0];
				name = row.Name;
				version = row.Version;
				assembly = row.AssemblyName;
			}

			this.Path = path;
			this.Name = name;
			this.Version = version;
			this.Assembly = assembly;
			this.IsSystem = isFromGac;
		}

		/// <summary>Create instance of assembly description</summary>
		/// <param name="path">Path to assembly</param>
		/// <param name="name">Name of the assembly</param>
		/// <param name="version">Assembly version</param>
		/// <param name="assembly">AssemblyName</param>
		/// <param name="isSystem">Assembly stored in GAC or locally</param>
		public Library(String path, String name, Version version, AssemblyName assembly, Boolean isSystem)
		{
			this.Path = path;
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.Version = version;
			this.Assembly = assembly;
			this.IsSystem = isSystem;
		}

		/// <summary>Opens PE file reader instance</summary>
		/// <returns>Opened instance</returns>
		public PEFile OpenPE()
			=> new PEFile(this.Path, StreamLoader.FromFile(this.Path));

		/// <summary>Serialization constructor</summary>
		/// <param name="info">Serialization Information</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		private Library(SerializationInfo info, StreamingContext context)
		{
			foreach(SerializationEntry entry in info)
				switch(entry.Name)
				{
				case "P":
					this.Path = (String)entry.Value;
					break;
				case "N":
					this.Name = (String)entry.Value;
					break;
				case "AN":
					this.Assembly = new AssemblyName((String)entry.Value);
					break;
				case "V":
					this.Version = new Version((String)entry.Value);
					break;
				case "IsSystem":
					this.IsSystem = true;
					break;
				}
		}

		/// <summary>Write to a serialized stream the elements of a collection under a specific identifier</summary>
		/// <param name="info">Serialization Information</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("P", this.Path);
			info.AddValue("N", this.Name);
			if(this.Assembly != null)
				info.AddValue("AN", this.Assembly.ToString());

			if(this.Version != null)
				info.AddValue("V", this.Version.ToString());

			if(this.IsSystem)
				info.AddValue("IsSystem", 1);
		}

		/// <summary>Search references by executable by name and version</summary>
		/// <param name="name">Name of the executable</param>
		/// <param name="version">Executable version</param>
		/// <returns>Found executable in references or null</returns>
		public Library FindLibrary(String name, Version version)
		{
			if(this.Reference != null)
				foreach(Library lib in this.Reference)
					if(lib.Equals(name, version))
						return lib;
					else
					{
						Library result = lib.FindLibrary(name, version);
						if(result != null)
							return result;
					}
			return null;
		}

		/// <summary>Compare 2 types of PE file information objects</summary>
		/// <param name="asmName">The assembly information to compare with current PE files loaded inside current analyzer</param>
		/// <returns>Objects are contains the same PE file</returns>
		public Boolean Equals(AssemblyName asmName)
		{//TODO: Add PublicKeyToken && not necessary Version equality if PKT is equals
			if(this.Name == asmName.Name)
			{
				//This is added if we found assembly with the same name and different version in current folder
				//Example: Referenced assembly = Microsoft.Extensions.Configuration.Binder
				String publicKeyToken = this.Assembly?.GetPublicKeyTokenString();
				return publicKeyToken == asmName.GetPublicKeyTokenString()
					&& this.Version == asmName.Version;
			}
			return false;
		}

		/// <summary>Compare 2 types of PE file information objects</summary>
		/// <param name="name">The name of PE file</param>
		/// <param name="version">The version of PE file</param>
		/// <returns>Objects are equal</returns>
		public Boolean Equals(String name, Version version)
			=> this.Name == name && this.Version == version;

		/// <summary>Compare 2 type of PE analyzer files</summary>
		/// <param name="lib">Different instance of PE file analyzers.</param>
		/// <returns>Objects are equal</returns>
		public Boolean Equals(Library lib)
			=> this.Name == lib.Name && this.Version == lib.Version;

		/// <summary>Show visually string representation of the library (I don't want to override default ToString method)</summary>
		/// <returns>Show version of currently loaded PE file as string.</returns>
		public String ShowAsString()
			=> this.Name + " Version=" + this.Version;
	}
}