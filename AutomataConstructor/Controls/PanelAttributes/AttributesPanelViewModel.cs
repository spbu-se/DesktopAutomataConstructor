using AutomataConstructor.ViewModels.Base;
using System.Collections.Generic;

namespace AutomataConstructor.Controls.PanelAttributes
{
    public class AttributesPanelViewModel : ViewModel
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
