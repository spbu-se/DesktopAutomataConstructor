using AutomataConstructor.Models.GraphModels;
using AutomataConstructor.ViewModels;
using AutomataConstructor.Models.GraphEditorModels;
using GraphX.Common.Enums;
using GraphX.Controls;
using System.Windows;
using System.Windows.Input;
using System;
using System.Linq;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

            butSelect.IsChecked = true;
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
            graphLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.None;
            graphLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            graphLogic.EdgeCurvingEnabled = true;
            graphArea.SetVerticesMathShape(VertexShape.Circle);
        }

        private VertexControl selectedVertex;
        private SelectedTool selectedTool;
        private readonly EditorObjectManager editor;

        void zoomControl_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void CreateEdgeControl(object vc)
        {
            throw new NotImplementedException();
        }

        private object CreateVertexControl(Point position)
        {
            var data = new StateVertex() { Name = "Vertex " + (graphArea.VertexList.Count + 1), IsFinal = false, IsInitial = false};
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            return vc;
        }

        void ToolbarButton_Checked(object sender, RoutedEventArgs e)
        {
            if (butDelete.IsChecked == true && sender == butDelete)
            {
                butEdit.IsChecked = false;
                butSelect.IsChecked = false;
                zoomControl.Cursor = Cursors.Help;
                selectedTool = SelectedTool.Delete;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (butEdit.IsChecked == true && sender == butEdit)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                zoomControl.Cursor = Cursors.Pen;
                selectedTool = SelectedTool.Edit;
                ClearSelectMode();
                return;
            }
            if (butSelect.IsChecked == true && sender == butSelect)
            {
                butEdit.IsChecked = false;
                butDelete.IsChecked = false;
                zoomControl.Cursor = Cursors.Hand;
                selectedTool = SelectedTool.Select;
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
                graphArea.SetVerticesDrag(false);
        }

        private void ClearEditMode()
        {
            if (selectedVertex != null) HighlightBehaviour.SetHighlighted(selectedVertex, false);
            editor.DestroyVirtualEdge();
            selectedVertex = null;
        }


    }
}
