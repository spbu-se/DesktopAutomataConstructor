using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;

namespace ControlsLibrary.ViewModel
{
    public class AttributeViewModel : BaseViewModel
    {
        public AttributeViewModel(string name, TypeEnum type)
        {
            Name = name;
            Type = type;
        }

        private string value;
        public string Value
        {
            get => value;
            set => Set(ref value, value);
        }

        public string Name { get; }

        public TypeEnum Type { get; }
    }
}
