using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.Scene.Commands;
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
using System.Windows.Documents;
using System.Windows.Input;


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

            ClearSelectMode();
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

            foreach (var node in graphArea.LogicCore.Graph.Vertices)
            {
                node.PropertyChanged += VertexEdited;
            }

            foreach (var edge in graphArea.LogicCore.Graph.Edges)
            {
                edge.PropertyChanged += EdgeEdited;
            }

            GraphEdited?.Invoke(this, EventArgs.Empty);
            undoRedoStack.Clear();
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
            undoRedoStack = new UndoRedoStack();
            this.VertexRemoved += SingleVertexRemoved;
            this.SelectionStarted += StartSelection;
        }

        private UndoRedoStack undoRedoStack;

        public UndoRedoStack UndoRedoStack
        {
            get => undoRedoStack;
            set => undoRedoStack = value;
        }

        public GraphArea GraphArea
        {
            get => graphArea;
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
                case Key.Z:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            undoRedoStack.Undo();
                            // GraphEdited - in case of creation and deletion
                        }
                        return;
                    }
                case Key.Y:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            undoRedoStack.Redo();
                            // GraphEdited - in case of creation and deletion
                        }
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
            zoomControl.MouseMove += OnSceneMouseMove;
            zoomControl.PreviewMouseUp += OnSceneMouseUp;

            using var deletionCursorStream = Application.GetResourceStream(new Uri("pack://application:,,,/ControlsLibrary;component/Controls/Scene/Assets/deletionCursor.cur", UriKind.RelativeOrAbsolute))?.Stream;
            if (deletionCursorStream != null)
            {
                deletionCursor = new Cursor(deletionCursorStream);
            }
            SetZoomControlFixed();
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
            graphArea.VertexSelected += VertexDraggingStarted;
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

                ClearSelectMode(true);
                ClearSelectedVertices();

                var command = new RemoveEdgeCommand(graphArea, args.EdgeControl);
                command.Execute();
                undoRedoStack.AddCommand(command);

                GraphEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        private CustomVertexControl selectedVertex;

        private readonly EditorObjectManager editor;

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
                    ClearSelectMode(true);
                    ClearSelectedVertices();
                    var position = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);

                    position.Offset(-60, -60); //Offset should be the half of the vertex controls width
                    var vc = CreateVertexControl(position);
                    if (selectedVertex != null)
                    {
                        CreateEdgeControl(vc);
                    }
                }
                else
                {
                    ClearSelectMode(true);
                    ClearSelectedVertices();
                    SelectionStarted?.Invoke(this, e);
                }
            }

            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ClearSelectMode(true);
                ClearSelectedVertices();                
                SelectionStarted?.Invoke(this, e);
            }
        }

        private void ClearSelectedVertices()
        {
            if (selectedVertices != null)
            {
                if (selectedVertices.Count > 0)
                {
                    var command = new SelectCommand(selectedVertices, false);
                    undoRedoStack.AddCommand(command);
                }
                selectedVertices = null;
            }
        }

        /// <summary>
        /// Fixes edges routing if vertex was dragged
        /// </summary>
        private void VertexDragged(object sender, VertexSelectedEventArgs args)
        {
            foreach (var edge in graphArea.EdgesList.Where(e => e.Value.Source == args.VertexControl || e.Value.Target == args.VertexControl))
            {
                ParallelEdgesProblemSolver.AvoidParallelEdges(graphArea, edge.Value);
            }
            CreateDragCommand((CustomVertexControl)args.VertexControl);
        }

        private void CreateDragCommand(CustomVertexControl vc)
        {
            var currentPosition = vc.GetPosition();
            if (currentPosition != dragStartPosition)
            {
                DragCommand command;
                if (selectedVertices != null && selectedVertices.Contains(vc))
                {
                    command = new DragCommand(graphArea, selectedVertices,
                        currentPosition.X - dragStartPosition.X, currentPosition.Y - dragStartPosition.Y);
                }
                else
                {
                    command = new DragCommand(graphArea, new HashSet<CustomVertexControl> { vc },
                        currentPosition.X - dragStartPosition.X, currentPosition.Y - dragStartPosition.Y);
                }
                undoRedoStack.AddCommand(command);
            }
        }

        private Point dragStartPosition;
        private void VertexDraggingStarted(object sender, VertexSelectedEventArgs args)
        {
            dragStartPosition = args.VertexControl.GetPosition();
        }

        private void CreateEdgeBlueprint(CustomVertexControl vc)
        {
            editor.CreateVirtualEdge(vc);
            selectedVertex = vc;
            HighlightBehaviour.SetHighlighted(selectedVertex, true);
        }

        private void DestroyEdgeBlueprint()
        {
            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private void CreateEdgeControl(CustomVertexControl vc)
        {
            if (ExecutorViewModel.InSimulation)
            {
                return;
            }

            var data = new EdgeViewModel((NodeViewModel)selectedVertex.Vertex, (NodeViewModel)vc.Vertex);

            // Doesn't create new edges with the same direction
            // TODO: should somehow notice user that edge wasn't created
            if (graphArea.LogicCore.Graph.Edges.Any(e => e.Source == (NodeViewModel)selectedVertex.Vertex && e.Target == (NodeViewModel)vc.Vertex))
            {
                DestroyEdgeBlueprint();
                return;
            }
            data.PropertyChanged += EdgeEdited;
            var ec = new EdgeControl(selectedVertex, vc, data);
            var command = new CreateEdgeCommand(graphArea, ec);
            command.Execute();
            undoRedoStack.AddCommand(command);
            //GraphEdited
            DestroyEdgeBlueprint();
        }

        private int numberOfVertex;

        private CustomVertexControl CreateVertexControl(Point position)
        {
            var data = new NodeViewModel() { Name = "S" + numberOfVertex, IsFinal = false, IsInitial = false, IsExpanded = false };
            data.PropertyChanged += VertexEdited;
            numberOfVertex++;
            var vc = new CustomVertexControl(data);
            data.PropertyChanged += errorReporter.GraphEdited;
            vc.SetPosition(position);

            var command = new CreateVertexCommand(graphArea, vc);
            command.Execute();
            undoRedoStack.AddCommand(command);
            
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
                        graphArea.SetVerticesDrag(false);
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

        private void SetZoomControlFixed()
        {
            initialTranslateX = zoomControl.TranslateX;
            initialTranslateY = zoomControl.TranslateY;
            zoomControl.PropertyChanged += zoomControl_PropertyChanged;
        }

        private void zoomControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(zoomControl.Presenter))
            {
                zoomControl.TranslateX = initialTranslateX;
                zoomControl.TranslateY = initialTranslateY;
            }           
        }

        private void ClearSelectMode(bool soft = false)
        {
            graphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    ((CustomVertexControl)a).IsSelected = false;
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
        private NodeViewModel SelectNode(CustomVertexControl vertexControl)
            => graphArea.VertexList.FirstOrDefault(x => x.Value == vertexControl).Key;

        private void OnSceneVertexSelected(object sender, VertexSelectedEventArgs args)
        {
            var vertexControl = (CustomVertexControl)args.VertexControl;
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                NodeSelected?.Invoke(this, new NodeSelectedEventArgs() { Node = SelectNode(vertexControl) });
                switch (Toolbar.SelectedTool)
                {
                    case SelectedTool.Edit:
                        {
                            if (selectedVertex == null)
                            {
                                CreateEdgeBlueprint(vertexControl);
                            }
                            else
                            {
                                CreateEdgeControl(vertexControl);
                            }                            
                            break;
                        }
                    case SelectedTool.Delete:
                        {
                            SafeRemoveVertex(vertexControl);
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
                    SelectNode(vertexControl).IsExpanded = !SelectNode(vertexControl).IsExpanded;
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

        private void SafeRemoveVertex(CustomVertexControl vc)
        {
            if (ExecutorViewModel.InSimulation)
            {
                return;
            }

            var removeCommand = new CompositeCommand(new List<ISceneCommand>());
            removeCommand.AddCommand(new RemoveVertexCommand(graphArea, vc));
            removeCommand.Execute();
            GraphEdited?.Invoke(this, EventArgs.Empty);

            VertexRemoved?.Invoke(this, new VertexRemovedEventArgs()
            { VertexControl = vc, RemoveCommand = removeCommand });
            
            undoRedoStack.AddCommand(removeCommand);
        }

        private EventHandler<VertexRemovedEventArgs> VertexRemoved;

        private void SingleVertexRemoved(object sender, VertexRemovedEventArgs args)
        {
            if (selectedVertices == null)
                return;

            var vc = args.VertexControl;
            if (selectedVertices.Remove(vc))
            {
                SafeRemoveSelectedVertices(args.RemoveCommand);
                selectedVertices.Add(vc);
            }

            ClearSelectMode(true);
            ClearSelectedVertices();
        }

        private void SafeRemoveSelectedVertices(CompositeCommand groupRemoveCommand)
        {
            foreach (var vc in selectedVertices)
            {
                var command = new RemoveVertexCommand(graphArea, vc);
                command.Execute();
                GraphEdited?.Invoke(this, EventArgs.Empty);
                groupRemoveCommand.AddCommand(command);                
            }
        }

        private AdornerSelectedArea selectedArea;
        private HashSet<CustomVertexControl> selectedVertices;
        private Point mouseDownPosition;
        private double initialTranslateX;
        private double initialTranslateY;

        private EventHandler<MouseButtonEventArgs> SelectionStarted;
        private void SetVertexSelected(CustomVertexControl vc, bool selected)
        {
            vc.IsSelected = selected;
            DragBehaviour.SetIsTagged(vc, selected);
            if (selected)
            {
                selectedVertices.Add(vc);
            }
            else
            {
                selectedVertices.Remove(vc);
            }
        }

        private void UpdateSelectedVertices()
        {
            var selectedRect = selectedArea.SelectedRect;
            Point centrePosition;
            bool selected;
            foreach (var vc in graphArea.VertexList.Values)
            {
                centrePosition = graphArea.TranslatePoint(vc.GetCenterPosition(), zoomControl);
                selected = selectedRect.Contains(centrePosition);
                SetVertexSelected((CustomVertexControl)vc, selected);
            }
        }

        private Rect UpdateSelectedRect(Point mouseCurrentPosition)
        {
            double x, y, width, height;
            x = mouseDownPosition.X < mouseCurrentPosition.X 
                ? mouseDownPosition.X 
                : mouseCurrentPosition.X;
            y = mouseDownPosition.Y < mouseCurrentPosition.Y 
                ? mouseDownPosition.Y 
                : mouseCurrentPosition.Y;

            width = Math.Abs(mouseCurrentPosition.X - mouseDownPosition.X);
            height = Math.Abs(mouseCurrentPosition.Y - mouseDownPosition.Y);

            return new Rect(x, y, width, height);
        }

        private void StartSelection(object sender, MouseButtonEventArgs e)
        {
            zoomControl.Cursor = Cursors.Arrow;
            mouseDownPosition = e.GetPosition(zoomControl);
            InitSelectedArea();
            selectedVertices = new HashSet<CustomVertexControl>();
        }

        private void InitSelectedArea()
        {
            selectedArea = new AdornerSelectedArea(zoomControl);            
            var adornerLayer = AdornerLayer.GetAdornerLayer(selectedArea.AdornedElement);
            adornerLayer.Add(selectedArea);
        }

        private void ClearSelectedArea()
        {
            if (selectedArea != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(selectedArea.AdornedElement);
                adornerLayer.Remove(selectedArea);
                selectedArea = null;
            }          
        }

        private void OnSceneMouseMove(object sender, MouseEventArgs e)
        {
            if (selectedArea != null)
            {
                selectedArea.SelectedRect = UpdateSelectedRect(e.GetPosition(selectedArea.AdornedElement));
                UpdateSelectedVertices();
                selectedArea.InvalidateVisual();
            }
        }

        private void RecoverSelectedCursor()
        {
            switch (Toolbar.SelectedTool)
            {
                case SelectedTool.Select:
                    {
                        zoomControl.Cursor = Cursors.Hand;
                        return;
                    }
                case SelectedTool.Edit:
                    {
                        zoomControl.Cursor = Cursors.Pen;
                        return;
                    }
                case SelectedTool.Delete:
                    {
                        zoomControl.Cursor = deletionCursor;
                        return;
                    }
            }
        }
        private void OnSceneMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedArea != null)
            {
                if (selectedVertices != null && selectedVertices.Count > 0)
                {
                    var command = new SelectCommand(selectedVertices, true);
                    undoRedoStack.AddCommand(command);
                }
                ClearSelectedArea();
                RecoverSelectedCursor();
            }
        }

        public void Dispose()
        {
            editor?.Dispose();
            graphArea?.Dispose();
        }
    }
}

