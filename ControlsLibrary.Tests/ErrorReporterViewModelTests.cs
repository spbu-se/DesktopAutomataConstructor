
using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;
using System.ComponentModel;
using System.Linq;

namespace ControlsLibrary.Tests
{
    public class ErrorReporterViewModelTests
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
        public void ErrorReporterSimpleTest()
        {
            var notified = false;
            void CheckNotified(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ErrorReporterViewModel.ErrorMessage))
                {
                    notified = true;
                }
            }
            var errorReporter = new ErrorReporterViewModel();
            errorReporter.PropertyChanged += CheckNotified;
            errorReporter.Graph = graph;
            Assert.AreEqual(Lang.noIssuesFound, errorReporter.ErrorMessage);
            var state1 = graph.Vertices.FirstOrDefault(v => v.ID == 1);
            state1.PropertyChanged += errorReporter.GraphEdited;
            state1.IsInitial = false;
            Assert.AreEqual("1", errorReporter.ErrorMessage);
            Assert.AreEqual(1, errorReporter.Errors.Count);
            Assert.True(notified);
        }
    }
}
