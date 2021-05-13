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
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
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
                var name = string.IsNullOrEmpty(fileName) ? $"({Lang.Saves_Unsaved})" : fileName;
                var hasUnsavedChanges = saved ? "" : "*";
                return $"{Lang.AutomataConstructor_Name} {name} {hasUnsavedChanges}";
            }
        }

        private readonly TestPanelViewModel tests;

        private void NotifyTitleChanged() => PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(WindowTitle))
            );

        public event PropertyChangedEventHandler PropertyChanged;

        #region SaveAutomatonAsCommand

        public static RoutedCommand SaveAutomatonAsCommand { get; set; } = new RoutedCommand("SaveAutomatAs", typeof(MainWindow));

        private void OnSaveAutomatonAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectAutomatonFileName, FileName = "automaton.xml" };
            if (dialog.ShowDialog() == true)
            {
                scene.Save(dialog.FileName);
                savePath = dialog.FileName;
                var splittedPath = dialog.FileName.Split(@"\");
                fileName = splittedPath[^1];
                saved = true;
                NotifyTitleChanged();
            }
        }

        private void CanSaveAutomatonAsCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = scene != null && scene.CanSave();

        #endregion SaveAutomatonAsCommand

        #region SaveAutomatonCommand

        public static RoutedCommand SaveAutomatonCommand { get; set; } = new RoutedCommand("SaveAutomat", typeof(MainWindow));

        private void OnSaveAutomatonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scene.Save(savePath);
            saved = true;
            NotifyTitleChanged();
        }

        private void CanSaveAutomatonCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = savePath != null && File.Exists(savePath) && scene != null && scene.CanSave();

        #endregion SaveAutomatonCommand

        #region OpenAutomatonCommand

        public static RoutedCommand OpenAutomatonCommand { get; set; } = new RoutedCommand("OpenAutomat", typeof(MainWindow));

        private void OnOpenAutomatonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
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
                fileName = splittedPath[^1];
                saved = true;
                NotifyTitleChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format($"{Lang.Saves_FailedToLoadAutomaton}\n {0}", ex));
            }
        }

        private void CanOpenAutomatonCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        #endregion OpenAutomatonCommand

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
            => e.CanExecute = tests.Tests != null && tests.Tests.Any();

        #endregion SaveTestsAsCommand

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
            catch (Exception)
            {
                MessageBox.Show(Lang.Saves_FailedToLoadTests);
            }
        }

        private void CanOpenTestsCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        #endregion OpenTests

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (saved)
            {
                return;
            }

            switch (MessageBox.Show(Lang.Saves_Reminder, Lang.AutomataConstructor_Name, MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.Yes:
                    {
                        if (scene != null && scene.CanSave() && File.Exists(savePath))
                        {
                            scene.Save(savePath);
                            return;
                        }

                        var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = Lang.Saves_SelectAutomatonFileName, FileName = "FSA.xml" };
                        if (dialog.ShowDialog() == true)
                        {
                            scene?.Save(dialog.FileName);
                        }

                        return;
                    }
                case MessageBoxResult.Cancel:
                    {
                        e.Cancel = true;
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }
}