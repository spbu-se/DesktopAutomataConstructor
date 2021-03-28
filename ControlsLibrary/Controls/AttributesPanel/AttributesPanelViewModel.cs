using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.ViewModel;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.AttributesPanel
{
    public class AttributesPanelViewModel : BaseViewModel
    {
        public AttributesPanelViewModel()
        {
            attributes = new List<AttributeViewModel>();
        }

        private IList<AttributeViewModel> attributes;

        public IList<AttributeViewModel> Attributes
        {
            get => attributes;
            set => Set(ref attributes, value);
        }
    }
}
