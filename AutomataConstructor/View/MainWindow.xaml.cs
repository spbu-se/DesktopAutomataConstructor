using ControlsLibrary.Controls.ErrorReporter;
using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Controls.TestPanel;
using ControlsLibrary.FileSerialization;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using GraphX.Common.Models;
using System;
using System.IO;
using YAXLib;
using System.Windows.Input;
using ControlsLibrary.Infrastructure.Command;

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
            scene.ErrorReporter = (ErrorReporterViewModel)errorReporter.DataContext;
            scene.ExecutorViewModel = (ExecutorViewModel)executor.DataContext;
            tests = (TestPanelViewModel)testPanel.DataContext;
            scene.TestPanel = tests;
            this.KeyDown += scene.OnSceneKeyDown;
        }

        private TestPanelViewModel tests;

        private string savePath = "";

        #region SaveAutomatAsCommand
        public static RoutedCommand SaveAutomatAsCommand { get; set; } = new RoutedCommand("SaveAutomatAs", typeof(MainWindow));

        private void OnSaveAutomatAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dialog.ShowDialog() == true)
            {
                FileServiceProviderWpf.SerializeDataToFile(dialog.FileName, scene.GraphArea.ExtractSerializationData());
                savePath = dialog.FileName;
            }
        }

        private void CanSaveAutomatAsCommandExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = scene.GraphArea != null && scene.GraphArea.LogicCore.Graph != null && scene.GraphArea.VertexList.Count > 0;
        #endregion

        #region SaveAutomatCommand
        public static RoutedCommand SaveAutomatCommand { get; set; } = new RoutedCommand("SaveAutomat", typeof(MainWindow));

        private void OnSaveAutomatCommandExecuted(object sender, ExecutedRoutedEventArgs e)
            => FileServiceProviderWpf.SerializeDataToFile(savePath, scene.GraphArea.ExtractSerializationData());

        private void CanSaveAutomatCommand(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = savePath != null && File.Exists(savePath) && scene.GraphArea != null && scene.GraphArea.LogicCore.Graph != null && scene.GraphArea.VertexList.Count > 0; 
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
                var data = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(dialog.FileName);
                scene.GraphArea.RebuildFromSerializationData(data);
                scene.GraphArea.SetVerticesDrag(true, true);
                scene.GraphArea.UpdateAllEdges();
                savePath = dialog.FileName;
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
                            tests.Tests.Add(new TestViewModel()
                            {
                                Result = test.Result,
                                TestString = test.TestString,
                                ShouldReject = test.ShouldReject,
                                Storage = tests.Tests,
                                Graph = scene.GraphArea.LogicCore.Graph
                            });
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
    }
}
