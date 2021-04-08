using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;

namespace ControlsLibrary.ViewModel
{
    public class AttributeViewModel : BaseViewModel
    {
        public AttributeViewModel(string name, TypeEnum type)
        {
            Name = name;
            this.type = type;
        }

        private string value;
        public string Value
        {
            get => value;
            set
            {
                Set(ref this.value, value);
            }
        }

        public string Name { get; }

        private TypeEnum type;
        public string Type 
        { 
            get
            {
                switch (type)
                {
                    case TypeEnum.Bool:
                        return "Boolean";
                    case TypeEnum.Int:
                        return "Int";
                    case TypeEnum.String:
                        return "String";
                }
                return "Missed";
            }
        }
    }
}
