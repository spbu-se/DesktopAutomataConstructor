using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace ControlsLibrary.Tests
{
    public class ScenePartialLoadTests
    {
        [Test]
        public void ScenePartialLoadTest()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                var scene = new Scene();
                var help = new ErrorReporterViewModel();
                scene.ErrorReporter = help;
                const string path1 = "../../../Files/NotEmptyScene1.xml";
                const string path2 = "../../../Files/NotEmptyScene2.xml";
                var state1 = new NodeViewModel { Name = "S1", IsInitial = true };
                var state2 = new NodeViewModel { Name = "S2", IsFinal = true };
                var state3 = new NodeViewModel { Name = "S3", IsInitial = true };
                var state4 = new NodeViewModel { Name = "S4", IsFinal = true };
                help.Graph.AddVertex(state1);
                help.Graph.AddVertex(state2);
                var transition1 = new EdgeViewModel(state1, state2) { IsEpsilon = true, TransitionTokensString = "1" };
                var transition2 = new EdgeViewModel(state1, state1) { TransitionTokensString = "0" };
                var transition3 = new EdgeViewModel(state3, state4) { TransitionTokensString = "34" };
                help.Graph.AddEdge(transition1);
                help.Graph.AddEdge(transition2);
                scene.Save(path1);
                FreeHelp(help);
                help.Graph.AddVertex(state3);
                help.Graph.AddVertex(state4);
                help.Graph.AddEdge(transition3);
                scene.Save(path2);
                FreeHelp(help);
                scene.Open(path1);
                scene.OpenOverAutomatonOnScene(path2);
                Assert.True(help.Graph.VertexCount == 4 && help.Graph.EdgeCount == 3);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S1").IsInitial);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S2").IsFinal);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S3").IsInitial);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S4").IsFinal);
                Assert.True(help.Graph.Edges.FirstOrDefault(v => v.TransitionTokensString == "1").IsEpsilon);
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "0"));
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "34"));
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
        
        private void FreeHelp(ErrorReporterViewModel help)
        {
            foreach (var vertex in help.Graph.Vertices)
            {
                help.Graph.RemoveVertex(vertex);
            }

            foreach (var edge in help.Graph.Edges)
            {
                help.Graph.RemoveEdge(edge);
            }
        }
    }
}