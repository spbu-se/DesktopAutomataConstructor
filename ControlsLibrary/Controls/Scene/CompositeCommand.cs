using System.Collections.Generic;
using System.Linq;

namespace ControlsLibrary.Controls.Scene
{
    public class CompositeCommand : ISceneCommand
    {
        private IList<ISceneCommand> commands;

        public CompositeCommand(IList<ISceneCommand> commands)
        {
            this.commands = commands;
        }
        public void Execute()
        {
            foreach (var command in commands)
            {
                command.Execute();
            }
        }

        public bool CanBeUndone => commands.All(c => c.CanBeUndone);

        public void Undo()
        {
            if (CanBeUndone)
            {
                foreach (var command in commands.Reverse())
                {
                    command.Undo();
                }
            }
        }

        public void AddCommand(ISceneCommand command)
        {
            this.commands.Add(command);
        }
    }
}
