﻿using System.Collections.Generic;
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

        private bool inSimulation;

        private void DropSimulation()
        {
            inSimulation = false;
            ActualStates.Clear();
        }

        private void StartSimulation()
        {
            inSimulation = true;
            FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetStr(InputString);
            inSimulation = true;
            ActualStates = FA.GetCurrentStates();
        }

        public ICommand StartOrDropDebugCommand { get; }
        private void OnStartOrDropDebugCommandExecuted(object p)
        {
            if (inSimulation)
            {
                DropSimulation();
            }

            StartSimulation();
        }

        private bool CanStartOrDropDebugExecute(object p)
        {
            return true;
        }

        public ICommand StepInCommand { get; }

        //TODO: Make in the FA methods which returns result in 
        private void OnStepInCommandExecuted(object p)
        {
            FA.SingleStep();
            ActualStates = FA.GetCurrentStates();
            if (!FA.CanDoStep())
            {
                
                inSimulation = false;
            }
        }
        private bool CanStepInCommandExecute(object p)
        {
            if (!inSimulation || FA == null)
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
            Result = FA.DoAllTransitions(InputString) ? ResultEnum.Passed : ResultEnum.Failed;
        }
        private bool CanRunCommandExecute(object p)
        {
            return !inSimulation;
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
            StartOrDropDebugCommand = new RelayCommand(OnStartOrDropDebugCommandExecuted, CanStartOrDropDebugExecute);
            StepInCommand = new RelayCommand(OnStepInCommandExecuted, CanStepInCommandExecute);
            RunCommand = new RelayCommand(OnRunCommandExecuted, CanRunCommandExecute);
            #endregion
        }

    }
}