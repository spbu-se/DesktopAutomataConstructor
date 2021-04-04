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
            //scene.NodeSelected += (sender, args) => AttributesPanel.Attributes = args.Node.Attributes;
            attributes.DataContext = AttributesPanel;
            var list = new List<AttributeViewModel>();
            list.Add(new AttributeViewModel("length", ControlsLibrary.Model.TypeEnum.Int));
            AttributesPanel.Attributes = list;
        }

        public AttributesPanelViewModel AttributesPanel { get; } = new AttributesPanelViewModel();
    }
}
