using Plugin.DependencyAnalyzer.UI;

namespace Plugin.DependencyAnalyzer
{
	partial class DocumentDependencies
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator tsMainSeparator1;
			System.Windows.Forms.ToolStripSeparator tsMainSeparator2;
			System.Windows.Forms.ToolStripSeparator tsMainSeparator3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentDependencies));
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Found", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Not found", System.Windows.Forms.HorizontalAlignment.Left);
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsbnOpen = new System.Windows.Forms.ToolStripButton();
			this.tsbnPrint = new System.Windows.Forms.ToolStripButton();
			this.tsbnSave = new System.Windows.Forms.ToolStripButton();
			this.ddlSearchType = new System.Windows.Forms.ToolStripDropDownButton();
			this.bnGraphAsList = new System.Windows.Forms.ToolStripButton();
			this.tsbnZoomIn = new System.Windows.Forms.ToolStripButton();
			this.tsbnZoomOut = new System.Windows.Forms.ToolStripButton();
			this.tsbnBackward = new System.Windows.Forms.ToolStripButton();
			this.tsbnForward = new System.Windows.Forms.ToolStripButton();
			this.tsbnUndo = new System.Windows.Forms.ToolStripButton();
			this.tsbnRedo = new System.Windows.Forms.ToolStripButton();
			this.graphView = new Microsoft.Msagl.GraphViewerGdi.GViewer();
			this.cmsGraphNode = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiGraphNodeInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGraphNodeReferences = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGraphDependencies = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGraphOpenLocation = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiGraphNodeRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.cmsGraphEdge = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiGraphEdgeAnalyze = new System.Windows.Forms.ToolStripMenuItem();
			this.graphToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.splitVertical = new System.Windows.Forms.SplitContainer();
			this.splitHorizontal = new System.Windows.Forms.SplitContainer();
			this.lvNodes = new Plugin.DependencyAnalyzer.UI.ListViewLibraryNodes();
			this.cmsListNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiListNodes_Focus = new System.Windows.Forms.ToolStripMenuItem();
			tsMainSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			tsMainSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			tsMainSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsMain.SuspendLayout();
			this.cmsGraphNode.SuspendLayout();
			this.cmsGraphEdge.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitVertical)).BeginInit();
			this.splitVertical.Panel1.SuspendLayout();
			this.splitVertical.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitHorizontal)).BeginInit();
			this.splitHorizontal.Panel1.SuspendLayout();
			this.splitHorizontal.Panel2.SuspendLayout();
			this.splitHorizontal.SuspendLayout();
			this.cmsListNodes.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMainSeparator1
			// 
			tsMainSeparator1.Name = "tsMainSeparator1";
			tsMainSeparator1.Size = new System.Drawing.Size(6, 31);
			// 
			// tsMainSeparator2
			// 
			tsMainSeparator2.Name = "tsMainSeparator2";
			tsMainSeparator2.Size = new System.Drawing.Size(6, 31);
			// 
			// tsMainSeparator3
			// 
			tsMainSeparator3.Name = "tsMainSeparator3";
			tsMainSeparator3.Size = new System.Drawing.Size(6, 31);
			// 
			// tsMain
			// 
			this.tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnOpen,
            this.tsbnPrint,
            this.tsbnSave,
            this.ddlSearchType,
            this.bnGraphAsList,
            tsMainSeparator1,
            this.tsbnZoomIn,
            this.tsbnZoomOut,
            tsMainSeparator2,
            this.tsbnBackward,
            this.tsbnForward,
            tsMainSeparator3,
            this.tsbnUndo,
            this.tsbnRedo});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(433, 31);
			this.tsMain.TabIndex = 0;
			// 
			// tsbnOpen
			// 
			this.tsbnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnOpen.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconOpen;
			this.tsbnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnOpen.Name = "tsbnOpen";
			this.tsbnOpen.Size = new System.Drawing.Size(29, 28);
			this.tsbnOpen.Text = "&Open...";
			this.tsbnOpen.ToolTipText = "Open... (Ctrl+O)";
			this.tsbnOpen.Click += new System.EventHandler(this.tsbnOpen_Click);
			// 
			// tsbnPrint
			// 
			this.tsbnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnPrint.Enabled = false;
			this.tsbnPrint.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconPrint;
			this.tsbnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnPrint.Name = "tsbnPrint";
			this.tsbnPrint.Size = new System.Drawing.Size(29, 28);
			this.tsbnPrint.Text = "&Print";
			this.tsbnPrint.Click += new System.EventHandler(this.tsbnPrint_Click);
			// 
			// tsbnSave
			// 
			this.tsbnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnSave.Enabled = false;
			this.tsbnSave.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.FileSave;
			this.tsbnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnSave.Name = "tsbnSave";
			this.tsbnSave.Size = new System.Drawing.Size(29, 28);
			this.tsbnSave.Text = "&Save...";
			this.tsbnSave.ToolTipText = "Save... (Ctrl+S)";
			this.tsbnSave.Click += new System.EventHandler(this.tsbnSave_Click);
			// 
			// ddlSearchType
			// 
			this.ddlSearchType.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ddlSearchType.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.FilterList;
			this.ddlSearchType.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ddlSearchType.Name = "ddlSearchType";
			this.ddlSearchType.Size = new System.Drawing.Size(34, 28);
			this.ddlSearchType.Text = "Search Type";
			// 
			// bnGraphAsList
			// 
			this.bnGraphAsList.BackgroundImage = global::Plugin.DependencyAnalyzer.Properties.Resources.listView;
			this.bnGraphAsList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnGraphAsList.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.listView;
			this.bnGraphAsList.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnGraphAsList.Name = "bnGraphAsList";
			this.bnGraphAsList.Size = new System.Drawing.Size(29, 28);
			this.bnGraphAsList.Text = "Show list of all libraries in graph";
			this.bnGraphAsList.Click += new System.EventHandler(this.bnGraphAsList_Click);
			// 
			// tsbnZoomIn
			// 
			this.tsbnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnZoomIn.Enabled = false;
			this.tsbnZoomIn.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconZoomIn;
			this.tsbnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnZoomIn.Name = "tsbnZoomIn";
			this.tsbnZoomIn.Size = new System.Drawing.Size(29, 28);
			this.tsbnZoomIn.Text = "Zoom &In";
			this.tsbnZoomIn.Click += new System.EventHandler(this.tsbnZoomIn_Click);
			// 
			// tsbnZoomOut
			// 
			this.tsbnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnZoomOut.Enabled = false;
			this.tsbnZoomOut.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconZoomOut;
			this.tsbnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnZoomOut.Name = "tsbnZoomOut";
			this.tsbnZoomOut.Size = new System.Drawing.Size(29, 28);
			this.tsbnZoomOut.Text = "Zoom &Out";
			this.tsbnZoomOut.Click += new System.EventHandler(this.tsbnZoomOut_Click);
			// 
			// tsbnBackward
			// 
			this.tsbnBackward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnBackward.Enabled = false;
			this.tsbnBackward.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconBackward1;
			this.tsbnBackward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnBackward.Name = "tsbnBackward";
			this.tsbnBackward.Size = new System.Drawing.Size(29, 28);
			this.tsbnBackward.Text = "Backward";
			this.tsbnBackward.Click += new System.EventHandler(this.tsbnBackward_Click);
			// 
			// tsbnForward
			// 
			this.tsbnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnForward.Enabled = false;
			this.tsbnForward.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconForward1;
			this.tsbnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnForward.Name = "tsbnForward";
			this.tsbnForward.Size = new System.Drawing.Size(29, 28);
			this.tsbnForward.Text = "Forward";
			this.tsbnForward.Click += new System.EventHandler(this.tsbnForward_Click);
			// 
			// tsbnUndo
			// 
			this.tsbnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnUndo.Enabled = false;
			this.tsbnUndo.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconUndo;
			this.tsbnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnUndo.Name = "tsbnUndo";
			this.tsbnUndo.Size = new System.Drawing.Size(29, 28);
			this.tsbnUndo.Text = "&Undo";
			this.tsbnUndo.Click += new System.EventHandler(this.tsbnUndo_Click);
			// 
			// tsbnRedo
			// 
			this.tsbnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRedo.Enabled = false;
			this.tsbnRedo.Image = global::Plugin.DependencyAnalyzer.Properties.Resources.iconRedo;
			this.tsbnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRedo.Name = "tsbnRedo";
			this.tsbnRedo.Size = new System.Drawing.Size(29, 28);
			this.tsbnRedo.Text = "&Redo";
			this.tsbnRedo.Click += new System.EventHandler(this.tsbnRedo_Click);
			// 
			// graphView
			// 
			this.graphView.AllowDrop = true;
			this.graphView.ArrowheadLength = 10D;
			this.graphView.AsyncLayout = false;
			this.graphView.AutoScroll = true;
			this.graphView.BackwardEnabled = false;
			this.graphView.BuildHitTree = true;
			this.graphView.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
			this.graphView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphView.EdgeInsertButtonVisible = false;
			this.graphView.FileName = "";
			this.graphView.ForwardEnabled = false;
			this.graphView.Graph = null;
			this.graphView.IncrementalDraggingModeAlways = false;
			this.graphView.InsertingEdge = false;
			this.graphView.LayoutAlgorithmSettingsButtonVisible = false;
			this.graphView.LayoutEditingEnabled = true;
			this.graphView.Location = new System.Drawing.Point(0, 0);
			this.graphView.LooseOffsetForRouting = 0.25D;
			this.graphView.Margin = new System.Windows.Forms.Padding(4);
			this.graphView.MouseHitDistance = 0.05D;
			this.graphView.Name = "graphView";
			this.graphView.NavigationVisible = false;
			this.graphView.NeedToCalculateLayout = true;
			this.graphView.OffsetForRelaxingInRouting = 0.6D;
			this.graphView.PaddingForEdgeRouting = 8D;
			this.graphView.PanButtonPressed = false;
			this.graphView.SaveAsImageEnabled = true;
			this.graphView.SaveAsMsaglEnabled = true;
			this.graphView.SaveButtonVisible = false;
			this.graphView.SaveGraphButtonVisible = false;
			this.graphView.SaveInVectorFormatEnabled = true;
			this.graphView.Size = new System.Drawing.Size(185, 47);
			this.graphView.TabIndex = 1;
			this.graphView.TightOffsetForRouting = 0.125D;
			this.graphView.ToolBarIsVisible = false;
			this.graphView.Transform = ((Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)(resources.GetObject("graphView.Transform")));
			this.graphView.UndoRedoButtonsVisible = false;
			this.graphView.WindowZoomButtonPressed = false;
			this.graphView.ZoomF = 1D;
			this.graphView.ZoomWindowThreshold = 0.05D;
			this.graphView.GraphChanged += new System.EventHandler(this.graphView_GraphChanged);
			this.graphView.ObjectUnderMouseCursorChanged += new System.EventHandler<Microsoft.Msagl.Drawing.ObjectUnderMouseCursorChangedEventArgs>(this.graphView_ObjectUnderMouseCursorChanged);
			this.graphView.DragDrop += new System.Windows.Forms.DragEventHandler(this.graphView_DragDrop);
			this.graphView.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphView_DragEnter);
			this.graphView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.graphView_KeyDown);
			this.graphView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.graphView_KeyUp);
			this.graphView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphView_MouseUp);
			// 
			// cmsGraphNode
			// 
			this.cmsGraphNode.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsGraphNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGraphNodeInfo,
            this.tsmiGraphNodeReferences,
            this.tsmiGraphDependencies,
            this.tsmiGraphOpenLocation,
            this.toolStripSeparator1,
            this.tsmiGraphNodeRemove});
			this.cmsGraphNode.Name = "cmsGraphNode";
			this.cmsGraphNode.Size = new System.Drawing.Size(198, 130);
			this.cmsGraphNode.Opening += new System.ComponentModel.CancelEventHandler(this.cmsGraphNode_Opening);
			this.cmsGraphNode.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsGraphNode_ItemClicked);
			// 
			// tsmiGraphNodeInfo
			// 
			this.tsmiGraphNodeInfo.Name = "tsmiGraphNodeInfo";
			this.tsmiGraphNodeInfo.Size = new System.Drawing.Size(197, 24);
			this.tsmiGraphNodeInfo.Text = "Node &info";
			// 
			// tsmiGraphNodeReferences
			// 
			this.tsmiGraphNodeReferences.Name = "tsmiGraphNodeReferences";
			this.tsmiGraphNodeReferences.Size = new System.Drawing.Size(197, 24);
			this.tsmiGraphNodeReferences.Text = "&References";
			// 
			// tsmiGraphDependencies
			// 
			this.tsmiGraphDependencies.Name = "tsmiGraphDependencies";
			this.tsmiGraphDependencies.Size = new System.Drawing.Size(197, 24);
			this.tsmiGraphDependencies.Text = "&Dependencies";
			// 
			// tsmiGraphOpenLocation
			// 
			this.tsmiGraphOpenLocation.Name = "tsmiGraphOpenLocation";
			this.tsmiGraphOpenLocation.Size = new System.Drawing.Size(197, 24);
			this.tsmiGraphOpenLocation.Text = "Show in &Folder";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
			// 
			// tsmiGraphNodeRemove
			// 
			this.tsmiGraphNodeRemove.Name = "tsmiGraphNodeRemove";
			this.tsmiGraphNodeRemove.Size = new System.Drawing.Size(197, 24);
			this.tsmiGraphNodeRemove.Text = "Remo&ve node";
			// 
			// cmsGraphEdge
			// 
			this.cmsGraphEdge.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsGraphEdge.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGraphEdgeAnalyze});
			this.cmsGraphEdge.Name = "cmsGraphEdge";
			this.cmsGraphEdge.Size = new System.Drawing.Size(131, 28);
			this.cmsGraphEdge.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsGraphEdge_ItemClicked);
			// 
			// tsmiGraphEdgeAnalyze
			// 
			this.tsmiGraphEdgeAnalyze.Name = "tsmiGraphEdgeAnalyze";
			this.tsmiGraphEdgeAnalyze.Size = new System.Drawing.Size(130, 24);
			this.tsmiGraphEdgeAnalyze.Text = "&Analyze";
			// 
			// splitVertical
			// 
			this.splitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitVertical.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitVertical.Location = new System.Drawing.Point(0, 0);
			this.splitVertical.Margin = new System.Windows.Forms.Padding(4);
			this.splitVertical.Name = "splitVertical";
			// 
			// splitVertical.Panel1
			// 
			this.splitVertical.Panel1.Controls.Add(this.graphView);
			this.splitVertical.Size = new System.Drawing.Size(433, 47);
			this.splitVertical.SplitterDistance = 185;
			this.splitVertical.SplitterWidth = 5;
			this.splitVertical.TabIndex = 2;
			this.splitVertical.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitVertical_MouseDoubleClick);
			// 
			// splitHorizontal
			// 
			this.splitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitHorizontal.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitHorizontal.Location = new System.Drawing.Point(0, 31);
			this.splitHorizontal.Margin = new System.Windows.Forms.Padding(4);
			this.splitHorizontal.Name = "splitHorizontal";
			this.splitHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitHorizontal.Panel1
			// 
			this.splitHorizontal.Panel1.Controls.Add(this.splitVertical);
			// 
			// splitHorizontal.Panel2
			// 
			this.splitHorizontal.Panel2.Controls.Add(this.lvNodes);
			this.splitHorizontal.Size = new System.Drawing.Size(433, 162);
			this.splitHorizontal.SplitterDistance = 47;
			this.splitHorizontal.SplitterWidth = 5;
			this.splitHorizontal.TabIndex = 2;
			this.splitHorizontal.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitHorizontal_MouseDoubleClick);
			// 
			// lvNodes
			// 
			this.lvNodes.AllowColumnReorder = true;
			this.lvNodes.CheckBoxes = true;
			this.lvNodes.ContextMenuStrip = this.cmsListNodes;
			this.lvNodes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvNodes.FullRowSelect = true;
			this.lvNodes.GridLines = true;
			listViewGroup1.Header = "Found";
			listViewGroup1.Name = null;
			listViewGroup2.Header = "Not found";
			listViewGroup2.Name = null;
			this.lvNodes.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
			this.lvNodes.HideSelection = false;
			this.lvNodes.Location = new System.Drawing.Point(0, 0);
			this.lvNodes.Name = "lvNodes";
			this.lvNodes.Size = new System.Drawing.Size(433, 110);
			this.lvNodes.TabIndex = 0;
			this.lvNodes.UseCompatibleStateImageBehavior = false;
			this.lvNodes.View = System.Windows.Forms.View.Details;
			// 
			// cmsListNodes
			// 
			this.cmsListNodes.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsListNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiListNodes_Focus});
			this.cmsListNodes.Name = "cmsListNodes";
			this.cmsListNodes.Size = new System.Drawing.Size(116, 28);
			this.cmsListNodes.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsListNodes_ItemClicked);
			// 
			// tsmiListNodes_Focus
			// 
			this.tsmiListNodes_Focus.Name = "tsmiListNodes_Focus";
			this.tsmiListNodes_Focus.Size = new System.Drawing.Size(115, 24);
			this.tsmiListNodes_Focus.Text = "&Focus";
			// 
			// DocumentDependencies
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitHorizontal);
			this.Controls.Add(this.tsMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "DocumentDependencies";
			this.Size = new System.Drawing.Size(433, 193);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.cmsGraphNode.ResumeLayout(false);
			this.cmsGraphEdge.ResumeLayout(false);
			this.splitVertical.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitVertical)).EndInit();
			this.splitVertical.ResumeLayout(false);
			this.splitHorizontal.Panel1.ResumeLayout(false);
			this.splitHorizontal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitHorizontal)).EndInit();
			this.splitHorizontal.ResumeLayout(false);
			this.cmsListNodes.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void LvNodes_OnNodeCheckedChanged(object sender, System.Windows.Forms.ItemCheckedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripButton tsbnOpen;
		private Microsoft.Msagl.GraphViewerGdi.GViewer graphView;
		private System.Windows.Forms.ToolStripButton tsbnZoomIn;
		private System.Windows.Forms.ToolStripButton tsbnZoomOut;
		private System.Windows.Forms.ToolStripButton tsbnUndo;
		private System.Windows.Forms.ToolStripButton tsbnRedo;
		private System.Windows.Forms.ToolStripButton tsbnBackward;
		private System.Windows.Forms.ToolStripButton tsbnForward;
		private System.Windows.Forms.ToolStripButton tsbnSave;
		private System.Windows.Forms.ToolStripButton tsbnPrint;
		private System.Windows.Forms.ToolTip graphToolTip;
		private System.Windows.Forms.SplitContainer splitVertical;
		private System.Windows.Forms.SplitContainer splitHorizontal;
		private System.Windows.Forms.ContextMenuStrip cmsGraphNode;
		private System.Windows.Forms.ContextMenuStrip cmsGraphEdge;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphNodeRemove;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphNodeInfo;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphEdgeAnalyze;
		private System.Windows.Forms.ToolStripDropDownButton ddlSearchType;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphDependencies;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphOpenLocation;
		private System.Windows.Forms.ToolStripMenuItem tsmiGraphNodeReferences;
		private ListViewLibraryNodes lvNodes;
		private System.Windows.Forms.ToolStripButton bnGraphAsList;
		private System.Windows.Forms.ContextMenuStrip cmsListNodes;
		private System.Windows.Forms.ToolStripMenuItem tsmiListNodes_Focus;
	}
}
