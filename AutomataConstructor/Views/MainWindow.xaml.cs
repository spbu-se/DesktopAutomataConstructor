using AutomataConstructor.Models.GraphModels;
using AutomataConstructor.ViewModels;
using AutomataConstructor.Models.GraphEditorModels;
using GraphX.Common.Enums;
using GraphX.Controls;
using System.Windows;

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
        }

        private void SetZoomControlProperties()
        {
            zoomControl.Zoom = 2;
            zoomControl.MinZoom = .5;
            zoomControl.MaxZoom = 50;
            zoomControl.ZoomSensitivity = 25;
            zoomControl.IsAnimationEnabled = false;
            ZoomControl.SetViewFinderVisibility(zoomControl, Visibility.Hidden);
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
    }
}
