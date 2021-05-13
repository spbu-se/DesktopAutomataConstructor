using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;
using System.Linq;

namespace ControlsLibrary.Tests
{
    public class TestPanelViewModelTests
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        private FAExecutor executor;

        [SetUp]
        public void SetUp()
        {
            var state1 = new NodeViewModel() { ID = 1, IsInitial = true };
            var state2 = new NodeViewModel() { ID = 2, IsFinal = true };
            graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            graph.AddVertex(state1);
            graph.AddVertex(state2);
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "1" });
            executor = new FAExecutor(graph);
        }

        [Test]
        public void SerializationTest()
        {
            var tests = new TestPanelViewModel();
            tests.AddTestCommand.Execute(null);
            var test = tests.Tests.FirstOrDefault();
            var testString = "12345";
            test.TestString = testString;
            var path = "../../../Files/tests.xml";
            tests.Save(path);
            tests.Tests.Clear();
            tests.Open(path);
            Assert.AreEqual(1, tests.Tests.Count);
            Assert.AreEqual(testString, tests.Tests.FirstOrDefault().TestString);
        }

        [Test]
        public void RunAllTestsTest()
        {
            var tests = new TestPanelViewModel();
            Assert.False(tests.RunAllTestsCommand.CanExecute(null));
            var notified = false;
            tests.PropertyChanged += (sender, e) => notified = true;
            tests.Open("../../../Files/SimpleTests.xml");
            tests.Executor = executor;
            Assert.True(tests.Tests.All(test => test.Result == ResultEnum.NotRunned));
            Assert.True(tests.RunAllTestsCommand.CanExecute(null));
            tests.RunAllTestsCommand.Execute(null);
            Assert.True(tests.NumberOfPassedTests == 2 && tests.NumberOfFailedTests == 1);
            Assert.True(notified);
        }

        [Test]
        public void RunSingleTestTest()
        {
            var tests = new TestPanelViewModel();
            Assert.False(tests.RunAllTestsCommand.CanExecute(null));
            var notified = false;
            tests.PropertyChanged += (sender, e) => notified = true;
            tests.Open("../../../Files/SimpleTests.xml");
            tests.Executor = executor;
            Assert.True(tests.Tests.Any(test => test.Result == ResultEnum.NotRunned));
            Assert.True(tests.RunAllTestsCommand.CanExecute(null));
            tests.Tests.FirstOrDefault(test => test.TestString == "1").ExecuteCommand.Execute(null);
            Assert.True(tests.NumberOfPassedTests == 1 && tests.NumberOfFailedTests == 0 && tests.NumberOfNotRunnedTests == 2);
            Assert.True(notified);
        }
    }
}