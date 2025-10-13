using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Plugin.DependencyAnalyzer.Data;
using Plugin.DependencyAnalyzer.UI;

namespace Plugin.DependencyAnalyzer
{
	/// <summary>The settings for dependencies analyzer plugin</summary>
	public class PluginSettings : INotifyPropertyChanged
	{
		private static class Constants
		{
			public const String ConstantsRegex = "(^(http:|https:).*|.*(\\.exe|\\.dll|\\.xml|\\.config)$)";

			public static System.Drawing.Color LibrarySystemColor = System.Drawing.Color.Green;

			public static System.Drawing.Color LibraryNotFoundColor = System.Drawing.Color.Red;

			public static System.Drawing.Color LibraryColor = System.Drawing.Color.LightYellow;
		}
		//Analyzer
		private Byte _recursiveLevel = 5;
		private LibrarySearchType _searchType = LibrarySearchType.AssemblyRef;
		//Analyzer
		//References
		private String _constantsRegex = Constants.ConstantsRegex;
		//References
		//UI
		private Boolean _highlightReferencedMembers = false;
		private LayoutMethod _layoutMethod = LayoutMethod.UseSettingsOfTheGraph;
		private LayerDirection _layerDirection = LayerDirection.LR;
		private Double _mouseHitDistance = 0.05;
		private Double _looseOffsetForRouting = 0.25;
		private Double _arrowheadLength = 10;
		private Double _offsetForRelaxingInRouting = 0.6;
		private Double _paddingForEdgeRouting = 8;
		private Double _tightOffsetForRouting = 0.125;
		private Double _zoomF = 1;
		private Double _zoomWindowThreshold = 0.05;
		private Boolean _insertingEdge = false;
		private System.Drawing.Color _librarySystemColor = Constants.LibrarySystemColor;
		private System.Drawing.Color _libraryNotFoundColor = Constants.LibraryNotFoundColor;
		private System.Drawing.Color _libraryColor = Constants.LibraryColor;
		private Int32 _splitterHorizontalDistance;
		//UI

		//Graph
		private Double _minNodeWidth = 13.5;
		private Double _nodeSeparation = 9.0;
		private Boolean _optimizeLabelPositions = true;
		private Boolean _simpleStretch = true;
		private Double _minNodeSeparation=4.5;
		private Double _minNodeHeight = 9.0;
		private Double _minLayerSeparation = 0.36;
		private Double _minimalWidth = 0;
		private Double _minimalHeight = 0;
		private Double _margin = 10.0;
		private Double _layerSeparation = 36.0;
		private Int32 _border=0;
		private System.Drawing.Color _backgroundColor = System.Drawing.Color.White;
		private Double _aspectRatio = 0;
		//Graph

		/// <summary>How deep to analyze assembly references</summary>
		[Category("Analyzer")]
		[Description("How deep to analyze assembly references")]
		[DefaultValue((Byte)5)]
		public Byte RecursiveLevel
		{
			get => this._recursiveLevel;
			set => this.SetField(ref this._recursiveLevel,
				value > 10 || value < 1 ? (Byte)5 : value,
				nameof(this.RecursiveLevel));
		}

		/// <summary>The type of search fo referenced assemblies.</summary>
		[Category("Analyzer")]
		[Description("AssemblyRef - Assembly references\r\nGAC - Analyze referenced assemblies in GAC\r\nModuleRef - Analyze references to PE\r\nNativeSystem - Analyze PE references")]
		[DefaultValue(LibrarySearchType.AssemblyRef)]
		[Editor(typeof(ColumnEditorTyped<LibrarySearchType>), typeof(UITypeEditor))]
		public LibrarySearchType SearchType
		{
			get => this._searchType;
			set
			{
				if((value & LibrarySearchType.AssemblyRef) == LibrarySearchType.AssemblyRef
					|| (value & LibrarySearchType.ModuleRef) == LibrarySearchType.ModuleRef)
					this.SetField(ref this._searchType, value, nameof(this.SearchType));
			}
		}

