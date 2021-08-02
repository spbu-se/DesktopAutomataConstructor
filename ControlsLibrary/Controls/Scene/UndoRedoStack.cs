using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene
{
    class UndoRedoStack
    {
        private readonly Stack<ISceneCommand> undoStack;
        private readonly Stack<ISceneCommand> redoStack;

        private bool isUndoAvailable;
        private bool isRedoAvailable;

        public UndoRedoStack()
        {
            undoStack = new Stack<ISceneCommand>();
            redoStack = new Stack<ISceneCommand>();
            isUndoAvailable = true;
            isRedoAvailable = true;
        }
        public void AddCommand(ISceneCommand sceneCommand)
        {
            if (sceneCommand.CanBeUndone)
            {
                undoStack.Push(sceneCommand);
                redoStack.Clear();
            }
        }

        public void SetUndoAvailable(bool available)
        {
            isUndoAvailable = available;
        }

        public void SetRedoAvailable(bool available)
        {
            isRedoAvailable = available;
        }

        public bool IsUndoAvailable => this.isUndoAvailable && this.undoStack.Count > 0;

        public bool IsRedoAvailable => this.isRedoAvailable && this.redoStack.Count > 0;

        public void Undo()
        {
            if (IsUndoAvailable)
            {
                var sceneCommand = undoStack.Pop();
                sceneCommand.Undo();
                redoStack.Push(sceneCommand);
            }
        }

        public void Redo()
        {
            if (IsRedoAvailable)
            {
                var sceneCommand = redoStack.Pop();
                sceneCommand.Execute();
                undoStack.Push(sceneCommand);
            }
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
    }
}
