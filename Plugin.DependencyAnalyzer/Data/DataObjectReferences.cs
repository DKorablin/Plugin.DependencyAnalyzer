using System;

namespace Plugin.DependencyAnalyzer.Data
{
	internal class DataObjectReferences : IDataObject
	{
		public String Name => this.Library.Name + " Version=" + this.Library.Version;

		public Library Library { get; private set; }

		public DataObjectReferences(Library lib)
			=> this.Library = lib;
	}
}