using ControlsLibrary.ViewModel;
using GraphX.Controls;

namespace ControlsLibrary.Controls.Scene.Commands
{
    class CreateEdgeCommand : ISceneCommand
    {
        private readonly GraphArea graphArea;
        private EdgeControl edgeControl;

        public CreateEdgeCommand(GraphArea graphArea, EdgeControl edgeControl)
        {
            this.graphArea = graphArea;
            this.edgeControl = edgeControl;
        }
        public bool CanBeUndone => true;

        public void Execute()
        {
            var data = edgeControl.Edge as EdgeViewModel;
            var copy = new EdgeControl(edgeControl.Source,
                edgeControl.Target, data); 
            graphArea.InsertEdgeAndData(data, edgeControl, 0, true);
            ParallelEdgesProblemSolver.AvoidParallelEdges(graphArea, edgeControl);
            edgeControl = copy;
        }

        public void Undo()
        {
            var copy = new EdgeControl(edgeControl.Source,
                edgeControl.Target, edgeControl.Edge as EdgeViewModel); 
            var command = new RemoveEdgeCommand(graphArea, edgeControl);
            command.Execute();
            edgeControl = copy;
        }
    }
}
