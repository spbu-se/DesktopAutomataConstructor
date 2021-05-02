using ControlsLibrary.Infrastructure.Command;
using GraphX.Common.Models;
using GraphX.Measure;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using YAXLib;

namespace ControlsLibrary.Model
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged
    {
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

        public bool IsEpsilon
        {
            get => isEpsilon;
            set
            {
                isEpsilon = value;
                OnPropertyChanged();
                OnPropertyChanged("TransitionTokens");
                OnPropertyChanged("TransitionTokensString");
            }
        }

        [YAXDontSerialize]
        public ICommand ChangeExpandingCommand { get; set; }

        private void OnChangeExpandingCommandExecuted(object p)
            => IsExpanded = !IsExpanded;

        private bool CanChangeExpandingCommandExecute(object p)
            => true;

        private bool isExpanded = false;

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
                OnPropertyChanged("TransitionTokens");
            }
        }

        [YAXDontSerialize]
        public List<char> TransitionTokens
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

        public override Point[] RoutingPoints { get; set; }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
