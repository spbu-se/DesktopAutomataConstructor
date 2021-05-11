using System.Collections.Generic;
using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.ViewModel;
using ControlsLibrary.Model;
using System.Windows.Input;
using ControlsLibrary.Infrastructure.Command;
using QuickGraph;
using System.Linq;
using System.Windows;

namespace ControlsLibrary.Controls.Executor
{
    public class ExecutorViewModel : BaseViewModel
    {
        private FiniteAutomata FA;
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        private bool inSimulation = false;

        public bool InSimulation { get => inSimulation; set => Set(ref inSimulation, value); }

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

        public ICommand StepInCommand { get; }

        //TODO: Make in the FA methods which returns result in 
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
        public string PassedString { get => passedString; set => passedString = value; }

        private string currentToken = "";
        public string CurrentToken { get => currentToken; set => currentToken = value; }

        private string notPassedString = "";
        public string NotPassedString { get => notPassedString; set => notPassedString = value; }

        private List<int> _ActualStates;
        public List<int> ActualStates
        {
            get => _ActualStates;
            set
            {
                Set(ref _ActualStates, value);
                string x = "";
                foreach (int state in _ActualStates)
                {
                    x += state.ToString();
                }
                States = x;
            }
        }
        private string _States;
        public string States
        {
            get => _States;
            set => Set(ref _States, value);
        }
        private ResultEnum _Result;
        public ResultEnum Result
        {
            get => _Result;
            set
            {
                Set(ref _Result, value);
                OnPropertyChanged("StringResult");
            }
        }
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
