using System.Collections.Generic;
using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;
using System.Windows.Input;
using ControlsLibrary.Infrastructure.Command;
using QuickGraph;
using System.Linq;

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
                //OnPropertyChanged();
            }
        }

        public ICommand StepInCommand { get; }
        private void OnStepInCommandExecuted(object p)
        {
            FA.SingleStep();
            ActualStates = FA.GetCurrentStates();
        }
        private bool CanStepInCommandExecute(object p)
        {
            if (FA == null)
            {
                return false;
            }
            return FA.CanDoStep();
        }

        public ICommand RunCommand { get; }
        private void OnRunCommandExecuted(object p)
        {
            FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetStr(InputString);
            Result = FA.DoAllTransitions(InputString);
        }
        private bool CanRunCommandExecute(object p)
        {
            return true;
        }

        private string _InputStr;
        public string InputString
        {
            get => _InputStr;
            set => Set(ref _InputStr, value);
        }

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
        private bool _Result;
        public bool Result
        {
            get => _Result;
            set => Set(ref _Result, value);
        }

        public ExecutorViewModel()
        {
            #region Commands
            StepInCommand = new RelayCommand(OnStepInCommandExecuted, CanStepInCommandExecute);
            RunCommand = new RelayCommand(OnRunCommandExecuted, CanRunCommandExecute);
            #endregion
        }

    }
}
