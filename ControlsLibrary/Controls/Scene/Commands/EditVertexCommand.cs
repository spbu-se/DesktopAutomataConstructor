using ControlsLibrary.ViewModel;
using System;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene.Commands
{
    class EditVertexCommand
    {
        static Dictionary<string, Action<NodeViewModel, object>> setProperty =
            new Dictionary<string, Action<NodeViewModel, object>>
            {
                {nameof(NodeViewModel.IsInitial),
                    (nodeViewModel, value) => nodeViewModel.IsInitial = (bool)value },
                {nameof(NodeViewModel.IsFinal),
                    (nodeViewModel, value) => nodeViewModel.IsFinal = (bool)value },
                {nameof(NodeViewModel.Name),
                    (nodeViewModel, value) => nodeViewModel.Name = (string)value }
            };

        private readonly NodeViewModel nodeViewModel;
        private readonly string propertyName;
        private readonly object oldValue;
        private readonly object newValue;

        public EditVertexCommand(NodeViewModel nodeViewModel, string propertyName, object oldValue, object newValue)
        {
            this.nodeViewModel = nodeViewModel;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public bool CanBeUndone => true;

        public void Execute()
        {
            try
            {
                setProperty[propertyName](nodeViewModel, newValue);
            }
            catch (InvalidCastException) { }
        }

        public void Undo()
        {
            var command = new EditVertexCommand(nodeViewModel, propertyName, newValue, oldValue);
            command.Execute();
        }
    }
}

