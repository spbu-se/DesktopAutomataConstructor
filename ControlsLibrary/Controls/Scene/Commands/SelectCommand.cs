using GraphX.Controls;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene.Commands
{
    public class SelectCommand : ISceneCommand
    {
        private readonly ICollection<VertexControl> vertices;
        private readonly bool selected;

        public SelectCommand(ICollection<VertexControl> vertices, bool selected)
        {
            this.vertices = vertices;
            this.selected = selected;
        }
        private void SelectVertex(VertexControl vc)
        {
            DragBehaviour.SetIsTagged(vc, selected);
            HighlightBehaviour.SetHighlighted(vc, selected);
        }
        public bool CanBeUndone => true;

        public void Execute()
        {
            foreach (var vc in vertices)
            {
                SelectVertex(vc);
            }
        }

        public void Undo()
        {
            var command = new SelectCommand(vertices, !selected);
            command.Execute();
        }
    }
}
