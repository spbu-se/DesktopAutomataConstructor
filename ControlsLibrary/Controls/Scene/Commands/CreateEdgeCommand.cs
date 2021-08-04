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
            //references to properties may be lost because of execution of Undo command or independent Remove- one
        }

        public void Undo()
        {
            var command = new RemoveEdgeCommand(graphArea, edgeControl);
            command.Execute();
            //the data isn't lost as far as it is preserved in Execute method of RemoveEdgeCommand
        }
    }
}
