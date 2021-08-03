using ControlsLibrary.ViewModel;
using GraphX.Controls;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene.Commands
{
    class RemoveVertexCommand : ISceneCommand
    {
        private readonly GraphArea graphArea;
        private readonly VertexControl vertexControl;
        private readonly List<EdgeControl> edges;

        private void AddAllEdges()
        {
            foreach (var gc in graphArea.GetRelatedEdgeControls(vertexControl))
            {
                edges.Add((EdgeControl)gc);
            }
        }

        public RemoveVertexCommand(GraphArea graphArea, VertexControl vc)
        {
            this.graphArea = graphArea;
            this.vertexControl = vc;
            this.edges = new List<EdgeControl>();
            AddAllEdges();
        }

        public bool CanBeUndone => true;
        public void Execute()
        {
            var vertex = vertexControl.Vertex as NodeViewModel;
            graphArea.RemoveVertex(vertex, true);
            vertexControl.Vertex = vertex;
            
            EdgeControl ec;
            EdgeControl edge;
            EdgeViewModel data;
            for (int i = 0; i < edges.Count; i++)
            { 
                edge = edges[i];
                data = edge.Edge as EdgeViewModel;
                ec = new EdgeControl(edge.Source, edge.Target, data);
                graphArea.RemoveEdge(data, true);
                edges[i] = ec;
            }            
        }

        public void Undo()
        {
            var command = new CompositeCommand(new List<ISceneCommand>());
            command.AddCommand(new CreateVertexCommand(graphArea, vertexControl));

            EdgeControl copy;
            EdgeControl ec;
            for (int i = 0; i < edges.Count; i++)
            {
                ec = edges[i];
                copy = new EdgeControl(ec.Source, ec.Target, ec.Edge as EdgeViewModel);
                command.AddCommand(new CreateEdgeCommand(graphArea, ec));
                edges[i] = copy;
            }
            command.Execute();            
        }
    }
}
