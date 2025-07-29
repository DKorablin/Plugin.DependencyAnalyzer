using System;

namespace Plugin.DependencyAnalyzer.Data
{
	/// <summary>The type of search fo referenced files.</summary>
	[Flags]
	public enum LibrarySearchType
	{
		/// <summary>Search assembly references</summary>
		AssemblyRef = 1 << 0,
		/// <summary>Search assembly references in Global Assembly Cache &amp; shared folders</summary>
		Gac = 1 << 1,
		/// <summary>Search unmanaged modules loaded by <c>DllImport</c> attribute</summary>
		ModuleRef = 1 << 2,
		/// <summary>Search unmanaged modules in shared &amp; system folders</summary>
		NativeSystem = 1 << 3,
		/// <summary>Search for unreferenced libraries &amp; assemblies in current folder</summary>
		UnreferencedInFolder = 1 << 4,
		/// <summary>Remove not found files from the graph</summary>
		RemoveNotFound = 1 << 5,
	}
}