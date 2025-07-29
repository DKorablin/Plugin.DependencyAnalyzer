using System;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer
{
	/// <summary>The settings for the form of list of PE file dependencies and functions</summary>
	public class PanelDependencySettings
	{
		/// <summary>What control we need to show to the user</summary>
		public enum LibraryControlType
		{
			/// <summary>Show assembly info</summary>
			Info,
			/// <summary>Analyze libraries for references</summary>
			Analyze,
			/// <summary>Show library imports</summary>
			References,
		}

		/// <summary>Path to the graph file or to the binary</summary>
		public String GraphFilePath { get; set; }

		/// <summary>The assembly that is analyzed in this window</summary>
		public String AssemblyPath { get; set; }

		/// <summary>What control we need to show to the user</summary>
		public LibraryControlType ControlType { get; set; } = LibraryControlType.Analyze;

		public PanelDependencySettings() { }

		internal PanelDependencySettings(IDataObject dataObject)
			=> this.ConsumeDataObject(dataObject);

		internal void ConsumeDataObject(IDataObject dataObject)
		{
			if(dataObject is DataObjectSelected selectedPe)
			{
				this.ControlType = PanelDependencySettings.LibraryControlType.Info;
				this.AssemblyPath = selectedPe.Library.Path;
			} else if(dataObject is DataObjectReferences references)
			{
				this.ControlType = PanelDependencySettings.LibraryControlType.References;
				this.AssemblyPath = references.Library.Path;
			} else if(dataObject is DataObjectAnalyze analyze)
			{
				this.ControlType = PanelDependencySettings.LibraryControlType.Analyze;
				this.AssemblyPath = analyze.Library.Path;
			}
		}

		/// <summary>Compare 2 settings files to understand is it the same for or new one.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if settings are the same.</returns>
		public override Boolean Equals(Object obj)
		{
			return obj is PanelDependencySettings settings
				&& this.GraphFilePath == settings.GraphFilePath
				&& (settings.AssemblyPath == null || this.AssemblyPath == settings.AssemblyPath)
				&& this.ControlType == settings.ControlType;
		}

		/// <summary>Gets the unique settings identifier.</summary>
		/// <returns>Graph file hash code.</returns>
		public override Int32 GetHashCode()
			=> this.GraphFilePath.GetHashCode();
	}
}