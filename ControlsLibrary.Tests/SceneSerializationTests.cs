using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace ControlsLibrary.Tests
{
    public class SceneSerializationTests
    {
        [Test]
        public void EmptySceneTest()
        {
            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                var scene = new Scene();
                var help = new ErrorReporterViewModel();
                scene.ErrorReporter = help;
                const string path = "../../../Files/EmptyScene.xml";
                scene.Save(path);
                help.Graph.AddVertex(new NodeViewModel());
                scene.Open(path);
                Assert.True(help.Graph.VertexCount == 0 && help.Graph.EdgeCount == 0);

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
        public void SceneSerializationTest()
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
                help.Graph.AddVertex(state1);
                help.Graph.AddVertex(state2);
                var transition1 = new EdgeViewModel(state1, state2) { IsEpsilon = true, TransitionTokensString = "1" };
                var transition2 = new EdgeViewModel(state1, state1) { TransitionTokensString = "0" };
                help.Graph.AddEdge(transition1);
                help.Graph.AddEdge(transition2);
                scene.Save(path);
                help.Graph.RemoveVertex(state1);
                help.Graph.RemoveVertex(state2);
                help.Graph.RemoveEdge(transition1);
                help.Graph.RemoveEdge(transition2);
                scene.Open(path);
                Assert.True(help.Graph.VertexCount == 2 && help.Graph.EdgeCount == 2);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S1").IsInitial);
                Assert.True(help.Graph.Vertices.FirstOrDefault(v => v.Name == "S2").IsFinal);
                Assert.True(help.Graph.Edges.FirstOrDefault(v => v.TransitionTokensString == "1").IsEpsilon);
                Assert.True(help.Graph.Edges.Any(v => v.TransitionTokensString == "0"));
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