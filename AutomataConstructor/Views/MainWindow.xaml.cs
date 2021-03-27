using AutomataConstructor.Models.GraphModels;
using AutomataConstructor.ViewModels;
using AutomataConstructor.Models.GraphEditorModels;
using GraphX.Common.Enums;
using GraphX.Controls;
using System.Windows;
using System.Windows.Input;
using System;
using QuickGraph;
using System.Linq;
using GraphX.Controls.Models;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            SetZoomControlProperties();
            SetGraphAreaProperties();
            editor = new EditorObjectManager(graphArea, zoomControl);
            butDelete.Checked += ToolbarButton_Checked;
            butSelect.Checked += ToolbarButton_Checked;
            butEdit.Checked += ToolbarButton_Checked;
            butProps.Checked += ToolbarButton_Checked;
        }

        private void SetZoomControlProperties()
        {
            zoomControl.Zoom = 2;
            zoomControl.MinZoom = .5;
            zoomControl.MaxZoom = 50;
            zoomControl.ZoomSensitivity = 25;
            zoomControl.IsAnimationEnabled = false;
            ZoomControl.SetViewFinderVisibility(zoomControl, Visibility.Hidden);
            zoomControl.MouseDown += zoomControl_MouseDown;
        }

        private void SetGraphAreaProperties()
        {
            var graphLogic = new DFAGraphLogic();
            graphArea.LogicCore = graphLogic;
            graphLogic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Custom;
            graphLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            graphLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            graphLogic.EdgeCurvingEnabled = true;
            graphArea.VertexSelected += graphArea_VertexSelected;
            graphArea.EdgeSelected += graphArea_EdgeSelected;
        }

        private void graphArea_EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && selectedTool == SelectedTool.Delete)
            {
                graphArea.RemoveEdge(args.EdgeControl.Edge as TransitionEdge, true);
                return;
            }
        }

        private VertexControl selectedVertex;
        private SelectedTool selectedTool;
        private readonly EditorObjectManager editor;

        private void zoomControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedTool == SelectedTool.Edit)
                {
                    var pos = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
                    pos.Offset(-22.5, -22.5);
                    var vc = CreateVertexControl(pos);
                    if (selectedVertex != null)
                        CreateEdgeControl(vc);
                }
                else if (selectedTool == SelectedTool.Select)
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

            var data = new TransitionEdge((StateVertex)selectedVertex.Vertex, (StateVertex)vc.Vertex, "", new List<char>() { 'a' });
            var ec = new EdgeControl(selectedVertex, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);

            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private VertexControl CreateVertexControl(Point position)
        {
            var data = new StateVertex() { Name = "Vertex " + (graphArea.VertexList.Count + 1), IsFinal = false, IsInitial = false };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            return vc;
        }

        void ToolbarButton_Checked(object sender, RoutedEventArgs e)
        {
            if (butProps.IsChecked == true && sender == butProps)
            {
                butEdit.IsChecked = false;
                butSelect.IsChecked = false;
                butDelete.IsChecked = false;
                zoomControl.Cursor = Cursors.Pen;
                selectedTool = SelectedTool.EditProperties;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (butDelete.IsChecked == true && sender == butDelete)
            {
                butEdit.IsChecked = false;
                butSelect.IsChecked = false;
                butProps.IsChecked = false;
                zoomControl.Cursor = Cursors.Help;
                selectedTool = SelectedTool.Delete;
                ClearEditMode();
                ClearSelectMode();
                ClearEditPropertiesMode();
                return;
            }
            if (butEdit.IsChecked == true && sender == butEdit)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butProps.IsChecked = false;
                zoomControl.Cursor = Cursors.Pen;
                selectedTool = SelectedTool.Edit;
                ClearSelectMode();
                ClearEditPropertiesMode();
                return;
            }
            if (butSelect.IsChecked == true && sender == butSelect)
            {
                butEdit.IsChecked = false;
                butDelete.IsChecked = false;
                butProps.IsChecked = false;
                zoomControl.Cursor = Cursors.Hand;
                selectedTool = SelectedTool.Select;
                ClearEditMode();
                ClearEditPropertiesMode();
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
                graphArea.SetVerticesDrag(false);
        }

        private void ClearEditMode()
        {
            if (selectedVertex != null) HighlightBehaviour.SetHighlighted(selectedVertex, false);
            editor.DestroyVirtualEdge();
            selectedVertex = null;
        }

        private void ClearEditPropertiesMode() => properties.Items.Clear();

        void graphArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                switch (selectedTool)
                {
                    case SelectedTool.Edit:
                        CreateEdgeControl(args.VertexControl);
                        break;
                    case SelectedTool.Delete:
                        SafeRemoveVertex(args.VertexControl);
                        break;
                    case SelectedTool.EditProperties:
                        var tb = new TextBox();
                        properties.Items.Clear();
                        properties.Items.Add(tb);
                        break;
                    default:
                        if (selectedTool == SelectedTool.Select && args.Modifiers == ModifierKeys.Control)
                            SelectVertex(args.VertexControl);
                        break;
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
            graphArea.RemoveVertexAndEdges(vc.Vertex as StateVertex);
        }

        public void Dispose()
        {
            if (editor != null)
                editor.Dispose();
            if (graphArea != null)
                graphArea.Dispose();
        }
    }
}
