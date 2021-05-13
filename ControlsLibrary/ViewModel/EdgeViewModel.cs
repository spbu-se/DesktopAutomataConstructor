using ControlsLibrary.Infrastructure.Command;
using GraphX.Common.Models;
using GraphX.Measure;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using YAXLib;

namespace ControlsLibrary.ViewModel
{
    /// <summary>
    /// Contains edge data and methods to interact with it
    /// </summary>
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor without source and target vertices
        /// </summary>
        public EdgeViewModel()
            : base(null, null, 1)
        {
            IniitCommands();
        }

        /// <summary>
        /// Constructor which gets two vertices and symbols of transition
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        /// <param name="availableSymbols">Symbols of transition</param>
        public EdgeViewModel(NodeViewModel source, NodeViewModel target)
            : base(source, target, 1)
        {
            IniitCommands();
        }

        private void IniitCommands()
        {
            ChangeExpandingCommand = new RelayCommand(OnChangeExpandingCommandExecuted, CanChangeExpandingCommandExecute);
        }

        private bool editionAvailable = true;

        /// <summary>
        /// True if edge data can be edited
        /// </summary>
        [YAXDontSerialize]
        public bool EditionAvailable
        {
            get => editionAvailable;
            set
            {
                editionAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool isEpsilon;

        /// <summary>
        /// Contains data if transition is epsilon
        /// </summary>
        public bool IsEpsilon
        {
            get => isEpsilon;
            set
            {
                isEpsilon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TransitionTokens));
                OnPropertyChanged(nameof(TransitionTokensString));
            }
        }

        /// <summary>
        /// Changes expangind
        /// </summary>
        [YAXDontSerialize]
        public ICommand ChangeExpandingCommand { get; set; }

        private void OnChangeExpandingCommandExecuted(object p)
            => IsExpanded = !IsExpanded;

        private bool CanChangeExpandingCommandExecute(object p)
            => true;

        private bool isExpanded = false;

        /// <summary>
        /// Returns true if edge label is expanded
        /// </summary>
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }

        private string transitionTokensString = "";

        /// <summary>
        /// Transition tokens in the string form
        /// </summary>
        public string TransitionTokensString
        {
            get
            {
                return transitionTokensString;
            }
            set
            {
                if (!EditionAvailable)
                {
                    return;
                }
                transitionTokensString = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TransitionTokens));
            }
        }

        /// <summary>
        /// List of tokens to do transition
        /// </summary>
        [YAXDontSerialize]
        public ICollection<char> TransitionTokens
        {
            get
            {
                if (transitionTokensString == "")
                {
                    return new List<char>();
                }
                return transitionTokensString.ToList();
            }
        }

        /// <summary>
        /// Routing points
        /// </summary>
        public override Point[] RoutingPoints { get; set; }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}