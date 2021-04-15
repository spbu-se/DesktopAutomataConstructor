﻿using ControlsLibrary.Infrastructure.Command;
using GraphX.Common.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ControlsLibrary.Model
{
    public class NodeViewModel : VertexBase, INotifyPropertyChanged
    {
        public NodeViewModel()
        {
            ChangeHidingCommand = new RelayCommand(OnChangeHigingCommandExecuted, CanChangeHigingCommandExecute);
        }

        public ICommand ChangeHidingCommand { get; set; }

        private bool CanChangeHigingCommandExecute(object p) => true;

        private void OnChangeHigingCommandExecuted(object p) => IsExpanded = !IsExpanded;

        private string name;
        private bool isInitial;
        private bool isFinal;
        private bool isExpanded;

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
            }
        }

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
