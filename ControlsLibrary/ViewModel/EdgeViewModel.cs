using GraphX.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ControlsLibrary.Model
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor which gets two vertices and symbols of transition
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        /// <param name="availableSymbols">Symbols of transition</param>
        public EdgeViewModel(NodeViewModel source, NodeViewModel target)
            : base(source, target, 1)
        {
        }

        private string transitionTokensString;

        public string TransitionTokensString
        {
            get => transitionTokensString;
            set
            {
                transitionTokensString = value;
                OnPropertyChanged();
            }
        }

        public List<char> TransitionTokens { get => transitionTokensString.ToList(); }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Overriding of the base method
        /// </summary>
        public override string ToString() => TransitionTokens.Aggregate("", (str, acc) => acc + str);
    }
}
