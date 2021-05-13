using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel.Base;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Sets FA executor model to execute strings on it
        /// </summary>
        public FAExecutor Executor { private get; set; }

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
            if (string.IsNullOrEmpty(InputString))
            {
                return;
            }
            FA = Executor.StartDebug(InputString);
            FA.SetString(InputString);
            InSimulation = true;
            CurrentToken = inputString[0].ToString();
            NotPassedString = inputString.Remove(0, 1);
            ActualStates = FA.GetCurrentStates();
            Result = ResultEnum.NotRunned;
        }

        private bool CanStartDebugCommandExecute(object p)
            => string.IsNullOrEmpty(InputString);

        /// <summary>
        /// Resets executor state to initial
        /// </summary>
        public ICommand DropDebugCommand { get; }

        private void OnDropDebugCommandExecuted(object p)
        {
            InSimulation = false;
            ActualStates.Clear();
            OnPropertyChanged(nameof(ActualStates));
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
            PassedString += currentToken;
            if (notPassedString.Length != 0)
            {
                CurrentToken = notPassedString[0].ToString();
                NotPassedString = notPassedString.Remove(0, 1);
            }
            else
            {
                CurrentToken = "";
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
            try
            {
                Result = Executor.Execute(InputString);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, Lang.Errors_InvalidAutomaton, MessageBoxButton.OK);
            }
        }

        private bool CanRunCommandExecute(object p)
        {
            return !inSimulation && Executor != null;
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
        public string PassedString { get => passedString; set => Set(ref passedString, value); }

        private string currentToken = "";

        /// <summary>
        /// The token which an automaton is handling at the moment
        /// </summary>
        public string CurrentToken { get => currentToken; set => Set(ref currentToken, value); }

        private string notPassedString = "";

        /// <summary>
        /// The not handled by an automaton part of a string
        /// </summary>
        public string NotPassedString { get => notPassedString; set => Set(ref notPassedString, value); }

        private List<int> actualStates;

        /// <summary>
        /// The list of states ids that an automaton stays on at the moment
        /// </summary>
        public List<int> ActualStates { get => actualStates; set => Set(ref actualStates, value); }

        private ResultEnum result;

        /// <summary>
        /// The current result of an execution
        /// </summary>
        public ResultEnum Result
        {
            get => result;
            set
            {
                Set(ref result, value);
                OnPropertyChanged(nameof(StringResult));
            }
        }

        /// <summary>
        /// The result converted into the string type
        /// </summary>
        public string StringResult => ResultPrinter.PrintResult(Result);

        public ExecutorViewModel()
        {
            #region Commands

            StartDebugCommand = new RelayCommand(OnStartDebugCommandExecuted, CanStartDebugCommandExecute);
            DropDebugCommand = new RelayCommand(OnDropDebugCommandExecuted, CanDropDebugCommandExecute);
            StepInCommand = new RelayCommand(OnStepInCommandExecuted, CanStepInCommandExecute);
            RunCommand = new RelayCommand(OnRunCommandExecuted, CanRunCommandExecute);

            #endregion Commands
        }
    }
}