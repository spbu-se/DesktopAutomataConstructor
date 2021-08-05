using System.Linq;
using System.Threading;
using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel;
using NUnit.Framework;

namespace ControlsLibrary.Tests.Files
{
    public class SceneSaveSelectedAreaTests
    {
        [Test]
        public void SceneSaveSelectedAreaTest()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                var scene = new Scene();
                var help = new ErrorReporterViewModel();
                scene.ErrorReporter = help;
                const string path = "../../../Files/NotEmptyScene.xml";
                var state1 = new NodeViewModel { Name = "S1", IsInitial = true };
                var state2 = new NodeViewModel { Name = "S2", IsFinal = true };
                var state3 = new NodeViewModel { Name = "S3"};
                var state4 = new NodeViewModel { Name = "S4"};
                var state5 = new NodeViewModel { Name = "S5"};
                help.Graph.AddVertex(state1);
                help.Graph.AddVertex(state2);
                help.Graph.AddVertex(state3);
                help.Graph.AddVertex(state4);
                help.Graph.AddVertex(state5);
                var transition1 = new EdgeViewModel(state1, state2) { IsEpsilon = true, TransitionTokensString = "1" };
                var transition2 = new EdgeViewModel(state1, state1) { TransitionTokensString = "0" };
                var transition3 = new EdgeViewModel(state2, state3) { TransitionTokensString = "23" };
                var transition4 = new EdgeViewModel(state3, state4) { TransitionTokensString = "34" };
                var transition5 = new EdgeViewModel(state4, state2) { TransitionTokensString = "42" };
                var transition6 = new EdgeViewModel(state5, state1) { TransitionTokensString = "51" };
                var transition7 = new EdgeViewModel(state2, state5) { TransitionTokensString = "25" };
                help.Graph.AddEdge(transition1);
                help.Graph.AddEdge(transition2);
                help.Graph.AddEdge(transition3);
                help.Graph.AddEdge(transition4);
                help.Graph.AddEdge(transition5);
                help.Graph.AddEdge(transition6);
                help.Graph.AddEdge(transition7);
                scene.SetVertexSelected(scene.GetVertexControl(state1), true);
                scene.SetVertexSelected(scene.GetVertexControl(state2), true);
                scene.SetVertexSelected(scene.GetVertexControl(state5), true);
                scene.SaveSelectedArea(path);
                FreeHelp(help);
                scene.Open(path);
                Assert.True(help.Graph.VertexCount == 3 && help.Graph.EdgeCount == 4);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S1").IsInitial);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S2").IsFinal);
                Assert.False(help.Graph.Vertices.Any(v => v.Name == "S3"));
                Assert.False(help.Graph.Vertices.Any(v => v.Name == "S4"));
                Assert.False(help.Graph.Vertices.Any(v => v.Name == "S5"));
                Assert.True(help.Graph.Edges.FirstOrDefault(v => v.TransitionTokensString == "1").IsEpsilon);
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "0"));
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "51"));
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "25"));
                Assert.False(help.Graph.Edges.Any(v => v.TransitionTokensString == "23"));
                Assert.False(help.Graph.Edges.Any(v => v.TransitionTokensString == "34"));
                Assert.False(help.Graph.Edges.Any(v => v.TransitionTokensString == "42"));
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