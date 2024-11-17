using System;

namespace Plugin.DependencyAnalyzer
{
	/// <summary>Идентификатор панели графа</summary>
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

		/// <summary>Путь к файлу графика или к бинарнику</summary>
		public String GraphFilePath { get; set; }

		/// <summary>Сборка, которая анализируется в данном окне</summary>
		public String AssemblyPath { get; set; }

		/// <summary>What control we need to show to the user</summary>
		public LibraryControlType ControlType { get; set; } = LibraryControlType.Analyze;

		public override Boolean Equals(Object obj)
		{
			return obj is PanelDependencySettings settings
				&& this.GraphFilePath == settings.GraphFilePath
				&& (settings.AssemblyPath == null || this.AssemblyPath == settings.AssemblyPath)
				&& this.ControlType == settings.ControlType;
		}

		public override Int32 GetHashCode()
			=> this.GraphFilePath.GetHashCode();
	}
}