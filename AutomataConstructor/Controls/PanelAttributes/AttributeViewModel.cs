using AutomataConstructor.ViewModels.Base;

namespace AutomataConstructor.Controls.PanelAttributes
{
    public class AttributeViewModel : ViewModel
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
