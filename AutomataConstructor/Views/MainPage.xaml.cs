using System;

using AutomataConstructor.ViewModels;

using Windows.UI.Xaml.Controls;

namespace AutomataConstructor.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
