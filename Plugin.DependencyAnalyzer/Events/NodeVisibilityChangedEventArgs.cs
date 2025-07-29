using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace Plugin.DependencyAnalyzer.Events
{
	/// <summary>The event args to notify main UI component that the visibility of node has been changed</summary>
	internal class NodeVisibilityChangedEventArgs : EventArgs
	{
		public enum ChangeType
		{
			/// <summary>The visibility of node is changed. Graph control should be invalidated.</summary>
			Visibility,
			/// <summary>The node should be removed from the graph with all edges and orphan nodes</summary>
			Remove,
		}

		/// <summary>The list of nodes that was changed.</summary>
		public IEnumerable<Node> Nodes { get; }

		public ChangeType Change { get; }

		public NodeVisibilityChangedEventArgs(IEnumerable<Node> nodes, ChangeType change)
		{
			this.Nodes = nodes;
			this.Change = change;
		}
	}
}