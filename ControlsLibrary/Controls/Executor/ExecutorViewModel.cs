using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using ControlsLibrary.ViewModel.Base;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ControlsLibrary.Controls.Executor
{
    /// <summary>
    /// Contains executor data and provides logic of execution
    /// </summary>
    public class ExecutorViewModel : BaseViewModel
    {
        private FiniteAutomata FA;

        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        /// <summary>
        /// Gets or sets FA graph to execute
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph { get => graph; set => Set(ref graph, value); }

        private bool inSimulation = false;

        /// <summary>
        /// Returns true if executor in the simulation state else false
        /// </summary>
        public bool InSimulation { get => inSimulation; private set => Set(ref inSimulation, value); }

        /// <summary>
        /// Starts simulation
        /// </summary>
        public ICommand StartDebugCommand { get; }

        private void OnStartDebugCommandExecuted(object p)
        {
            if (InputString == null || InputString == "")
            {
                return;
            }
            FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetStr(InputString);
            InSimulation = true;
            currentToken = inputString[0].ToString();
            OnPropertyChanged("CurrentToken");
            notPassedString = inputString.Remove(0, 1);
            OnPropertyChanged("NotPassedString");
            ActualStates = FA.GetCurrentStates();
            Result = ResultEnum.NotRunned;
        }

        private bool CanStartDebugCommandExecute(object p)
            => true;

        /// <summary>
        /// Resets executor state to initial
        /// </summary>
        public ICommand DropDebugCommand { get; }

        private void OnDropDebugCommandExecuted(object p)
        {
            InSimulation = false;
            ActualStates.Clear();
            OnPropertyChanged("ActualStates");
            PassedString = "";
            CurrentToken = "";
            NotPassedString = "";
        }

        private bool CanDropDebugCommandExecute(object p)
            => true;

        /// <summary>
        /// Makes one step of execution
        /// </summary>
        public ICommand StepInCommand { get; }

        private void OnStepInCommandExecuted(object p)
        {
            FA.SingleStep();
            ActualStates = FA.GetCurrentStates();
            passedString += currentToken;
            OnPropertyChanged("PassedString");
            if (notPassedString.Length != 0)
            {
                currentToken = notPassedString[0].ToString();
                OnPropertyChanged("CurrentToken");
                notPassedString = notPassedString.Remove(0, 1);
                OnPropertyChanged("NotPassedString");
            }
            else
            {
                currentToken = "";
                OnPropertyChanged("CurrentToken");
            }
            if (!FA.CanDoStep())
            {
                Result = FA.StepResult;
            }
        }

        private bool CanStepInCommandExecute(object p)
        {
            if (!InSimulation || FA == null)
            {
                return false;
            }
            return FA.CanDoStep();
        }

        /// <summary>
        /// Gets result of execution by fast run and sets it on the result string
        /// </summary>
        public ICommand RunCommand { get; }

        private void OnRunCommandExecuted(object p)
        {
            var errors = FAAnalyzer.GetErrors(Graph);
            if (errors.Count > 0)
            {
                MessageBox.Show(errors.Aggregate("", (folder, error) => folder + error + "\n"), "Invalid automat!", MessageBoxButton.OK);
                return;
            }
            FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetStr(InputString);
            Result = FA.DoAllTransitions(InputString) ? ResultEnum.Passed : ResultEnum.Failed;
        }

        private bool CanRunCommandExecute(object p)
        {
            return !inSimulation;
        }

        private string inputString = "";

        /// <summary>
        /// The input string for automaton
        /// </summary>
        public string InputString
        {
            get => inputString;
            set
            {
                if (inSimulation)
                {
                    return;
                }
                Result = ResultEnum.NotRunned;
                Set(ref inputString, value);
            }
        }

        private string passedString = "";

        /// <summary>
        /// The part of a string that already has been feed to an automaton
        /// </summary>
        public string PassedString { get => passedString; set => passedString = value; }

        private string currentToken = "";

        /// <summary>
        /// The token which an automaton is handling at the moment
        /// </summary>
        public string CurrentToken { get => currentToken; set => currentToken = value; }

        private string notPassedString = "";

        /// <summary>
        /// The not handled by an automaton part of a string
        /// </summary>
        public string NotPassedString { get => notPassedString; set => notPassedString = value; }

        private List<int> actualStates;

        /// <summary>
        /// The list of states ids that an automaton stays on at the moment
        /// </summary>
        public List<int> ActualStates { get => actualStates; set => Set(ref actualStates, value); }

        private ResultEnum _Result;

        /// <summary>
        /// The current result of an execution
        /// </summary>
        public ResultEnum Result
        {
            get => _Result;
            set
            {
                Set(ref _Result, value);
                OnPropertyChanged("StringResult");
            }
        }

        /// <summary>
        /// The result converted into the string type
        /// </summary>
        public string StringResult { get => ResultPrinter.PrintResult(Result); }

        public ExecutorViewModel()
        {
            #region Commands
            StartDebugCommand = new RelayCommand(OnStartDebugCommandExecuted, CanStartDebugCommandExecute);
            DropDebugCommand = new RelayCommand(OnDropDebugCommandExecuted, CanDropDebugCommandExecute);
            StepInCommand = new RelayCommand(OnStepInCommandExecuted, CanStepInCommandExecute);
            RunCommand = new RelayCommand(OnRunCommandExecuted, CanRunCommandExecute);
            #endregion
        }
    }
}
