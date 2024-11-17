using System;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer
{
	internal class EventArgsBase : EventArgs
	{
		public EventType Type { get; set; }
		public String GraphFilePath { get; set; }
		public IDataObject Data { get; set; }
	}
}