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
        /// Constructor aimed only for deserialization
        /// </summary>
        public EdgeViewModel()
            : base(null, null, 1)
        {
            InitCommands();
        }

        /// <summary>
        /// Constructor which gets two vertices and symbols of transition
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        public EdgeViewModel(NodeViewModel source, NodeViewModel target)
            : base(source, target)
        {
            InitCommands();
        }

        private void InitCommands()
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
        /// Changes expanding
        /// </summary>
        [YAXDontSerialize]
        public ICommand ChangeExpandingCommand { get; set; }

        private void OnChangeExpandingCommandExecuted(object p)
            => IsExpanded = !IsExpanded;

        private bool CanChangeExpandingCommandExecute(object p)
            => true;

        private bool isExpanded;

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
            get => transitionTokensString;
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
        public ICollection<char> TransitionTokens => transitionTokensString == "" ? new List<char>() : transitionTokensString.ToList();

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