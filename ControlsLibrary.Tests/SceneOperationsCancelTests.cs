using ControlsLibrary.Controls.Scene;
using ControlsLibrary.Controls.Scene.Commands;
using ControlsLibrary.ViewModel;
using GraphX.Controls;
using NUnit.Framework;
using System.Threading;

namespace ControlsLibrary.Tests
{
    public class SceneOperationsCancelTests
    {
        [Test]
        public void SceneUndoingandRedoingCreateVertexAndEdgeControlsCommands()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                var scene = new Scene();

                var stack = new UndoRedoStack();
                scene.UndoRedoStack = stack;

                var graphArea = scene.GraphArea;

                var firstNodeData = new NodeViewModel() { Name = "S1", IsInitial = false, IsFinal = false };
                var firstVertexControl = new VertexControl(firstNodeData);
                var command = new CreateVertexCommand(graphArea, firstVertexControl);
                command.Execute();
                stack.AddCommand(command);

                var secondNodeData = new NodeViewModel() { Name = "S2", IsInitial = false, IsFinal = false };
                var secondVertexControl = new VertexControl(secondNodeData);
                command = new CreateVertexCommand(graphArea, secondVertexControl);
                command.Execute();
                stack.AddCommand(command);

                var edgeData = new EdgeViewModel(firstNodeData, secondNodeData);
                var edgeControl = new EdgeControl(firstVertexControl, secondVertexControl, edgeData);
                var createEdgeCommand = new CreateEdgeCommand(graphArea, edgeControl);

                stack.Undo();
                Assert.False(graphArea.EdgesList.ContainsKey(edgeData));
                Assert.False(graphArea.GetRelatedEdgeControls(firstVertexControl).Contains(edgeControl));
                Assert.IsNull(edgeControl.Source);
                Assert.IsNull(edgeControl.Target);
                Assert.IsNull(edgeControl.Edge);

                stack.Undo();
                Assert.False(graphArea.VertexList.ContainsKey(secondNodeData));
                Assert.True(graphArea.VertexList.ContainsKey(firstNodeData));

                stack.Undo();
                Assert.False(graphArea.VertexList.ContainsKey(secondNodeData));
                Assert.False(graphArea.VertexList.ContainsKey(firstNodeData));

                stack.Redo();
                Assert.False(graphArea.VertexList.ContainsKey(secondNodeData));
                Assert.True(graphArea.VertexList.ContainsKey(firstNodeData));

                stack.Redo();

                stack.Redo();
                Assert.True(graphArea.EdgesList.ContainsKey(edgeData));
                Assert.True(graphArea.GetRelatedEdgeControls(firstVertexControl).Contains(edgeControl));
                Assert.AreEqual(edgeControl.Source, firstVertexControl);
                Assert.AreEqual(edgeControl.Target, secondVertexControl);
                Assert.AreEqual(edgeControl.Edge as EdgeViewModel, edgeData);

                stack.Undo();
                Assert.False(graphArea.EdgesList.ContainsKey(edgeData));
                Assert.False(graphArea.GetRelatedEdgeControls(firstVertexControl).Contains(edgeControl));
                Assert.IsNull(edgeControl.Source);
                Assert.IsNull(edgeControl.Target);
                Assert.IsNull(edgeControl.Edge);

                // start the Dispatcher processing
                System.Windows.Threading.Dispatcher.Run();
            }));

            // set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);

            // make the thread a background thread
            newWindowThread.IsBackground = true;

            // start the thread
            newWindowThread.Start();
        }

        [Test]
        public void SceneUndoingandRedoingRemoveVertexCommand()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                var scene = new Scene();

                var stack = new UndoRedoStack();
                scene.UndoRedoStack = stack;

                var graphArea = scene.GraphArea;

                var data = new NodeViewModel() { Name = "S", IsInitial = false, IsFinal = false };
                var vertexControl = new VertexControl(data);
                var createCommand = new CreateVertexCommand(graphArea, vertexControl);
                createCommand.Execute();
                stack.AddCommand(createCommand);

                var oldVertexRelatedControls = graphArea.GetRelatedControls(vertexControl);

                var removeCommand = new RemoveVertexCommand(graphArea, vertexControl);
                removeCommand.Execute();
                stack.AddCommand(removeCommand);
                Assert.False(graphArea.VertexList.ContainsKey(data));
                Assert.False(graphArea.VertexList.Values.Contains(vertexControl));
                Assert.IsNull(vertexControl.Vertex);

                stack.Undo();
                Assert.True(graphArea.VertexList.ContainsKey(data));
                Assert.True(graphArea.VertexList.Values.Contains(vertexControl));
                Assert.AreEqual(vertexControl.Vertex as NodeViewModel, data);

                var newVertexRelatedControls = graphArea.GetRelatedControls(vertexControl);
                Assert.AreEqual(oldVertexRelatedControls, newVertexRelatedControls);

                stack.Redo();
                Assert.False(graphArea.VertexList.ContainsKey(data));
                Assert.False(graphArea.VertexList.Values.Contains(vertexControl));
                Assert.IsNull(vertexControl.Vertex);

                // start the Dispatcher processing
                System.Windows.Threading.Dispatcher.Run();
            }));

            // set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);

            // make the thread a background thread
            newWindowThread.IsBackground = true;

            // start the thread
            newWindowThread.Start();
        }
    }
}
