using ControlsLibrary.ViewModel.Base;

namespace ControlsLibrary.Controls.TestPanel
{
    internal class TestViewModel : BaseViewModel
    {
        private string testString;

        public string TestString
        {
            get => testString;
            set => Set(ref testString, value);
        }

        private TestResultEnum result;

        public TestResultEnum Result
        {
            get => result;
            set => Set(ref result, value);
        }

        private bool shouldReject;

        public bool ShouldReject
        {
            get => shouldReject;
            set => Set(ref shouldReject, value);
        }
    }
}
