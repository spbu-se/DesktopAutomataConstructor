using ControlsLibrary.ViewModel;
using GraphX.Controls;
using System;

namespace ControlsLibrary.Controls.Scene.Commands
{
    public class CreateVertexCommand : ISceneCommand
    {
        private readonly GraphArea graphArea;
        private readonly VertexControl vertexControl;
        public CreateVertexCommand(GraphArea graphArea, VertexControl vertexControl)
        {
            this.graphArea = graphArea;
            this.vertexControl = vertexControl;
        }

        public bool CanBeUndone => true;
        public void Execute()
        {
            graphArea.AddVertexAndData(vertexControl.Vertex as NodeViewModel, vertexControl, true);
        }

        public void Undo()
        {
            var command = new RemoveVertexCommand(graphArea, vertexControl);
            command.Execute();
        }
    }
}
