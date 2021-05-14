using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;
using System.Linq;

namespace ControlsLibrary.Tests
{
    public class FiniteAutomataTests
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
            var FA = FiniteAutomata.ConvertGraphToAutomata(graph.Edges.ToList(), graph.Vertices.ToList());
            Assert.False(FA.DoAllTransitions("12345"));
            FA.SetString("12345");
            FA.SingleStep();
            //Assert.False(FA.CanDoStep()); //TODO: Debug CanDoStep on an empty FA
            Assert.AreEqual(ResultEnum.NotRunned, FA.StepResult);
        }

        [Test]
        public void SimpleAutomatonFastRunTest()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "1" });

            var FA = FiniteAutomata.ConvertGraphToAutomata(graph.Edges.ToList(), graph.Vertices.ToList());

            Assert.True(FA.DoAllTransitions("1"));
            Assert.False(FA.DoAllTransitions("0"));
        }
    }
}