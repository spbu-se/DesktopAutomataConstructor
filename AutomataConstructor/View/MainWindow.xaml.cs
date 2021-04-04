using ControlsLibrary.Controls.AttributesPanel;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            scene.Toolbar = (ToolbarViewModel)toolbar.DataContext;
            attributes.DataContext = AttributesPanel;
            scene.NodeSelected += (sender, args) => AttributesPanel.Attributes = args.Node.Attributes;
        }

        public AttributesPanelViewModel AttributesPanel { get; } = new AttributesPanelViewModel();
    }
}
