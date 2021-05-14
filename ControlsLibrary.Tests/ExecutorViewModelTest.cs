using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ControlsLibrary.Tests
{
    public class ExecutorViewModelTest
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
            graph.AddEdge(new EdgeViewModel(state1, state1) { TransitionTokensString = "1" });
            graph.AddEdge(new EdgeViewModel(state1, state2) { TransitionTokensString = "0" });
            graph.AddEdge(new EdgeViewModel(state2, state1) { TransitionTokensString = "1" });
            executor = new FAExecutor(graph);
        }

        [Test]
        public void FastExecutionTest()
        {
            var executorViewModel = new ExecutorViewModel() { Executor = executor };
            Assert.AreEqual(ResultEnum.NotRunned, executorViewModel.Result);
            executorViewModel.InputString = "111010";
            executorViewModel.RunCommand.Execute(null);
            Assert.AreEqual(ResultEnum.Passed, executorViewModel.Result);
            executorViewModel.InputString = "1101";
            Assert.AreEqual(ResultEnum.NotRunned, executorViewModel.Result);
            executorViewModel.RunCommand.Execute(null);
            Assert.AreEqual(ResultEnum.Failed, executorViewModel.Result);
        }

        private bool CheckStep(ExecutorViewModel executorViewModel, List<int> actualStates, ResultEnum result,
            string passedString, string currentToken, string notPassedString)
            => new HashSet<int>(executorViewModel.ActualStates).SetEquals(new HashSet<int>(actualStates)) && executorViewModel.Result == result
            && executorViewModel.PassedString == passedString && executorViewModel.CurrentToken == currentToken && executorViewModel.NotPassedString == notPassedString;

        [Test]
        public void StepByStepExecutionTest()
        {
            var executorViewModel = new ExecutorViewModel() { Executor = executor };
            Assert.AreEqual(ResultEnum.NotRunned, executorViewModel.Result);
            var notified = false;
            void CheckNotified(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ExecutorViewModel.InSimulation))
                {
                    notified = true;
                }
            }

            executorViewModel.PropertyChanged += CheckNotified;
            executorViewModel.InputString = "11010";

            executorViewModel.StartDebugCommand.Execute(null);
            Assert.True(executorViewModel.InSimulation);
            Assert.True(notified);
            notified = false;
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1 }, ResultEnum.NotRunned, "", "1", "1010"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1 }, ResultEnum.NotRunned, "1", "1", "010"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1 }, ResultEnum.NotRunned, "11", "0", "10"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 2 }, ResultEnum.NotRunned, "110", "1", "0"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1 }, ResultEnum.NotRunned, "1101", "0", ""));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 2 }, ResultEnum.Passed, "11010", "", ""));

            Assert.False(executorViewModel.StepInCommand.CanExecute(null));

            executorViewModel.DropDebugCommand.Execute(null);

            Assert.False(executorViewModel.InSimulation);
            Assert.True(notified);
        }

        [Test]
        public void StepByStepExecutionOfNonDeterministicAutomatonTest()
        {
            var executorViewModel = new ExecutorViewModel() { Executor = executor };
            graph.Edges.FirstOrDefault(edge => edge.Source.ID == 1 && edge.Target.ID == 2).TransitionTokensString = "01";
            executorViewModel.InputString = "110";

            executorViewModel.StartDebugCommand.Execute(null);
            Assert.True(executorViewModel.InSimulation);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1 }, ResultEnum.NotRunned, "", "1", "10"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1, 2 }, ResultEnum.NotRunned, "1", "1", "0"));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 1, 2 }, ResultEnum.NotRunned, "11", "0", ""));

            executorViewModel.StepInCommand.Execute(null);
            Assert.True(CheckStep(executorViewModel, new List<int>() { 2 }, ResultEnum.Passed, "110", "", ""));
        }

        [Test]
        public void DropDebugTest()
        {
            var executorViewModel = new ExecutorViewModel { Executor = executor };

            executorViewModel.InputString = "11010";

            executorViewModel.StartDebugCommand.Execute(null);

            executorViewModel.StepInCommand.Execute(null);

            executorViewModel.DropDebugCommand.Execute(null);
            Assert.False(executorViewModel.InSimulation);

            Assert.True(CheckStep(executorViewModel, new List<int>(), ResultEnum.NotRunned, "", "", ""));
        }
    }
}