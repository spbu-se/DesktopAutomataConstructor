using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;
using ControlsLibrary.Infrastructure.Command;
using System.Windows.Input;

namespace ControlsLibrary.Controls.Toolbox
{
    public class ToolboxViewModel : BaseViewModel
    {
        private SelectedTool selectedTool;

        public SelectedTool SelectedTool { get => selectedTool; set => Set(ref selectedTool, value); }

        public ICommand SelectSelectToolCommand { get; }

        private void OnSelectSelectToolCommmandExecuted(object p) => SelectedTool = SelectedTool.Select;

        public ICommand SelectEditToolCommand { get; }

        private void OnSelectEditToolCommmandExecuted(object p) => SelectedTool = SelectedTool.Edit;

        private bool CanSelectToolCommandExecute(object p) => true;

        public ICommand SelectEditAttributesToolCommand { get; }

        private void OnSelectEditAttributesToolCommmandExecuted(object p) => SelectedTool = SelectedTool.Select;

        public ICommand SelectDeleteToolCommand { get; }

        private void OnSelectDeleteToolCommmandExecuted(object p) => SelectedTool = SelectedTool.Select;

        public ToolboxViewModel()
        {
            SelectSelectToolCommand = new RelayCommand(OnSelectSelectToolCommmandExecuted, CanSelectToolCommandExecute);
            SelectEditToolCommand = new RelayCommand(OnSelectEditToolCommmandExecuted, CanSelectToolCommandExecute);
            SelectEditAttributesToolCommand = new RelayCommand(OnSelectEditAttributesToolCommmandExecuted, CanSelectToolCommandExecute);
            SelectDeleteToolCommand = new RelayCommand(OnSelectDeleteToolCommmandExecuted, CanSelectToolCommandExecute);
        }

        public bool IsSelectToolChosen { get => SelectedTool == SelectedTool.Select; }

        public bool IsEditToolChosen { get => SelectedTool == SelectedTool.Edit; }

        public bool IsDeleteToolChosen { get => SelectedTool == SelectedTool.Delete; }

        public bool IsEditAttributesToolChosen { get => SelectedTool == SelectedTool.EditAttributes; }
    }
}
