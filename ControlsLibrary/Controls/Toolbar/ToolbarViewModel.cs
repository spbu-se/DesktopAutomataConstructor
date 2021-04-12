using ControlsLibrary.Model;
using ControlsLibrary.ViewModel.Base;
using System;

namespace ControlsLibrary.Controls.Toolbar
{
    public class ToolbarViewModel : BaseViewModel
    {
        private SelectedTool selectedTool;

        public SelectedTool SelectedTool { get => selectedTool; }

        public delegate void SelectedToolChangedEventHandler(object sender, EventArgs e);

        public event SelectedToolChangedEventHandler SelectedToolChanged;

        private bool selectToolSelected;
        private bool editToolSelected;
        private bool deleteToolSelected;
        private bool editAttributesToolSelected;

        public bool SelectToolSelected
        {
            get => selectToolSelected;
            set
            {
                Set(ref selectToolSelected, value);
                if (value)
                {
                    EditToolSelected = false;
                    DeleteToolSelected = false;
                    selectedTool = SelectedTool.Select;
                    SelectedToolChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool EditToolSelected
        {
            get => editToolSelected;
            set
            {
                Set(ref editToolSelected, value);
                if (value)
                {
                    SelectToolSelected = false;
                    DeleteToolSelected = false;
                    selectedTool = SelectedTool.Edit;
                    SelectedToolChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool DeleteToolSelected
        {
            get => deleteToolSelected;
            set
            {
                Set(ref deleteToolSelected, value);
                if (value)
                {
                    SelectToolSelected = false;
                    EditToolSelected = false;
                    selectedTool = SelectedTool.Delete;
                    SelectedToolChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
