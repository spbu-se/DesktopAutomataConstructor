using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;

namespace ControlsLibrary.Tests
{
    public class FATypeAnalyzerTests
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        [SetUp]
        public void SetUp()
        {
            graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
        }

        [Test]
        public void EmptyGraphTest()
        {
            Assert.AreEqual(FATypeEnum.DFA, FAAnalyzer.GetType(graph));
        }

        [Test]
        public void SimpleDFATest()
        {
            var vertex1 = new NodeViewModel();
            var vertex2 = new NodeViewModel();
            var vertex3 = new NodeViewModel();
            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);
            graph.AddVertex(vertex3);

            graph.AddEdge(new EdgeViewModel(vertex1, vertex2) { TransitionTokensString = "1" });
            graph.AddEdge(new EdgeViewModel(vertex1, vertex3) { TransitionTokensString = "2" });

            Assert.AreEqual(FATypeEnum.DFA, FAAnalyzer.GetType(graph));
        }

        [Test]
        public void SimpleNFATest()
        {
            var vertex1 = new NodeViewModel();
            var vertex2 = new NodeViewModel();
            var vertex3 = new NodeViewModel();
            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);
            graph.AddVertex(vertex3);

            graph.AddEdge(new EdgeViewModel(vertex1, vertex2) { TransitionTokensString = "1" });
            graph.AddEdge(new EdgeViewModel(vertex1, vertex3) { TransitionTokensString = "1" });

            Assert.AreEqual(FATypeEnum.NFA, FAAnalyzer.GetType(graph));
        }

        [Test]
        public void SelfLoopedNFATest()
        {
            var vertex1 = new NodeViewModel();
            var vertex2 = new NodeViewModel();
            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);

            graph.AddEdge(new EdgeViewModel(vertex1, vertex2) { TransitionTokensString = "1" });
            graph.AddEdge(new EdgeViewModel(vertex1, vertex1) { TransitionTokensString = "1" });

            Assert.AreEqual(FATypeEnum.NFA, FAAnalyzer.GetType(graph));
        }

        [Test]
        public void SimpleEpsilonNFATest()
        {
            var vertex1 = new NodeViewModel();
            var vertex2 = new NodeViewModel();
            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);

            graph.AddEdge(new EdgeViewModel(vertex1, vertex2) { IsEpsilon = true });

            Assert.AreEqual(FATypeEnum.EpsilonNFA, FAAnalyzer.GetType(graph));
        }

        [Test]
        public void SelfLoopedEpsilonNFATest()
        {
            var vertex1 = new NodeViewModel();
            graph.AddVertex(vertex1);

            graph.AddEdge(new EdgeViewModel(vertex1, vertex1) { IsEpsilon = true });

            Assert.AreEqual(FATypeEnum.EpsilonNFA, FAAnalyzer.GetType(graph));
        }
    }
}