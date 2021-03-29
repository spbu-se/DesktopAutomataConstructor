using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;
using ControlsLibrary.Infrastructure.Command;
using System.Windows.Input;

namespace ControlsLibrary.Controls.Toolbar
{
    public class ToolbarViewModel : BaseViewModel
    {
        private SelectedTool selectedTool;

        public SelectedTool SelectedTool { get => selectedTool; set => Set(ref selectedTool, value); }

        

        public ToolbarViewModel()
        {
        }
    }
}
