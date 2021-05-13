using ControlsLibrary.Infrastructure.Command;
using GraphX.Common.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using YAXLib;

namespace ControlsLibrary.ViewModel
{
    /// <summary>
    /// Contains node on the scene data and methods to interact with it
    /// </summary>
    public class NodeViewModel : VertexBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Basic constructor
        /// </summary>
        public NodeViewModel()
        {
            ChangeHidingCommand = new RelayCommand(OnChangeHigingCommandExecuted, CanChangeHigingCommandExecute);
        }

        /// <summary>
        /// Changes hidding
        /// </summary>
        [YAXDontSerialize]
        public ICommand ChangeHidingCommand { get; set; }

        private bool CanChangeHigingCommandExecute(object p) => true;

        private void OnChangeHigingCommandExecuted(object p) => IsExpanded = !IsExpanded;

        private string name;
        private bool isInitial;
        private bool isFinal;
        private bool isExpanded;
        private bool isActual;
        private bool editionAvailable = true;

        /// <summary>
        /// Gets if properties editing availible
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

        /// <summary>
        /// Is node expanded to edit attributes
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

        /// <summary>
        /// Returns true if state is actual on the execution
        /// </summary>
        [YAXDontSerialize]
        public bool IsActual
        {
            get => isActual;
            set
            {
                isActual = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Name of the state
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is state initial
        /// </summary>
        public bool IsInitial
        {
            get => isInitial;
            set
            {
                isInitial = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is state final
        /// </summary>
        public bool IsFinal
        {
            get => isFinal;
            set
            {
                isFinal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalMarkVisibility));
            }
        }

        /// <summary>
        /// Returns is state final mark visibility
        /// </summary>
        public Visibility FinalMarkVisibility { get => IsFinal ? Visibility.Visible : Visibility.Hidden; }

        //TODO: Add bool to visibility converter and remove this property

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Overriding of the base method
        /// </summary>
        public override string ToString() => Name;
    }
}