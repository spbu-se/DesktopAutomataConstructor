using ControlsLibrary.FileSerialization;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel.Base;
using Microsoft.Win32;
using System.Windows.Input;
using System;
using System.Windows;

namespace ControlsLibrary.Controls.SaveManager
{
    public class SaveManagerViewModel : BaseViewModel
    {
        public SaveManagerViewModel()
        {
            SaveLayoutCommand = new RelayCommand(OnSaveLayoutCommandExecuted, CanSaveLayoutCommandExecute);
            LoadLayoutCommand = new RelayCommand(OnLoadLayoutCommandExecuted, CanLoadLayoutCommandExecute);
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
            var dlg = new OpenFileDialog { Filter = "All files|*.*", Title = "Select layout file", FileName = "laytest.xml" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                graph.RebuildFromSerializationData(FileServiceProviderWpf.DeserializeDataFromFile(dlg.FileName));
                graph.SetVerticesDrag(true, true);
                graph.UpdateAllEdges();
                //gg_zoomctrl.ZoomToFill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex));
            }
        }

        private bool CanLoadLayoutCommandExecute(object p) => true;
    }
}
