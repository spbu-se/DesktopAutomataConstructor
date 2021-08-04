using ControlsLibrary.ViewModel;
using System;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene.Commands
{
    class EditEdgeCommand : ISceneCommand
    {
        static Dictionary<string, Action<EdgeViewModel, object>> setProperty =
            new Dictionary<string, Action<EdgeViewModel, object>>
            {
                {nameof(EdgeViewModel.IsEpsilon),
                    (edgeViewModel, value) => edgeViewModel.IsEpsilon = (bool)value },
                {nameof(EdgeViewModel.IsExpanded),
                    (edgeViewModel, value) => edgeViewModel.IsExpanded = (bool)value },
                {nameof(EdgeViewModel.TransitionTokensString),
                    (edgeViewModel, value) => edgeViewModel.TransitionTokensString = (string)value }                
            };

        private readonly EdgeViewModel edgeViewModel;
        private readonly string propertyName;
        private readonly object oldValue;
        private readonly object newValue;

        public EditEdgeCommand(EdgeViewModel edgeViewModel, string propertyName, object oldValue, object newValue)
        {
            this.edgeViewModel = edgeViewModel;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public bool CanBeUndone => true;

        public void Execute()
        {
            try
            {
                setProperty[propertyName](edgeViewModel, newValue);
            }
            catch (InvalidCastException) { }
        }

        public void Undo()
        {
            var command = new EditEdgeCommand(edgeViewModel, propertyName, newValue, oldValue);
            command.Execute();
        }
    }
}