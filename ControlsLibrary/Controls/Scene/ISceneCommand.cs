namespace ControlsLibrary.Controls.Scene
{
    public interface ISceneCommand
    {
        public bool CanBeUndone { get; }
        public void Execute();
        public void Undo();
    }
}
