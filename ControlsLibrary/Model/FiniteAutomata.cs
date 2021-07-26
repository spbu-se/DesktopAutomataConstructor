using ControlsLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlsLibrary.Model
{
    public class FiniteAutomata
    {
        public static readonly int ERROR_STATE = -1;
        public static readonly int EPSILON = 0;

        public static int ConvertToInt(char x, List<char> alp)
        {
            int y = alp.IndexOf(x) + 1;
            if (y > 0)
            {
                return y;
            }
            else return ERROR_STATE;
        }

        private readonly Dictionary<int, List<int>[]> _transitionTable;
        private List<int> _initialStates;
        private List<int> _acceptingStates;
        private List<int> _currentStates;
        private string _str;
        private int _strPosition;
        public ResultEnum StepResult;
        private bool hasErrorState;
        public bool HasErrorState { get => hasErrorState; }
        public List<char> Alphabet { get; }

        public static List<string> GetDefaultStatNames(int n)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < n; i++)
            {
                string x = "Q" + n.ToString();
                list.Add(x);
            }
            return list;
        }

        private List<int> GetNewStatesFromSingleState(int state, char x)
        {
            if (ConvertToInt(x, Alphabet) != ERROR_STATE)
            {
                if (_transitionTable[state][ConvertToInt(x, Alphabet)] != null)
                {
                    if (_transitionTable[state][ConvertToInt(x, Alphabet)].Count == 0)
                    {
                        hasErrorState = true;
                    }
                    return _transitionTable[state][ConvertToInt(x, Alphabet)];
                }
                else
                {
                    hasErrorState = true;
                    return new List<int>();
                }
            }
            else
            {
                hasErrorState = true;
                return new List<int>();
            }
        }

        public List<int> GetAllNewStates(List<int> states, char x)
        {
            List<int> result_list = new List<int>() { };
            foreach (int state in states)
            {
                result_list = result_list.Union(GetNewStatesFromSingleState(state, x)).ToList();
            }
            return result_list;
        }

        public List<int> EpsilonClosure(List<int> states)
        {
            List<int> allStates = states;
            List<int> progressStates = new List<int>();
            while (true)
            {
                progressStates = allStates;
                foreach (int n in allStates)
                {
                    progressStates = progressStates.Union(_transitionTable[n][EPSILON]).ToList();
                }
                if (progressStates.Count == allStates.Count)
                {
                    return allStates;
                }
                allStates = progressStates;
            }
        }

        private void DoSingleTransition(char x)
        {
            List<int> newCurrentStates = GetAllNewStates(_currentStates, x);

            _currentStates = EpsilonClosure(newCurrentStates);
        }

        private void ResetAutomata()
        {
            _currentStates = EpsilonClosure(_initialStates);
            hasErrorState = false;
            StepResult = ResultEnum.NotRunned;
        }

        public void SetString(string str)
        {
            _str = str;
            _strPosition = 0;
            this.ResetAutomata();
        }

        private bool HasAccepingtState(List<int> states)
        {
            foreach (int state in states)
            {
                if (_acceptingStates.Contains(state))
                {
                    return true;
                }
            }
            return false;
        }

        public void SingleStep()
        {
            this.DoSingleTransition(_str[_strPosition]);
            _strPosition++;
            if (_strPosition == _str.Length)
            {
                if (HasAccepingtState(_currentStates))
                {
                    StepResult = ResultEnum.Passed;
                }
                else
                {
                    StepResult = ResultEnum.Failed;
                }
            }
        }

        public bool CanDoStep()
        {
            if (_str.Length > _strPosition)
            {
                return true;
            }
            return false;
        }

        public bool DoAllTransitions(String str)
        {
            ResetAutomata();
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    DoSingleTransition(str[i]);
                }
            }
            foreach (int state in _currentStates)
            {
                if (_acceptingStates.Contains(state))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDeterminated()
        {
            foreach (KeyValuePair<int, List<int>[]> keyValue in _transitionTable)
            {
                for (int j = 0; j < _transitionTable[keyValue.Key].Length; j++)
                {
                    if (_transitionTable[keyValue.Key][j].Count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public FiniteAutomata(List<char> alphabet, Dictionary<int, List<int>[]> transitionTable, List<int> numOfInitialStates, List<int> acceptingStates)
        {
            this.Alphabet = alphabet;
            this._transitionTable = transitionTable;
            this._acceptingStates = acceptingStates;
            this._initialStates = numOfInitialStates;
            this._currentStates = _initialStates;
            _str = "";
            _strPosition = 0;
            hasErrorState = false;
        }

        public List<int> GetCurrentStates()
        {
            return _currentStates;
        }

        public static FiniteAutomata AutomataABC()
        {
            List<char> alph = new List<char>() { 'a', 'b', 'c' };
            Dictionary<int, List<int>[]> table = new Dictionary<int, List<int>[]>();
            table.Add(0, new List<int>[] { new List<int>(), new List<int>(), new List<int>() });
            table.Add(1, new List<int>[] { new List<int>(), new List<int>(), new List<int>() });
            table.Add(2, new List<int>[] { new List<int>(), new List<int>(), new List<int>() });
            table[0][ConvertToInt('a', alph)] = new List<int>() { 0 };
            table[0][ConvertToInt('b', alph)] = new List<int>() { 1 };
            table[0][ConvertToInt('c', alph)] = new List<int>() { 2 };
            table[1][ConvertToInt('b', alph)] = new List<int>() { 1 };
            table[1][ConvertToInt('c', alph)] = new List<int>() { 2 };
            table[2][ConvertToInt('c', alph)] = new List<int>() { 2 };
            return new FiniteAutomata(alph, table, new List<int>() { 0 }, new List<int>() { 0, 1, 2 });
        }

        public static FiniteAutomata ConvertGraphToAutomata(List<EdgeViewModel> edges, List<NodeViewModel> nodes)
        {
            string allSigns = "";
            foreach (var edge in edges)
            {
                allSigns += edge.TransitionTokensString;
            }
            List<char> automataAlphabet = new List<char>();
            foreach (var element in allSigns.ToCharArray())
            {
                if (!automataAlphabet.Contains(element))
                {
                    automataAlphabet.Add(element);
                }
            }
            Dictionary<int, List<int>[]> table = new Dictionary<int, List<int>[]>();
            List<int> acceptingStates = new List<int>();
            List<int> initialStates = new List<int>();

            foreach (var node in nodes)
            {
                int nodeID = (int)node.ID;
                if (node.IsInitial)
                {
                    initialStates.Add(nodeID);
                }
                if (node.IsFinal)
                {
                    acceptingStates.Add(nodeID);
                }
                table.Add(nodeID, new List<int>[automataAlphabet.Count + 1]);
                for (int i = 0; i < table[nodeID].Length; i++)
                {
                    table[nodeID][i] = new List<int>();
                }
            }
            foreach (var edge in edges)
            {
                foreach (var element in edge.TransitionTokensString.ToCharArray())
                {
                    table[(int)edge.Source.ID][FiniteAutomata.ConvertToInt(element, automataAlphabet)].Add((int)edge.Target.ID);
                }
                if (edge.IsEpsilon)
                {
                    table[(int)edge.Source.ID][EPSILON].Add((int)edge.Target.ID);
                }
            }
            return new FiniteAutomata(automataAlphabet, table, initialStates, acceptingStates);
        }
    }
}