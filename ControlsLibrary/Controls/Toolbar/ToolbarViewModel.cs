using ControlsLibrary.Model;
using ControlsLibrary.ViewModel.Base;
using System;

namespace ControlsLibrary.Controls.Toolbar
{
    public class ToolbarViewModel : BaseViewModel
    {
        private SelectedTool selectedTool;

        public SelectedTool SelectedTool 
        { 
            get => selectedTool;
            set
            {
                selectedTool = value;
                EditToolSelected = false;
                DeleteToolSelected = false;
                SelectToolSelected = false;
                switch (selectedTool)
                {
                    case SelectedTool.Select:
                        SelectToolSelected = true;
                        return;
                    case SelectedTool.Delete:
                        DeleteToolSelected = true;
                        return;
                    case SelectedTool.Edit:
                        EditToolSelected = true;
                        return;
                }
            }
        }

        public delegate void SelectedToolChangedEventHandler(object sender, EventArgs e);

        public event SelectedToolChangedEventHandler SelectedToolChanged;

        private bool selectToolSelected;
        private bool editToolSelected;
        private bool deleteToolSelected;

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
