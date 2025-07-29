using System;
using System.Collections.Generic;

namespace Plugin.DependencyAnalyzer
{
	/// <summary>Settings for dependency graph form.</summary>
	public class DocumentDependenciesSettings
	{
		/// <summary>Path to the graph file or to the binary</summary>
		public String GraphFilePath { get; set; }

		/// <summary>The list of removed nodes from the graph.</summary>
		public String[] RemovedNodes { get; set; }

		internal void AddNodeToRemoved(String labelText)
		{
			List<String> labels = this.RemovedNodes == null
				? new List<String>()
				: new List<String>(this.RemovedNodes);

			labels.Add(labelText);
			this.RemovedNodes = labels.ToArray();
		}
	}
}