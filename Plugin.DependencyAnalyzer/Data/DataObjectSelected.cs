using System;

namespace Plugin.DependencyAnalyzer.Data
{
	internal class DataObjectSelected : IDataObject
	{
		String IDataObject.Name => this.Library.Name;

		public Library Library { get; }

		public DataObjectSelected(Library library)
			=> this.Library = library;
	}
}