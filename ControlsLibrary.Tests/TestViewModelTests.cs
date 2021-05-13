using NUnit.Framework;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using QuickGraph;
using System;
using System.Collections.Generic;

namespace ControlsLibrary.Tests
{
    public class TestViewModelTests
    {
        private FAExecutor executor;
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
            executor = new FAExecutor(graph);
        }

        [Test]
        public void RunTest()
        {
            var test = new TestViewModel();
            Assert.False(test.ExecuteCommand.CanExecute(null));
            test.Executor = executor;
            Assert.True(test.ExecuteCommand.CanExecute(null));

            Assert.AreEqual(ResultEnum.NotRunned, test.Result);

            test.ExecuteCommand.Execute(null);
            Assert.AreEqual(ResultEnum.Failed, test.Result);
            test.TestString = "1";
            test.ExecuteCommand.Execute(null);
            Assert.AreEqual(ResultEnum.Passed, test.Result);

            test.ShouldReject = true;
            test.ExecuteCommand.Execute(null);
            Assert.AreEqual(ResultEnum.Failed, test.Result);
        }

        [Test]
        public void RemoveFromStorageTest()
        {
            var storage = new List<TestViewModel>();
            var test = new TestViewModel() { Executor = executor, Storage = storage };
            Assert.False(test.RemoveFromStorageCommand.CanExecute(null));
            storage.Add(test);
            Assert.True(test.RemoveFromStorageCommand.CanExecute(null));
            test.RemoveFromStorageCommand.Execute(null);
            Assert.False(test.RemoveFromStorageCommand.CanExecute(null));
            Assert.False(storage.Contains(test));
        }
    }
}
