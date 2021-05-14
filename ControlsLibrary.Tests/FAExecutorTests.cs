using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;

namespace ControlsLibrary.Tests
{
    public class FAExecutorTests
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        [SetUp]
        public void SetUp()
        {
            graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
        }

        [Test]
        public void SimpleFastExecutionTest()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "1" });
            var executor = new FAExecutor(graph);
            Assert.AreEqual(ResultEnum.Passed, executor.Execute("1"));
            Assert.AreEqual(ResultEnum.Failed, executor.Execute("0"));
        }

        [Test]
        public void ChangedGraphFastExecutionTest()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            var transition = new EdgeViewModel(state1, state2) { TransitionTokensString = "1" };
            graph.AddEdge(transition);
            var executor = new FAExecutor(graph);
            executor.Execute("1");

            transition.TransitionTokensString = "0";
            Assert.AreEqual(ResultEnum.Passed, executor.Execute("0"));
            Assert.AreEqual(ResultEnum.Failed, executor.Execute("1"));
        }

        [Test]
        public void SimpleStartDebugTest()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "1" });
            var executor = new FAExecutor(graph);
            var FA = executor.StartDebug("1");
            Assert.AreEqual(true, FA.DoAllTransitions("1"));
            Assert.AreEqual(false, FA.DoAllTransitions("0"));
        }
    }
}