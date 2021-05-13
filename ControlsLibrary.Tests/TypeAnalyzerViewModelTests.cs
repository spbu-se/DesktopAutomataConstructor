using NUnit.Framework;
using ControlsLibrary.Controls.TypeAnalyzer;
using QuickGraph;
using ControlsLibrary.ViewModel;
using ControlsLibrary.Properties.Langs;
using System.Linq;
using System.ComponentModel;

namespace ControlsLibrary.Tests
{
    public class TypeAnalyzerViewModelTests
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        [SetUp]
        public void SetUp()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "1" });
        }

        [Test]
        public void TypeAnalyzerSimpleTest()
        {
            var notified = false;
            void CheckNotified(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(TypeAnalyzerViewModel.StringType))
                {
                    notified = true;
                }
            }
            var typeAnalyzer = new TypeAnalyzerViewModel();
            typeAnalyzer.PropertyChanged += CheckNotified;
            typeAnalyzer.Graph = graph;
            Assert.AreEqual(Lang.DFA, typeAnalyzer.StringType);
            var state1 = graph.Vertices.FirstOrDefault(v => v.ID == 1);
            graph.AddEdge(new EdgeViewModel(state1, state1) { TransitionTokensString = "1" });
            typeAnalyzer.OnPropertyChanged(nameof(typeAnalyzer.StringType));
            Assert.AreEqual(Lang.NFA, typeAnalyzer.StringType);
            Assert.True(notified);
        }
    }
}
