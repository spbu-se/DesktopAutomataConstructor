using ControlsLibrary.Controls.AttributesPanel;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.Controls.Toolbar;
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
            scene.NodeSelected += (sender, args) => AttributesPanel.Attributes = args.VertexControl.Attributes;
        }

        public AttributesPanelViewModel AttributesPanel { get; } = new AttributesPanelViewModel();


    }
}
