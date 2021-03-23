using System;
using GraphX.Controls;
using AutomataConstructor.Models.GraphModels;
using AutomataConstructor.ViewModels.Base;
using GraphX.Common.Enums;
using System.Drawing;

namespace AutomataConstructor.ViewModels
{
    internal class MainWindowViewModel : ViewModel, IDisposable
    {
        public MainWindowViewModel()
        {
            zoomControl = new ZoomControl();
            SetZoomControlProperties();

            graphArea = new DFAGraphArea();
            SetGraphAreaProperties();
        }

        #region zoomControl
        private readonly ZoomControl zoomControl;

        public ZoomControl ZoomControl { get => zoomControl; }

        private void SetZoomControlProperties()
        {
            zoomControl.Zoom = 2;
            zoomControl.MinZoom = .5;
            zoomControl.MaxZoom = 50;
            zoomControl.ZoomSensitivity = 25;
            zoomControl.IsAnimationEnabled = false;
        } 
        #endregion

        #region graphArea
        private readonly DFAGraphArea graphArea;

        public DFAGraphArea GraphArea { get => graphArea; }

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
        #endregion

        /*private VertexControl CreateVertexControl(Point position)
        {
            var data = new StateVertex("Vertex " + (graphArea.VertexList.Count + 1)) { ImageId = ShowcaseHelper.Rand.Next(0, ThemedDataStorage.EditorImages.Count) };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            return vc;
        }*/

        private void SafeRemoveVertex(VertexControl vc)
            => graphArea.RemoveVertexAndEdges(vc.Vertex as StateVertex);

        public void Dispose()
        {
            if (graphArea != null)
                graphArea.Dispose();
        }
    }
}
