using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using System.Threading;

namespace ControlsLibrary.Tests
{
    public class SceneCancelingTests
    {
        [Test]
        public void SceneEditingDisablingDuringSimulationTest()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                var scene = new Scene();

                var help = new ErrorReporterViewModel();
                scene.ErrorReporter = help;

                var graph = help.Graph;

                var state1 = new NodeViewModel { ID = 1, IsInitial = true };
                var state2 = new NodeViewModel { ID = 2, IsFinal = true };
                graph.AddVertex(state1);
                graph.AddVertex(state2);
                graph.AddEdge(new EdgeViewModel(state1, state1) { TransitionTokensString = "1" });
                graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "0" });
                graph.AddEdge(new EdgeViewModel(state2, state1) { TransitionTokensString = "1" });
                var executor = new FAExecutor(graph);

                var executorViewModel = new ExecutorViewModel() { Executor = executor };

                executorViewModel.StartDebugCommand.Execute(null);

                foreach (var vertex in graph.Vertices)
                {
                    Assert.False(vertex.EditionAvailable);
                }

                foreach (var edge in graph.Edges)
                {
                    Assert.False(edge.EditionAvailable);
                }

                executorViewModel.DropDebugCommand.Execute(null);

                foreach (var vertex in graph.Vertices)
                {
                    Assert.True(vertex.EditionAvailable);
                }

                foreach (var edge in graph.Edges)
                {
                    Assert.True(edge.EditionAvailable);
                }

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