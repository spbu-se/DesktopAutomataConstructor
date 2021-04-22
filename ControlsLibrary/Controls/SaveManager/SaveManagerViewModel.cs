using ControlsLibrary.FileSerialization;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Controls.TestPanel;
using Microsoft.Win32;
using System.Windows.Input;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using YAXLib;
using GraphX.Common.Models;

namespace ControlsLibrary.Controls.SaveManager
{
    public class SaveManagerViewModel : BaseViewModel
    {
        public SaveManagerViewModel()
        {
            SaveLayoutCommand = new RelayCommand(OnSaveLayoutCommandExecuted, CanSaveLayoutCommandExecute);
            LoadLayoutCommand = new RelayCommand(OnLoadLayoutCommandExecuted, CanLoadLayoutCommandExecute);
            SaveTestsCommand = new RelayCommand(OnSaveTestsCommandExecuted, CanSaveTestsCommandExecute);
            LoadTestsCommand = new RelayCommand(OnLoadTestsCommandExecuted, CanLoadTestsCommandExecute);
        }

        public void GraphEdited(object sender, EventArgs e)
        {
            OnPropertyChanged("Graph");
        }

        private GraphArea graph;
        public GraphArea Graph 
        {
            get => graph;
            set
            {
                Set(ref graph, value);
            }
        }

        private ICollection<TestViewModel> tests;
        public ICollection<TestViewModel> Tests
        {
            get => tests;
            set => Set(ref tests, value);
        }

        public ICommand SaveLayoutCommand { get; set; }

        private void OnSaveLayoutCommandExecuted(object p)
        {
            var dlg = new SaveFileDialog { Filter = "All files|*.*", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dlg.ShowDialog() == true)
            {
                FileServiceProviderWpf.SerializeDataToFile(dlg.FileName + ".xml", graph.ExtractSerializationData());
            }
        }

        private bool CanSaveLayoutCommandExecute(object p) => graph != null && graph.LogicCore.Graph != null && graph.VertexList.Count > 0;


        public ICommand LoadLayoutCommand { get; set; }

        private void OnLoadLayoutCommandExecuted(object p)
        {
            var dialog = new OpenFileDialog { Filter = "All files|*.*", Title = "Select layout file", FileName = "laytest.xml" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var data = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(dialog.FileName);
                graph.RebuildFromSerializationData(data);
                graph.SetVerticesDrag(true, true);
                graph.UpdateAllEdges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex));
            }
        }

        private bool CanLoadLayoutCommandExecute(object p) => true;

        public ICommand SaveTestsCommand { get; set; }

        private void OnSaveTestsCommandExecuted(object p)
        {
            var dialog = new SaveFileDialog { Filter = "All files|*.xml", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dialog.ShowDialog() == true)
            {
                var data = new List<TestSerializationData>();
                foreach (var test in tests)
                {
                    data.Add(new TestSerializationData() { Result = test.Result, ShouldReject = test.ShouldReject, TestString = test.TestString });
                }
                FileServiceProviderWpf.SerializeDataToFile(dialog.FileName, data);
            }
        }

        private bool CanSaveTestsCommandExecute(object p) => tests != null && tests.Count > 0;

        public ICommand LoadTestsCommand { get; set; }

        private void OnLoadTestsCommandExecuted(object p)
        {
            tests.Clear();
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
                            tests.Add(new TestViewModel() { Result = test.Result, TestString = test.TestString, ShouldReject = test.ShouldReject, Storage = Tests, Graph = graph.LogicCore.Graph });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex));
            }
        }

        private bool CanLoadTestsCommandExecute(object p) => true;
    }
}
