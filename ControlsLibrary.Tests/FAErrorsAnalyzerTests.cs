using ControlsLibrary.Model;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;
using System.Linq;

namespace ControlsLibrary.Tests
{
    public class FAErrorsAnalyzerTests
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
            Assert.True(FAAnalyzer.GetErrors(graph).Count == 0);
        }

        [Test]
        public void MissingInitialStateTest()
        {
            graph.AddVertex(new NodeViewModel());

            Assert.True(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetInitial));
        }

        [Test]
        public void MissingAcceptingStateTest()
        {
            graph.AddVertex(new NodeViewModel());

            Assert.True(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetAccepting));
        }

        [Test]
        public void MoreThanOneInitialStateTest()
        {
            graph.AddVertex(new NodeViewModel() { IsInitial = true });
            graph.AddVertex(new NodeViewModel() { IsInitial = true });

            Assert.False(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetInitial));
        }

        [Test]
        public void MoreThanOneAcceptingStateTest()
        {
            graph.AddVertex(new NodeViewModel() { IsFinal = true });
            graph.AddVertex(new NodeViewModel() { IsFinal = true });

            Assert.False(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetAccepting));
        }

        [Test]
        public void ValidGraphTest()
        {
            graph.AddVertex(new NodeViewModel() { IsFinal = true });
            graph.AddVertex(new NodeViewModel() { IsInitial = true });

            Assert.False(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetAccepting));
            Assert.False(FAAnalyzer.GetErrors(graph).Any(e => e == Lang.Errors_SetInitial));
        }
    }
}