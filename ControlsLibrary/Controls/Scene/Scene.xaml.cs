using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Model;
using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.Logic.Algorithms.OverlapRemoval;
using GraphX.Logic.Models;
using QuickGraph;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Visuzlizes a graph and provides editing of a graph
    /// </summary>
    public partial class Scene : UserControl
    {
        private ToolbarViewModel toolBar;

        public ToolbarViewModel Toolbar
        {
            get => toolBar;
            set
            {
                toolBar = value;
                toolBar.SelectedToolChanged += ToolSelected;
            }
        }

        public Scene()
        {
            InitializeComponent();
            SetZoomControlProperties();
            SetGraphAreaProperties();
            editor = new EditorObjectManager(graphArea, zoomControl);
        }

        private void SetZoomControlProperties()
        {
            zoomControl.Zoom = 2;
            zoomControl.MinZoom = .5;
            zoomControl.MaxZoom = 50;
            zoomControl.ZoomSensitivity = 25;
            zoomControl.IsAnimationEnabled = false;
            ZoomControl.SetViewFinderVisibility(zoomControl, Visibility.Hidden);
            zoomControl.MouseDown += OnSceneMouseDown;
        }

        private void SetGraphAreaProperties()
        {
            var graphLogic = new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>()
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog
            };
            graphArea.LogicCore = graphLogic;
            graphLogic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Custom;
            graphLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            graphLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            graphLogic.EdgeCurvingEnabled = true;
            graphLogic.EnableParallelEdges = true;
            graphLogic.ParallelEdgeDistance = 50;
            graphArea.VertexSelected += OnSceneVertexSelected;
            graphArea.EdgeSelected += EdgeSelected;

           /* graphLogic.DefaultLayoutAlgorithmParams =
                graphLogic.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.LinLog);
            graphLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            graphLogic.DefaultOverlapRemovalAlgorithmParams =
                graphLogic.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)graphLogic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)graphLogic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            graphLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            graphLogic.AsyncAlgorithmCompute = false;
            graphLogic.EdgeCurvingEnabled = false;*/
        }

        public event EventHandler<NodeSelectedEventArgs> NodeSelected;

        private void EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && Toolbar.SelectedTool == SelectedTool.Delete)
            {
                graphArea.RemoveEdge(args.EdgeControl.Edge as EdgeViewModel, true);
                return;
            }
        }

        private VertexControl selectedVertex;
        private readonly EditorObjectManager editor;

        private void OnSceneMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Toolbar.SelectedTool == SelectedTool.Edit)
                {
                    var pos = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
                    pos.Offset(-22.5, -22.5);
                    var vc = CreateVertexControl(pos);
                    if (selectedVertex != null)
                    {
                        CreateEdgeControl(vc);
                    }
                }
                else if (Toolbar.SelectedTool == SelectedTool.Select)
                {
                    ClearSelectMode(true);
                }
            }
        }

        private void CreateEdgeControl(VertexControl vc)
        {
            if (selectedVertex == null)
            {
                editor.CreateVirtualEdge(vc, vc.GetPosition());
                selectedVertex = vc;
                HighlightBehaviour.SetHighlighted(selectedVertex, true);
                return;
            }

            var data = new EdgeViewModel((NodeViewModel)selectedVertex.Vertex, (NodeViewModel)vc.Vertex);
            var ec = new EdgeControl(selectedVertex, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);

            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private VertexControl CreateVertexControl(Point position)
        {
            var data = new NodeViewModel() { Name = "Vertex " + (graphArea.VertexList.Count + 1), IsFinal = false, IsInitial = false, IsExpanded = true };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            return vc;
        }

        private void ToolSelected(object sender, EventArgs e)
        {
            if (Toolbar.SelectedTool == SelectedTool.Delete)
            {
                zoomControl.Cursor = Cursors.Help;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (Toolbar.SelectedTool == SelectedTool.Edit)
            {
                zoomControl.Cursor = Cursors.Pen;
                ClearSelectMode();
                return;
            }
            if (Toolbar.SelectedTool == SelectedTool.Select)
            {
                zoomControl.Cursor = Cursors.Hand;
                ClearEditMode();
                graphArea.SetVerticesDrag(true, true);
                graphArea.SetEdgesDrag(true);
                return;
            }
        }

        private void ClearSelectMode(bool soft = false)
        {
            graphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    HighlightBehaviour.SetHighlighted(a, false);
                    DragBehaviour.SetIsTagged(a, false);
                });

            if (!soft)
            {
                graphArea.SetVerticesDrag(false);
            }
        }

        private void ClearEditMode()
        {
            if (selectedVertex != null)
            {
                HighlightBehaviour.SetHighlighted(selectedVertex, false);
            }
            editor.DestroyVirtualEdge();
            selectedVertex = null;
        }

        private NodeViewModel SelectNode(VertexControl vertexControl)
            => graphArea.VertexList.FirstOrDefault(x => x.Value == vertexControl).Key;

        private void OnSceneVertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                NodeSelected?.Invoke(this, new NodeSelectedEventArgs() { Node = SelectNode(args.VertexControl) });
                switch (Toolbar.SelectedTool)
                {
                    case SelectedTool.Edit:
                        CreateEdgeControl(args.VertexControl);
                        break;
                    case SelectedTool.Delete:
                        SafeRemoveVertex(args.VertexControl);
                        break;
                    default:
                        if (Toolbar.SelectedTool == SelectedTool.Select && args.Modifiers == ModifierKeys.Control)
                        {
                            SelectVertex(args.VertexControl);
                        }
                        break;
                }
            }

            if (args.MouseArgs.ClickCount == 2)
            {
                if (Toolbar.SelectedTool == SelectedTool.Select)
                {
                    SelectNode(args.VertexControl).IsExpanded = !SelectNode(args.VertexControl).IsExpanded;
                }
            }
        }

        private static void SelectVertex(DependencyObject vc)
        {
            if (DragBehaviour.GetIsTagged(vc))
            {
                HighlightBehaviour.SetHighlighted(vc, false);
                DragBehaviour.SetIsTagged(vc, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vc, true);
                DragBehaviour.SetIsTagged(vc, true);
            }
        }

        private void SafeRemoveVertex(VertexControl vc)
        {
            foreach (var edge in graphArea.LogicCore.Graph.Edges)
            {
                if (edge.IsSelfLoop && edge.Source == SelectNode(vc))
                {
                    graphArea.RemoveEdge(edge);
                }
            }
            graphArea.RemoveVertexAndEdges(vc.Vertex as NodeViewModel);
        }

        public void Dispose()
        {
            if (editor != null)
            {
                editor.Dispose();
            }
            if (graphArea != null)
            {
                graphArea.Dispose();
            }
        }
    }
}
