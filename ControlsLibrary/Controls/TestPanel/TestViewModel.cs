using ControlsLibrary.Infrastructure.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ControlsLibrary.Controls.TestPanel
{
    internal class TestViewModel : INotifyPropertyChanged
    {
        private ICollection<TestViewModel> storage;

        public TestViewModel(ICollection<TestViewModel> storage)
        {
            this.storage = storage;
            RemoveFromStorageCommand = new RelayCommand(OnRemoveFromStorageCommandExecuted, CanRemoveFromStorageCommandExecute);
        }

        public ICommand RemoveFromStorageCommand { get; set; }

        private void OnRemoveFromStorageCommandExecuted(object p) => storage.Remove(this);

        private bool CanRemoveFromStorageCommandExecute(object p) => storage.Contains(this);

        private string testString;

        public string TestString
        {
            get => testString;
            set
            {
                testString = value;
                OnPropertyChanged();
            }
        }

        private TestResultEnum result;

        public TestResultEnum Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
                OnPropertyChanged(StringResult);
            }
        }

        public string StringResult
        {
            get
            {
                switch (Result)
                {
                    case TestResultEnum.NotRunned:
                        {
                            return "NotRunned";
                        }
                    case TestResultEnum.Failed:
                        {
                            return "Failed";
                        }
                    case TestResultEnum.Passed:
                        {
                            return "Passed";
                        }
                    default:
                        {
                            return "NotDefined";
                        }
                }
            }
        }

        private bool shouldReject;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShouldReject
        {
            get => shouldReject;
            set
            {
                shouldReject = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
