using ControlsLibrary.Model;
using GraphX.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Creates, holds and controls various auxiliary objects on a scene, like virtual edge used to draw future edge
    /// position when user draws a new edge.
    /// </summary>
    internal class EditorObjectManager : IDisposable
    {
        private EdgeBlueprint edgeBlueprint;
        private readonly ZoomControl zoomControl;
        private readonly GraphArea graphArea;
        private readonly ResourceDictionary resourceDictionary;

        public EditorObjectManager(GraphArea graphArea, ZoomControl zoomControl)
        {
            this.graphArea = graphArea;
            this.zoomControl = zoomControl;
            zoomControl.MouseMove += zoomControl_MouseMove;
            resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Templates/EditorTemplates.xaml", UriKind.RelativeOrAbsolute)
            };
        }

        public void CreateVirtualEdge(VertexControl source, Point mousePosition)
        {
            edgeBlueprint = new EdgeBlueprint(source, mousePosition, (LinearGradientBrush)resourceDictionary["EdgeBrush"]);
            graphArea.InsertCustomChildControl(0, edgeBlueprint.EdgePath);
        }

        void zoomControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (edgeBlueprint == null)
            {
                return;
            }
            var position = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
            position.Offset(2, 2);
            edgeBlueprint.UpdateTargetPosition(position);
        }

        private void ClearEdgeBp()
        {
            if (edgeBlueprint == null)
            {
                return;
            }
            graphArea.RemoveCustomChildControl(edgeBlueprint.EdgePath);
            edgeBlueprint.Dispose();
            edgeBlueprint = null;
        }

        public void DestroyVirtualEdge() => ClearEdgeBp();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
