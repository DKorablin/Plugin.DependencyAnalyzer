using System;
using System.Collections.Generic;

namespace Plugin.DependencyAnalyzer.Data
{
	internal class DataObjectAnalyze : IDataObject
	{
		String IDataObject.Name => this.Library.Name + " Version=" + this.Library.Version;

		public Library Library { get; private set; }

		public Library[] ChildAssemblies { get; private set; }

		public DataObjectAnalyze(LibraryAnalyzer analyzer, Library parentAssembly, Library childAssembly = null)
		{
			this.Library = parentAssembly;

			List<Library> asm = new List<Library>(analyzer.KnownLibraries.Count);
			if(childAssembly != null)
				asm.Add(childAssembly);

			foreach(Library library in analyzer.GetReferencedLibraries(parentAssembly))
				if(library.Name != childAssembly?.Name)
					asm.Add(library);

			this.ChildAssemblies = asm.ToArray();
		}
	}
}