		/// <summary>Show All members</summary>
		[Category("Analyzer")]
		[DefaultValue(false)]
		[DisplayName("Show All members")]
		[Description("Show all members in Members Analyzer window and highlight used")]
		public Boolean HighlightReferencedMembers
		{
			get => this._highlightReferencedMembers;
			set => this.SetField(ref this._highlightReferencedMembers, value, nameof(this.HighlightReferencedMembers));
		}

		/// <summary>Constants search pattern</summary>
		[Category("References")]
		[DefaultValue(Constants.ConstantsRegex)]
		[DisplayName("Constants search pattern")]
		[Description("Search exact constants strings in references dialog")]
		public String ConstantsRegex
		{
			get => this._constantsRegex;
			set
			{
				String check = value;
				if(String.IsNullOrWhiteSpace(check))
					check = null;
				else//Exception check
					Regex.Match(String.Empty, check);

				this.SetField(ref this._constantsRegex, check, nameof(this.ConstantsRegex));
			}
		}

		/// <summary>Exposes the kind of the layout that is used when the graph is laid out by the viewer</summary>
		[Category("UI")]
		[DefaultValue(LayoutMethod.UseSettingsOfTheGraph)]
		[Description("Exposes the kind of the layout that is used when the graph is laid out by the viewer")]
		public LayoutMethod LayoutMethod
		{
			get => this._layoutMethod;
			set => this.SetField(ref this._layoutMethod, value, nameof(this.LayoutMethod));
		}

		/// <summary>The mouse hit distance</summary>
		[Category("UI")]
		[DefaultValue(0.05)]
		public Double MouseHitDistance
		{
			get => this._mouseHitDistance;
			set => this.SetField(ref this._mouseHitDistance,
				value > 0 ? value : 0.05,
				nameof(this.MouseHitDistance));
		}

		/// <summary>The loose offset for routing</summary>
		[Category("UI")]
		[DefaultValue(0.25)]
		public Double LooseOffsetForRouting
		{
			get => this._looseOffsetForRouting;
			set => this.SetField(ref this._looseOffsetForRouting,
				value > 0 ? value : 0.25,
				nameof(this.LooseOffsetForRouting));
		}

		/// <summary>Length of arrowheads for newly inserted edges</summary>
		[Category("UI")]
		[DefaultValue(10d)]
		[Description("Length of arrowheads for newly inserted edges")]
		public Double ArrowheadLength
		{
			get => this._arrowheadLength;
			set => this.SetField(ref this._arrowheadLength,
				value > 0 ? value : 10,
				nameof(this.ArrowheadLength));
		}

		/// <summary>If is set to true then the mouse left click on a node and dragging the cursor to another node will create an edge and add it to the graph</summary>
		[Category("UI")]
		[Description("If is set to true then the mouse left click on a node and dragging the cursor to another node will create an edge and add it to the graph")]
		[DefaultValue(false)]
		public Boolean InsertingEdge
		{
			get => this._insertingEdge;
			set => this.SetField(ref this._insertingEdge, value, nameof(InsertingEdge));
		}

		/// <summary>The offset for relaxing in routing</summary>
		[Category("UI")]
		[DefaultValue(0.6)]
		public Double OffsetForRelaxingInRouting
		{
			get => this._offsetForRelaxingInRouting;
			set => this.SetField(ref this._offsetForRelaxingInRouting,
				value > 0 ? value : 0.6,
				nameof(OffsetForRelaxingInRouting));
		}

		/// <summary>Padding used to route a new inserted edge around the nodes</summary>
		[Category("UI")]
		[DefaultValue(8d)]
		[Description("Padding used to route a new inserted edge around the nodes")]
		public Double PaddingForEdgeRouting
		{
			get => this._paddingForEdgeRouting;
			set => this.SetField(ref this._paddingForEdgeRouting,
				value > 0 ? value : 8,
				nameof(this.PaddingForEdgeRouting));
		}

		/// <summary>The tight Offset for routing</summary>
		[Category("UI")]
		[DefaultValue(0.125)]
		public Double TightOffsetForRouting
		{
			get => this._tightOffsetForRouting;
			set => this.SetField(ref this._tightOffsetForRouting,
				value > 0 ? value : 0.125, nameof(this.TightOffsetForRouting));
		}

