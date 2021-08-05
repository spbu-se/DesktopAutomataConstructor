using GraphX.Controls;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene.Commands
{
    public class DragCommand : ISceneCommand
    {
        private readonly GraphArea graphArea;
        private readonly ICollection<CustomVertexControl> vertices;
        private readonly double X_coordDiff;
        private readonly double Y_coordDiff;

        public DragCommand(GraphArea graphArea, ICollection<CustomVertexControl> vertices,
        double X_coordDiff, double Y_coordDiff)
        {
            this.graphArea = graphArea;
            this.vertices = vertices;
            this.X_coordDiff = X_coordDiff;
            this.Y_coordDiff = Y_coordDiff;
        }
        public bool CanBeUndone => true;

        public void Execute()
        {
            foreach (var vc in vertices)
            {
                vc.SetPosition(vc.GetPosition().X + X_coordDiff, vc.GetPosition().Y + Y_coordDiff);
            }
        }

        public void Undo()
        {
            var command = new DragCommand(graphArea, vertices, -X_coordDiff, -Y_coordDiff);
            command.Execute();
        }
    }
}

