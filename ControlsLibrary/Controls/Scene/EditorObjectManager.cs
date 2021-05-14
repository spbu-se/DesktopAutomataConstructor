using GraphX.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Creates, holds and controls various auxiliary objects on a scene, like virtual edge used to draw future edge
    /// position when user draws a new edge.
    /// </summary>
    internal class EditorObjectManager : IDisposable
    {
        private EdgeBlueprint edgeBlueprint;
        private ZoomControl zoomControl;
        private GraphArea graphArea;
        private ResourceDictionary resourceDictionary;

        /// <summary>
        /// The basic constructor
        /// </summary>
        /// <param name="graphArea"></param>
        /// <param name="zoomControl"></param>
        public EditorObjectManager(GraphArea graphArea, ZoomControl zoomControl)
        {
            this.graphArea = graphArea;
            this.zoomControl = zoomControl;
            zoomControl.MouseMove += ZoomControlMouseMove;
            resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/ControlsLibrary;component/View/Templates/EditorTemplates.xaml", UriKind.RelativeOrAbsolute)
            };
        }

        /// <summary>
        /// Creates a new edge blueprint on the scene
        /// </summary>
        /// <param name="source">Source vertex for a edge bluepting</param>
        public void CreateVirtualEdge(VertexControl source)
        {
            edgeBlueprint = new EdgeBlueprint(source, (SolidColorBrush)resourceDictionary["EdgeArrowBrush"]);
            graphArea.InsertCustomChildControl(0, edgeBlueprint.EdgePath);
        }

        private void ZoomControlMouseMove(object sender, MouseEventArgs e)
        {
            if (edgeBlueprint == null)
            {
                return;
            }
            var position = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
            position.Offset(2, 2);
            edgeBlueprint.UpdateTargetPosition(position);
        }

        private void ClearEdgeBluePrint()
        {
            if (edgeBlueprint == null)
            {
                return;
            }
            graphArea.RemoveCustomChildControl(edgeBlueprint.EdgePath);
            edgeBlueprint.Dispose();
            edgeBlueprint = null;
        }

        public void DestroyVirtualEdge() => ClearEdgeBluePrint();

        public void Dispose()
        {
            ClearEdgeBluePrint();
            graphArea = null;
            if (zoomControl != null)
            {
                zoomControl.MouseMove -= ZoomControlMouseMove;
            }
            zoomControl = null;
            resourceDictionary = null;
        }
    }
}