		/// <summary>Gets or sets the zoom factor</summary>
		[Category("UI")]
		[Description("Gets or sets the zoom factor")]
		[DefaultValue(1d)]
		public Double ZoomF
		{
			get => this._zoomF;
			set => this.SetField(ref this._zoomF,
				value > 0 ? value : 1,
				nameof(this.ZoomF));
		}

		/// <summary>If the minimal side of the zoom window is shorter than the threshold then zoom does not take place</summary>
		[Category("UI")]
		[Description("If the minimal side of the zoom window is shorter than the threshold then zoom does not take place")]
		[DefaultValue(0.05)]
		public Double ZoomWindowThreshold
		{
			get => this._zoomWindowThreshold;
			set => this.SetField(ref this._zoomWindowThreshold,
				value > 0 ? value : 0.05,
				nameof(this.ZoomWindowThreshold));
		}

		/// <summary>Color to draw reference to GAC or system library</summary>
		[Category("UI")]
		[Description("Color to draw reference to GAC or system library")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public System.Drawing.Color LibrarySystemColor
		{
			get => this._librarySystemColor;
			set => this.SetField(ref this._librarySystemColor,
				value == System.Drawing.Color.Empty ? Constants.LibrarySystemColor : value,
				nameof(this.LibrarySystemColor));
		}

		/// <summary>Color to draw reference to not found libraries</summary>
		[Category("UI")]
		[Description("Color to draw reference to not found libraries")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public System.Drawing.Color LibraryNotFoundColor
		{
			get => this._libraryNotFoundColor;
			set => this.SetField(ref this._libraryNotFoundColor,
				value == System.Drawing.Color.Empty ? Constants.LibraryNotFoundColor : value,
				nameof(this.LibraryNotFoundColor));
		}

		/// <summary>Color to draw found references</summary>
		[Category("UI")]
		[Description("Color to draw found references")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public System.Drawing.Color LibraryColor
		{
			get => this._libraryColor;
			set => this.SetField(ref this._libraryColor,
				value == System.Drawing.Color.Empty ? Constants.LibraryColor : value,
				nameof(this.LibraryColor));
		}

		/// <summary>Libraries list default height</summary>
		[Category("UI")]
		[DisplayName("Splitter distance")]
		[Description("Libraries list default height")]
		[DefaultValue(0)]
		//[Browsable(false)]
		public Int32 SplitterHorizontalDistance
		{
			get => this._splitterHorizontalDistance;
			set => this.SetField(ref this._splitterHorizontalDistance, value, nameof(this.SplitterHorizontalDistance));
		}

		/// <summary>Required aspect ratio of the graph bounding box</summary>
		[Category("Graph")]
		[DefaultValue(0)]
		[Description("Required aspect ratio of the graph bounding box")]
		public Double AspectRatio
		{
			get => this._aspectRatio;
			set => this.SetField(ref this._aspectRatio, value, nameof(this.AspectRatio));
		}

		/// <summary>Background color for drawing ,plus initial fill color</summary>
		[Category("Graph")]
		[DisplayName("Background color")]
		[Description("Background color for drawing ,plus initial fill color")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public System.Drawing.Color BackgroundColorUI
		{
			get => this._backgroundColor;
			set => this.SetField(ref this._backgroundColor,
				value == System.Drawing.Color.Empty ? System.Drawing.Color.White : value,
				nameof(this.BackgroundColor));
		}

		[Category("Graph")]
		[Browsable(false)]
		private Color BackgroundColor => Convert(this.BackgroundColorUI);

		/// <summary>Thickness of the graph border line</summary>
		[Category("Graph")]
		[DefaultValue(0)]
		[Description("Thickness of the graph border line")]
		public Int32 Border
		{
			get => this._border;
			set => this.SetField(ref this._border, value, nameof(this.Border));
		}

		/// <summary>Directs node layers</summary>
		[Category("Graph")]
		[DefaultValue(LayerDirection.LR)]
		[Description("Directs node layers")]
		public LayerDirection LayerDirection
		{
			get => this._layerDirection;
			set => this.SetField(ref this._layerDirection, value, nameof(this.LayerDirection));
		}

		/// <summary>Distance between two neighbor layers</summary>
		[Category("Graph")]
		[DefaultValue(36.0)]
		[Description("Distance between two neighbor layers")]
		public Double LayerSeparation
		{
			get => this._layerSeparation;
			set => this.SetField(ref this._layerSeparation, value, nameof(this.LayerSeparation));
		}

		/// <summary>Margins width</summary>
		[Category("Graph")]
		[DefaultValue(10.0)]
		[Description("Margins width")]
		public Double Margin
		{
			get => this._margin;
			set => this.SetField(ref this._margin, value, nameof(this.Margin));
		}

		/// <summary>The resulting layout should at least as high as this this value</summary>
		[Category("Graph")]
		[DefaultValue(0d)]
		[Description("The resulting layout should at least as high as this this value")]
		public Double MinimalHeight
		{
			get => this._minimalHeight;
			set => this.SetField(ref this._minimalHeight, value, nameof(this.MinimalHeight));
		}

		/// <summary>The resulting layout should be not more narrow than this value</summary>
		[Category("Graph")]
		[DefaultValue(0d)]
		[Description("The resulting layout should be not more narrow than this value")]
		public Double MinimalWidth
		{
			get => this._minimalWidth;
			set => this.SetField(ref this._minimalWidth, value, nameof(this.MinimalWidth));
		}

		/// <summary>Minimal layer separation</summary>
		[Category("Graph")]
		[DefaultValue(0.36)]
		[Description("Minimal layer separation")]
		public Double MinLayerSeparation
		{
			get => this._minLayerSeparation;
			set => this.SetField(ref this._minLayerSeparation, value, nameof(this.MinLayerSeparation));
		}

		/// <summary>Lower bound for the node height</summary>
		[Category("Graph")]
		[DefaultValue(9.0)]
		[Description("Lower bound for the node height")]
		public Double MinNodeHeight
		{
			get => this._minNodeHeight;
			set => this.SetField(ref this._minNodeHeight, value, nameof(this.MinNodeHeight));
		}

		/// <summary>Minimal node separation</summary>
		[Category("Graph")]
		[DefaultValue(4.5)]
		[Description("Minimal node separation")]
		public Double MinNodeSeparation
		{
			get => this._minNodeSeparation;
			set => this.SetField(ref this._minNodeSeparation, value, nameof(this.MinNodeSeparation));
		}

		/// <summary>Lower bound for the node width</summary>
		[Category("Graph")]
		[DefaultValue(13.5)]
		[Description("Lower bound for the node width")]
		public Double MinNodeWidth
		{
			get => this._minNodeWidth;
			set => this.SetField(ref this._minNodeWidth, value, nameof(this.MinNodeWidth));
		}

		/// <summary>Node separation</summary>
		[Category("Graph")]
		[DefaultValue(9.0)]
		[Description("Node separation")]
		public Double NodeSeparation
		{
			get => this._nodeSeparation;
			set => this.SetField(ref this._nodeSeparation, value, nameof(this.NodeSeparation));
		}

		/// <summary>If set to true then the label positions are optimized</summary>
		[Category("Graph")]
		[DefaultValue(true)]
		[Description("If set to true then the label positions are optimized")]
		public Boolean OptimizeLabelPositions
		{
			get => this._optimizeLabelPositions;
			set => this.SetField(ref this._optimizeLabelPositions, value, nameof(this.OptimizeLabelPositions));
		}

		/// <summary>Works together with AspectRatio. If is set to false then the aspect ratio algorithm kicks in</summary>
		[Category("Graph")]
		[DefaultValue(true)]
		[Description("Works together with AspectRatio. If is set to false then the aspect ratio algorithm kicks in")]
		public Boolean SimpleStretch
		{
			get => this._simpleStretch;
			set => this.SetField(ref this._simpleStretch, value, nameof(this.SimpleStretch));
		}

		internal void SetObjectData(Object obj)
		{
			PropertyInfo[] thisProps = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach(PropertyInfo p in thisProps)
				if(p.CanRead)
					foreach(PropertyInfo tp in props)
						if(tp.Name == p.Name && tp.CanWrite)
							if(tp.PropertyType == p.PropertyType)
								tp.SetValue(obj, p.GetValue(this, null), null);
		}

		internal static Color Convert(System.Drawing.Color color)
			=> new Color(color.R, color.G, color.B);

		/// <summary>The event to notify when property has changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}