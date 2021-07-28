using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Controls.TypeAnalyzer;
using ControlsLibrary.FileSerialization;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using GraphX.Common.Enums;
using GraphX.Common.Models;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.Logic.Algorithms.OverlapRemoval;
using GraphX.Logic.Models;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ControlsLibrary.Properties.Langs;
using GraphX.Common;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Visualizes a graph and provides editing of a graph
    /// </summary>
    public partial class Scene
    {
        private TypeAnalyzerViewModel typeAnalyzer;

        /// <summary>
        /// Sets a new type analyzer view model on the scene
        /// </summary>
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

        /// <summary>
        /// Sets a new toolbar view model on the scene
        /// </summary>
        public ToolbarViewModel Toolbar
        {
            get => toolBar;
            set
            {
                toolBar = value;
                toolBar.SelectedToolChanged += ToolSelected;
            }
        }

        private ErrorReporterViewModel errorReporter;

        /// <summary>
        /// Sets a new error reporter view model on the scene
        /// </summary>
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

        /// <summary>
        /// Disables graph editing if executor in simulation
        /// </summary>
        private void InSimulationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ExecutorViewModel.InSimulation))
            {
                return;
            }

            ClearSelectMode(true);
            ClearEditMode();
            foreach (var node in graphArea.LogicCore.Graph.Vertices)
            {
                node.EditionAvailable = !ExecutorViewModel.InSimulation;
            }

            foreach (var edge in graphArea.LogicCore.Graph.Edges)
            {
                edge.EditionAvailable = !ExecutorViewModel.InSimulation;
            }
        }

        /// <summary>
        /// Updates isActual property values in nodes of the graph if executor state changed
        /// </summary>
        private void UpdateActualStates(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ExecutorViewModel.ActualStates))
            {
                return;
            }
            foreach (var node in graphArea.LogicCore.Graph.Vertices)
            {
                node.IsActual = false;
            }
            executorViewModel.ActualStates.ForEach(stateId =>
            {
                var node = graphArea.LogicCore.Graph.Vertices.FirstOrDefault(v => v.ID == stateId);
                if (node != null)
                {
                    node.IsActual = true;
                }
            });
        }

        private ExecutorViewModel executorViewModel;

        /// <summary>
        /// Sets a new executor view model
        /// </summary>
        public ExecutorViewModel ExecutorViewModel
        {
            get => executorViewModel;
            set
            {
                executorViewModel = value;
                executorViewModel.Executor = new FAExecutor(graphArea.LogicCore.Graph);
                executorViewModel.PropertyChanged += UpdateActualStates;
                executorViewModel.PropertyChanged += InSimulationChanged;
            }
        }

        private TestPanelViewModel testPanel;

        /// <summary>
        /// Sets a new test panel
        /// </summary>
        public TestPanelViewModel TestPanel
        {
            get => testPanel;
            set
            {
                testPanel = value;
                testPanel.Executor = new FAExecutor(graphArea.LogicCore.Graph);
            }
        }

        /// <summary>
        /// Handles changing of states properties values and invokes graph edited if property is important to FSA
        /// </summary>
        private void VertexEdited(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NodeViewModel.Name) || e.PropertyName == nameof(NodeViewModel.IsInitial) || e.PropertyName == nameof(NodeViewModel.IsFinal))
            {
                GraphEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles changing of transitions properties values and invokes graph edited if property is important to FSA
        /// </summary>
        private void EdgeEdited(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EdgeViewModel.TransitionTokens) || e.PropertyName == nameof(EdgeViewModel.IsEpsilon))
            {
                GraphEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Serializes and saves graph in xml format into the file in the given path
        /// </summary>
        /// <param name="path">Path of the file to save graph</param>
        public void Save(string path)
        {
            var datas = graphArea.ExtractSerializationData();
            datas.ForEach(data =>
            {
                if (data.Data.GetType() == typeof(NodeViewModel))
                {
                    data.HasLabel = false;
                }
            });
            FileServiceProviderWpf.SerializeDataToFile(path, datas);
        }

        /// <summary>
        /// Returns if graph can be saved or not
        /// </summary>
        /// <returns>True if there are more than 0 vertices</returns>
        public bool CanSave()
            => graphArea.VertexList.Count > 0;

        /// <summary>
        /// Deserializes graph data from the file on the given path
        /// </summary>
        /// <param name="path">Path of the given file</param>
        public void Open(string path)
        {
            var data = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(path);
            graphArea.RebuildFromSerializationData(data);
            graphArea.UpdateAllEdges();
            openedAutomatonsCount = 1;

            foreach (var node in graphArea.LogicCore.Graph.Vertices)
            {
                node.PropertyChanged += VertexEdited;
            }

            foreach (var edge in graphArea.LogicCore.Graph.Edges)
            {
                edge.PropertyChanged += EdgeEdited;
            }

            GraphEdited?.Invoke(this, EventArgs.Empty);
        }

        public void OpenOverAutomatonOnScene(string path)
        {
            var data = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(path);
            openedAutomatonsCount++;
            RebuildFromSerializationDataOverAutomaton(data);
        }

        private void RebuildFromSerializationDataOverAutomaton(IEnumerable<GraphSerializationData> data)
        {
            foreach (var element in data)
            {
                if (element.Data is NodeViewModel)
                {
                    var newNodeViewModel = element.Data as NodeViewModel;
                    newNodeViewModel.IsComponentObject = true;
                    newNodeViewModel.ID = Convert.ToInt64($"{newNodeViewModel.ID}{openedAutomatonsCount}");
                    var vc = CreateVertexControl(newNodeViewModel);
                    vc.SetPosition(element.Position.X, element.Position.Y);
                }

                if (element.Data is EdgeViewModel)
                {
                    var edgeViewModel = element.Data as EdgeViewModel;
                    edgeViewModel.Source.ID = Convert.ToInt64($"{edgeViewModel.Source.ID}{openedAutomatonsCount}");
                    edgeViewModel.Target.ID = Convert.ToInt64($"{edgeViewModel.Target.ID}{openedAutomatonsCount}");
                    var dataSource 
                        = graphArea.VertexList.Keys.FirstOrDefault(
                            a => a.ID == edgeViewModel.Source.ID && a.IsComponentObject);
                    var dataTarget 
                        = graphArea.VertexList.Keys.FirstOrDefault(
                            a => a.ID == edgeViewModel.Target.ID && a.IsComponentObject);
                    edgeViewModel.Source = dataSource;
                    edgeViewModel.Target = dataTarget;
                    CreateEdgeControl(edgeViewModel);
                }
            }
        }

        /// <summary>
        /// The basic constructor
        /// </summary>
        public Scene()
        {
            InitializeComponent();
            SetZoomControlProperties();
            SetGraphAreaProperties();
            editor = new EditorObjectManager(graphArea, zoomControl);
            openedAutomatonsCount = 0;
        }

        /// <summary>
        /// Handles application hot keys
        /// </summary>
        public void OnSceneKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                    {
                        toolBar.SelectedTool = SelectedTool.Delete;
                        return;
                    }
                case Key.S:
                    {
                        toolBar.SelectedTool = SelectedTool.Select;
                        return;
                    }
                case Key.E:
                    {
                        toolBar.SelectedTool = SelectedTool.Edit;
                        return;
                    }
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

            using var deletionCursorStream = Application.GetResourceStream(new Uri("pack://application:,,,/ControlsLibrary;component/Controls/Scene/Assets/deletionCursor.cur", UriKind.RelativeOrAbsolute))?.Stream;
            if (deletionCursorStream != null)
            {
                deletionCursor = new Cursor(deletionCursorStream);
            }
        }

        //TODO: learn how to extract drag information from the graphArea
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
            graphArea.SetEdgesDrag(false);
            graphArea.VertexMouseUp += VertexDragged;
        }

        /// <summary>
        /// Invokes if node on the graph was selected
        /// </summary>
        public event EventHandler<NodeSelectedEventArgs> NodeSelected;

        /// <summary>
        /// Invokes if FSA was edited
        /// </summary>
        public event EventHandler<EventArgs> GraphEdited;

        /// <summary>
        /// Update edge route if graph was edited
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        private void UpdateEdgeRoutingPoints(NodeViewModel source, NodeViewModel target)
        {
            var parallelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == target && e.Target == source);
            if (parallelEdge == null)
            {
                return;
            }

            var newEdge = new EdgeViewModel(parallelEdge.Source, parallelEdge.Target) { TransitionTokensString = parallelEdge.TransitionTokensString };
            var ec = new EdgeControl(graphArea.VertexList[parallelEdge.Source], graphArea.VertexList[parallelEdge.Target], newEdge);
            graphArea.RemoveEdge(parallelEdge, true);
            graphArea.InsertEdgeAndData(newEdge, ec, 0, true);
        }

        /// <summary>
        /// Handles edge selection and removes edge if scene in the deletion mode
        /// </summary>
        private void EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && Toolbar.SelectedTool == SelectedTool.Delete)
            {
                if (ExecutorViewModel.InSimulation)
                {
                    return;
                }
                var edgeViewModel = (EdgeViewModel)args.EdgeControl.Edge;

                if (edgeViewModel == null)
                {
                    return;
                }

                var source = edgeViewModel.Source;
                var target = edgeViewModel.Target;
                graphArea.RemoveEdge(edgeViewModel, true);
                UpdateEdgeRoutingPoints(source, target);
                GraphEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        private VertexControl selectedVertex;

        private readonly EditorObjectManager editor;

        private long openedAutomatonsCount;

        /// <summary>
        /// Creates new vertices by click on the scene and creates targeted edges if scene in the creating of the new edge state
        /// </summary>
        private void OnSceneMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Toolbar.SelectedTool == SelectedTool.Edit)
                {
                    if (ExecutorViewModel.InSimulation)
                    {
                        return;
                    }
                    var position = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);

                    position.Offset(-60, -60); //Offset should be the half of the vertex controls width
                    var vc = CreateVertexControl(position);
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

        /// <summary>
        /// Fixes edges routing if vertex was dragged
        /// </summary>
        private void VertexDragged(object sender, VertexSelectedEventArgs args)
        {
            foreach (var edge in graphArea.EdgesList.Where(e => e.Value.Source == args.VertexControl || e.Value.Target == args.VertexControl))
            {
                AvoidParallelEdges(edge.Value);
            }
        }

        private void CreateEdgeControl(VertexControl vc)
        {
            if (ExecutorViewModel.InSimulation)
            {
                return;
            }
            if (selectedVertex == null)
            {
                editor.CreateVirtualEdge(vc);
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

            var ec = CreateEdgeControl(data);

            AvoidParallelEdges(ec);

            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private EdgeControl CreateEdgeControl(EdgeViewModel data)
        {
            data.PropertyChanged += EdgeEdited;
            var ec = new EdgeControl(graphArea.VertexList[data.Source], graphArea.VertexList[data.Target], data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);
            GraphEdited?.Invoke(this, EventArgs.Empty);
            return ec;
        }

        /// <summary>
        /// Creates edge routing points to avoid overlapping of an edge by a parallel one
        /// </summary>
        /// <param name="edgeControl">Edge control which was overlapped or overlaps other</param>
        private void AvoidParallelEdges(EdgeControl edgeControl)
        {
            var edge = edgeControl.Edge as EdgeViewModel;
            if (edge == null)
            {
                return;
            }
            var parallelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == edge.Target && edge.Source == e.Target);

            if (parallelEdge == null)
            {
                return;
            }

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

            edge.RoutingPoints = new[] { sourcePos, bypassPoint1, targetPos };
            parallelEdge.RoutingPoints = new[] { targetPos, bypassPoint2, targetPos };
            graphArea.UpdateAllEdges();
        }

        private int numberOfVertex;

        private VertexControl CreateVertexControl(Point position)
        {
            var vc = CreateVertexControl(new NodeViewModel
            {
                Name = "S" + numberOfVertex,
                IsFinal = false,
                IsInitial = false,
                IsExpanded = false
            });
            vc.SetPosition(position);
            return vc;
        }

        private VertexControl CreateVertexControl(NodeViewModel data)
        {
            data.PropertyChanged += VertexEdited;
            numberOfVertex++;
            var vc = new VertexControl(data);
            data.PropertyChanged += errorReporter.GraphEdited;
            graphArea.AddVertexAndData(data, vc, true);
            GraphEdited?.Invoke(this, EventArgs.Empty);
            return vc;
        }

        private Cursor deletionCursor;

        /// <summary>
        /// Handles changing of the selected tool
        /// </summary>
        private void ToolSelected(object sender, EventArgs e)
        {
            switch (Toolbar.SelectedTool)
            {
                case SelectedTool.Delete:
                    {
                        zoomControl.Cursor = deletionCursor;
                        ClearEditMode();
                        ClearSelectMode();
                        graphArea.SetEdgesDrag(false);
                        return;
                    }
                case SelectedTool.Edit:
                    {
                        zoomControl.Cursor = Cursors.Pen;
                        ClearSelectMode();
                        graphArea.SetEdgesDrag(false);
                        return;
                    }
                case SelectedTool.Select:
                    {
                        zoomControl.Cursor = Cursors.Hand;
                        ClearEditMode();
                        graphArea.SetEdgesDrag(false);
                        graphArea.SetVerticesDrag(true, true);
                        return;
                    }
                default:
                    {
                        return;
                    }
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

        /// <summary>
        /// Selects node view model by the vertex control
        /// </summary>
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
                        {
                            CreateEdgeControl(args.VertexControl);
                            break;
                        }
                    case SelectedTool.Delete:
                        {
                            SafeRemoveVertex(args.VertexControl);
                            break;
                        }
                    default:
                        {
                            if (Toolbar.SelectedTool == SelectedTool.Select && args.Modifiers == ModifierKeys.Control)
                            {
                                SelectVertex(args.VertexControl);
                            }
                            break;
                        }
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
            if (ExecutorViewModel.InSimulation)
            {
                return;
            }
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

        public void ConvertNfaToDfa()
        {
            try
            {
                var dfaGraph = NfaToDfaConverter.Convert(graphArea.LogicCore.Graph);
                graphArea.VertexList.Values.ForEach(SafeRemoveVertex);
                dfaGraph.Vertices.ForEach(node => CreateVertexControl(node));
                dfaGraph.Edges.ForEach(edge => CreateEdgeControl(edge));
                numberOfVertex = graphArea.VertexList.Count;
                graphArea.RelayoutGraph();
                GraphEdited?.Invoke(this, EventArgs.Empty);
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, Lang.Errors_InvalidAutomaton, MessageBoxButton.OK);
            }
        }

        public void Dispose()
        {
            editor?.Dispose();
            graphArea?.Dispose();
        }
    }
}