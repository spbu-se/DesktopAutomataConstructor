using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;
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
                selectedTool = SelectedTool.Select;
                Set(ref selectToolSelected, value);
                if (value)
                {
                    EditToolSelected = false;
                    DeleteToolSelected = false;
                    EditAttributesToolSelected = false;
                }
            }
        }

        public bool EditToolSelected
        {
            get => editToolSelected;
            set
            {
                selectedTool = SelectedTool.Edit;
                Set(ref editToolSelected, value);
                if (value)
                {
                    SelectToolSelected = false;
                    DeleteToolSelected = false;
                    EditAttributesToolSelected = false;
                }
            }
        }

        public bool DeleteToolSelected
        {
            get => deleteToolSelected;
            set
            {
                selectedTool = SelectedTool.Delete;
                Set(ref deleteToolSelected, value);
                if (value)
                {
                    SelectToolSelected = false;
                    EditToolSelected = false;
                    EditAttributesToolSelected = false;
                }
            }
        }

        public bool EditAttributesToolSelected
        {
            get => editAttributesToolSelected;
            set
            {
                selectedTool = SelectedTool.EditAttributes;
                Set(ref editAttributesToolSelected, value);
                if (value)
                {
                    SelectToolSelected = false;
                    EditToolSelected = false;
                    DeleteToolSelected = false;
                }
            }
        }
    }
}
