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
            EdgeControl ec;
            EdgeControl copy;
            EdgeViewModel data;
            for (int i = 0; i < edges.Count; i++)
            { 
                ec = edges[i];
                data = ec.Edge as EdgeViewModel;
                copy = new EdgeControl(ec.Source, ec.Target, data);
                graphArea.RemoveEdge(data, true);
                edges[i] = copy;
            }

            var vertex = vertexControl.Vertex as NodeViewModel;
            graphArea.RemoveVertex(vertexControl.Vertex as NodeViewModel, true);
            vertexControl.Vertex = vertex;
        }

        public void Undo()
        {
            var command = new CompositeCommand(new List<ISceneCommand>());
            command.AddCommand(new CreateVertexCommand(graphArea, vertexControl));

            EdgeControl ec;
            EdgeControl copy;            
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
