using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.Controls.TypeAnalyzer;
using ControlsLibrary.FileSerialization;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.IO;
using YAXLib;
using System.Windows.Input;
using System.ComponentModel;

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
                var name = fileName == null || fileName == "" ? "(unsaved)" : fileName;
                var hasUnsavedChanges = saved ? "" : "*";
                return $"Automata constructor {name} {hasUnsavedChanges}";
            }
        }

        private TestPanelViewModel tests;

        private void NotifyTitleChanged() => PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.WindowTitle))
            );

        public event PropertyChangedEventHandler PropertyChanged;

        #region SaveAutomatAsCommand
        public static RoutedCommand SaveAutomatAsCommand { get; set; } = new RoutedCommand("SaveAutomatAs", typeof(MainWindow));

        private void OnSaveAutomatAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = "Select layout file name", FileName = "laytest.xml" };
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
            var dialog = new OpenFileDialog { Filter = "All files|*.*", Title = "Select layout file", FileName = "laytest.xml" };
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
                MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex));
            }
        }

        private void CanOpenAutomatCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        #endregion

        #region SaveTestsAsCommand
        public static RoutedCommand SaveTestsAsCommand { get; set; } = new RoutedCommand("SaveTestsAs", typeof(MainWindow));

        private void OnSaveTestsAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dialog.ShowDialog() == true)
            {
                var data = new List<TestSerializationData>();
                foreach (var test in tests.Tests)
                {
                    data.Add(new TestSerializationData() { Result = test.Result, ShouldReject = test.ShouldReject, TestString = test.TestString });
                }
                FileServiceProviderWpf.SerializeDataToFile(dialog.FileName, data);
            }
        }

        private void CanSaveTestsAsCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = tests.Tests != null && tests.Tests.Count > 0;
        #endregion

        #region OpenTests
        public static RoutedCommand OpenTestsCommand { get; set; } = new RoutedCommand("OpenTests", typeof(MainWindow));

        private void OnOpenTestsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            tests.Tests.Clear();
            var dialog = new OpenFileDialog { Filter = "All files|*.xml", Title = "Select tests file", FileName = "laytest.xml" };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                using (FileStream stream = File.Open(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var deserializer = new YAXSerializer(typeof(List<TestSerializationData>));
                    using (var textReader = new StreamReader(stream))
                    {
                        var datas = (List<TestSerializationData>)deserializer.Deserialize(textReader);
                        foreach (var test in datas)
                        {
                            tests.AddTest(test.Result, test.TestString, test.ShouldReject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex));
            }
        }

        private void CanOpenTestsCommandExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        #endregion

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!saved)
            {
                var result = MessageBox.Show("Save changes before closing?", "Automata constructor", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (scene != null && scene.CanSave() && File.Exists(savePath))
                    {
                        scene.Save(savePath);
                        return;
                    }

                    var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = "Select layout file name", FileName = "laytest.xml" };
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
