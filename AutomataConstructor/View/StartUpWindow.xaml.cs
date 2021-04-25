using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomataConstructor.View
{
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        public StartUpWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().ShowDialog();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (langageComboBox.SelectedIndex)
            {
                case 0:
                    {
                        Properties.Settings.Default.language = "en-US";
                        break;
                    }
                case 1:
                    {
                        Properties.Settings.Default.language = "ru-Ru";
                        break;
                    }
            }

            Properties.Settings.Default.Save();
        }
    }
}
