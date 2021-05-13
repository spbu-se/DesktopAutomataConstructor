using AutomataConstructor.Properties.Langs;
using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Controls.TypeAnalyzer;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            NotifyTitleChanged();
            scene.Toolbar = (ToolbarViewModel)toolbar.DataContext;
            scene.ErrorReporter = (ErrorReporterViewModel)errorReporter.DataContext;
            scene.TypeAnalyzer = (TypeAnalyzerViewModel)typeAnalyzer.DataContext;
            scene.ExecutorViewModel = (ExecutorViewModel)executor.DataContext;
            scene.GraphEdited += HandleGraphEditions;
            tests = (TestPanelViewModel)testPanel.DataContext;
            scene.TestPanel = tests;
            this.KeyDown += scene.OnSceneKeyDown;
        }

        private void HandleGraphEditions(object sender, EventArgs e)
        {
            saved = false;
            NotifyTitleChanged();
        }

        private bool saved = true;

        private string savePath = "";

        private string fileName = "";

        public string WindowTitle
        {
            get
            {
                var name = fileName == null || fileName == "" ? $"({Lang.Saves_Unsaved})" : fileName;
                var hasUnsavedChanges = saved ? "" : "*";
                return $"{Lang.AutomataConstructor_Name} {name} {hasUnsavedChanges}";
            }
        }

        private TestPanelViewModel tests;

        private void NotifyTitleChanged() => PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(WindowTitle))
            );

        public event PropertyChangedEventHandler PropertyChanged;

        #region SaveAutomatAsCommand
        public static RoutedCommand SaveAutomatAsCommand { get; set; } = new RoutedCommand("SaveAutomatAs", typeof(MainWindow));

        private void OnSaveAutomatAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectAutomatonFileName, FileName = "automaton.xml" };
            if (dialog.ShowDialog() == true)
            {
                scene.Save(dialog.FileName);
                savePath = dialog.FileName;
                var splittedPath = dialog.FileName.Split(@"\");
                fileName = splittedPath[splittedPath.Length - 1];
                saved = true;
                NotifyTitleChanged();
            }
        }

        private void CanSaveAutomatAsCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = scene != null && scene.CanSave();
        #endregion

        #region SaveAutomatCommand
        public static RoutedCommand SaveAutomatCommand { get; set; } = new RoutedCommand("SaveAutomat", typeof(MainWindow));

        private void OnSaveAutomatCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scene.Save(savePath);
            saved = true;
            NotifyTitleChanged();
        }

        private void CanSaveAutomatCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = savePath != null && File.Exists(savePath) && scene != null && scene.CanSave();
        #endregion

        #region OpenAutomatCommand
        public static RoutedCommand OpenAutomatCommand { get; set; } = new RoutedCommand("OpenAutomat", typeof(MainWindow));

        private void OnOpenAutomatCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "All files|*.*", Title = Lang.Saves_SelectAutomatonFileName, FileName = "automaton.xml" };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                scene.Open(dialog.FileName);
                savePath = dialog.FileName;
                var splittedPath = dialog.FileName.Split(@"\");
                fileName = splittedPath[splittedPath.Length - 1];
                saved = true;
                NotifyTitleChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format($"{Lang.Saves_FailedToLoadAutomaton}\n {0}", ex));
            }
        }

        private void CanOpenAutomatCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        #endregion

        #region SaveTestsAsCommand
        public static RoutedCommand SaveTestsAsCommand { get; set; } = new RoutedCommand("SaveTestsAs", typeof(MainWindow));

        private void OnSaveTestsAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectAutomatonFileName, FileName = "automaton.xml" };
            if (dialog.ShowDialog() == true)
            {
                tests.Save(dialog.FileName);
            }
        }

        private void CanSaveTestsAsCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = tests.Tests != null && tests.Tests.Count > 0;
        #endregion

        #region OpenTests
        public static RoutedCommand OpenTestsCommand { get; set; } = new RoutedCommand("OpenTests", typeof(MainWindow));

        private void OnOpenTestsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectTestFileName, FileName = "tests.xml" };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                tests.Open(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Lang.Saves_FailedToLoadTests);
            }
        }

        private void CanOpenTestsCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        #endregion

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!saved)
            {
                var result = MessageBox.Show(Lang.Saves_Reminder, Lang.AutomataConstructor_Name, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (scene != null && scene.CanSave() && File.Exists(savePath))
                    {
                        scene.Save(savePath);
                        return;
                    }

                    var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectAutomatonFileName, FileName = "FSA.xml" };
                    if (dialog.ShowDialog() == true)
                    {
                        scene.Save(dialog.FileName);
                    }
                }
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
