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
using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.TypeAnalyzer;
using ControlsLibrary.Controls.Executor;
using System.ComponentModel;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.FileSerialization;
using GraphX.Common.Models;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Visuzlizes a graph and provides editing of a graph
    /// </summary>
    public partial class Scene : UserControl
    {
        private TypeAnalyzerViewModel typeAnalyzer;

        public TypeAnalyzerViewModel TypeAnalyzer
        {
            get => typeAnalyzer;
            set
            {
                typeAnalyzer = value;
                typeAnalyzer.Graph = graphArea.LogicCore.Graph;
                GraphEdited += typeAnalyzer.GraphEdited;
            }
        }

        private ToolbarViewModel toolBar;

        private ErrorReporterViewModel errorReporter;
        public ToolbarViewModel Toolbar
        {
            get => toolBar;
            set
            {
                toolBar = value;
                toolBar.SelectedToolChanged += ToolSelected;
            }
        }

        public ErrorReporterViewModel ErrorReporter
        {
            get => errorReporter;
            set
            {
                errorReporter = value;
                errorReporter.Graph = graphArea.LogicCore.Graph;
                GraphEdited += errorReporter.GraphEdited;
            }
        }

        private ExecutorViewModel executorViewModel;
        public ExecutorViewModel ExecutorViewModel
        {
            get => executorViewModel;
            set
            {
                executorViewModel = value;
                executorViewModel.Graph = graphArea.LogicCore.Graph;
                executorViewModel.PropertyChanged += UpdateActualStates;
            }
        }

        private TestPanelViewModel testPanel;

        public TestPanelViewModel TestPanel
        {
            get => testPanel;
            set
            {
                testPanel = value;
                testPanel.Graph = graphArea.LogicCore.Graph;
            }
        }

        private void UpdateActualStates(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualStates")
            {
                foreach (var node in graphArea.LogicCore.Graph.Vertices)
                {
                    node.IsActual = false;
                }
                foreach (var stateId in executorViewModel.ActualStates)
                {
                    var node = graphArea.LogicCore.Graph.Vertices.FirstOrDefault(v => v.ID == stateId);
                    if (node != null)
                    {
                        node.IsActual = true;
                    }
                }
            }
        }

        public void Save(string path)
        {
            var datas = graphArea.ExtractSerializationData();
            foreach (var data in datas)
            {
                if (data.Data.GetType() == typeof(NodeViewModel))
                {
                    data.HasLabel = false;
                }
            }
            FileServiceProviderWpf.SerializeDataToFile(path, datas);
        }

        public bool CanSave()
            => graphArea.VertexList.Count > 0;

        public void Open(string path)
        {
            var data = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(path);
            graphArea.RebuildFromSerializationData(data);
            graphArea.SetVerticesDrag(true, true);
            graphArea.UpdateAllEdges();
            GraphEdited?.Invoke(this, EventArgs.Empty);
        }

        public Scene()
        {
            InitializeComponent();
            SetZoomControlProperties();
            SetGraphAreaProperties();
            editor = new EditorObjectManager(graphArea, zoomControl);
        }

        public void OnSceneKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                toolBar.SelectedTool = SelectedTool.Delete;
            }
            if (e.Key == Key.S)
            {
                toolBar.SelectedTool = SelectedTool.Select;
            }
            if (e.Key == Key.E)
            {
                toolBar.SelectedTool = SelectedTool.Edit;
            }
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

            using var deletionCursorStream = Application.GetResourceStream(new Uri("pack://application:,,,/ControlsLibrary;component/Controls/Scene/Assets/deletionCursor.cur", UriKind.RelativeOrAbsolute)).Stream;
            deletionCursor = new Cursor(deletionCursorStream);
        }

        private void SetGraphAreaProperties()
        {
            var logic =
                new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
                {
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog,
                };

            graphArea.LogicCore = logic;

            logic.DefaultLayoutAlgorithmParams = logic.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.LinLog);
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams = logic.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            logic.AsyncAlgorithmCompute = false;
            logic.EdgeCurvingEnabled = false;

            graphArea.VertexSelected += OnSceneVertexSelected;
            graphArea.EdgeSelected += EdgeSelected;

            graphArea.VertexMouseUp += VertexDragged;
        }

        public event EventHandler<NodeSelectedEventArgs> NodeSelected;

        public event EventHandler<EventArgs> GraphEdited;

        private void UpdateEdgeRoutingPoints(NodeViewModel source, NodeViewModel target)
        {
            var parralelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == target && e.Target == source);
            if (parralelEdge != null)
            {
                var newEdge = new EdgeViewModel(parralelEdge.Source, parralelEdge.Target) { TransitionTokensString = parralelEdge.TransitionTokensString };
                var ec = new EdgeControl(graphArea.VertexList[parralelEdge.Source], graphArea.VertexList[parralelEdge.Target], newEdge);
                graphArea.RemoveEdge(parralelEdge, true);
                graphArea.InsertEdgeAndData(newEdge, ec, 0, true);
            }
        }

        private void EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && Toolbar.SelectedTool == SelectedTool.Delete)
            {
                var edgeViewModel = args.EdgeControl.Edge as EdgeViewModel;
                var source = edgeViewModel.Source;
                var target = edgeViewModel.Target;
                graphArea.RemoveEdge(edgeViewModel, true);
                UpdateEdgeRoutingPoints(source, target);
                GraphEdited?.Invoke(this, EventArgs.Empty);
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

        private void VertexDragged(object sender, VertexSelectedEventArgs args)
        {
            foreach (var edge in graphArea.EdgesList.Where(e => e.Value.Source == args.VertexControl || e.Value.Target == args.VertexControl))
            {
                AvoidParralelEdges(edge.Value);
            }
        }

        private void EdgePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TransitionTokens" || e.PropertyName == "IsEpsilon")
            {
                GraphEdited?.Invoke(this, EventArgs.Empty);
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
            
            // Doesn't create new edges with the same direction
            // TODO: should somehow notice user that edge wasn't created
            if (graphArea.LogicCore.Graph.Edges.Any(e => e.Source == (NodeViewModel)selectedVertex.Vertex && e.Target == (NodeViewModel)vc.Vertex))
            {
                HighlightBehaviour.SetHighlighted(selectedVertex, false);
                selectedVertex = null;
                editor.DestroyVirtualEdge();
                return;
            }
            data.PropertyChanged += EdgePropertyChanged;
            var ec = new EdgeControl(selectedVertex, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);

            AvoidParralelEdges(ec);

            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private void AvoidParralelEdges(EdgeControl edgeControl)
        {
            var edge = edgeControl.Edge as EdgeViewModel;
            var parralelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == edge.Target && edge.Source == e.Target);

            if (parralelEdge != null)
            {
                var sourcePos = edgeControl.Source.GetCenterPosition().ToGraphX();
                var targetPos = edgeControl.Target.GetCenterPosition().ToGraphX();

                var middleX = (sourcePos.X + targetPos.X) / 2;
                var middleY = (sourcePos.Y + targetPos.Y) / 2;

                var distance = Geometry.GetDistance(sourcePos, targetPos);
                var diagonal = Math.Min(Math.Max(distance / 25, 20), 80);

                var bypassPoint1 = new GraphX.Measure.Point(middleX, middleY);
                var bypassPoint2 = new GraphX.Measure.Point(middleX, middleY);

                if ((sourcePos.X - targetPos.X) * (sourcePos.Y - targetPos.Y) > 0)
                {
                    bypassPoint1.X -= diagonal;
                    bypassPoint1.Y += diagonal;
                    bypassPoint2.X += diagonal;
                    bypassPoint2.Y -= diagonal;
                }
                else
                {
                    bypassPoint1.X -= diagonal;
                    bypassPoint1.Y -= diagonal;
                    bypassPoint2.X += diagonal;
                    bypassPoint2.Y += diagonal;
                }
                new GraphX.Measure.Point(middleX - diagonal, middleY);

                edge.RoutingPoints = new GraphX.Measure.Point[] { sourcePos, bypassPoint1, targetPos };
                parralelEdge.RoutingPoints = new GraphX.Measure.Point[] { targetPos, bypassPoint2, targetPos };
                graphArea.UpdateAllEdges();
            }
        }

        private int numberOfVertex = 0;

        private VertexControl CreateVertexControl(Point position)
        {
            var data = new NodeViewModel() { Name = "S" + numberOfVertex, IsFinal = false, IsInitial = false, IsExpanded = false };
            numberOfVertex++;
            var vc = new VertexControl(data);
            data.PropertyChanged += errorReporter.GraphEdited;
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            GraphEdited?.Invoke(this, EventArgs.Empty);
            return vc;
        }

        private static Cursor deletionCursor;

        private void ToolSelected(object sender, EventArgs e)
        {
            if (Toolbar.SelectedTool == SelectedTool.Delete)
            {
                zoomControl.Cursor = deletionCursor;
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
            GraphEdited?.Invoke(this, EventArgs.Empty);
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
