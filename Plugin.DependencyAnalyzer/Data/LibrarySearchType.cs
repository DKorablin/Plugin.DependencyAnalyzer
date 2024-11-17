using System;

namespace Plugin.DependencyAnalyzer.Data
{
	[Flags]
	public enum LibrarySearchType
	{
		/// <summary>Search assembly references</summary>
		AssemblyRef = 1 << 0,
		/// <summary>Search assembly references in Global Assembly Cache & shared folders</summary>
		Gac = 1 << 1,
		/// <summary>Search unmanaged modules loaded by <c>DllImport</c> attribute</summary>
		ModuleRef = 1 << 2,
		/// <summary>Search unmanaged modules in shared & system folders</summary>
		NativeSystem = 1 << 3,
		/// <summary>Search for unreferenced libraries & assemblies in current folder</summary>
		UnreferencedInFolder = 1 << 4,
	}
}