using ControlsLibrary.FileSerialization;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.ViewModel.Base;
using Microsoft.Win32;
using System.Windows.Input;
using System;

namespace ControlsLibrary.Controls.SaveManager
{
    public class SaveManagerViewModel : BaseViewModel
    {
        public SaveManagerViewModel()
        {
            SaveLayoutCommand = new RelayCommand(OnSaveLayoutCommandExecuted, CanSaveLayoutCommandExecute);
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
                FileServiceProviderWpf.SerializeDataToFile(dlg.FileName, graph.ExtractSerializationData());
            }
        }

        private bool CanSaveLayoutCommandExecute(object p) => graph != null && graph.LogicCore.Graph != null && graph.VertexList.Count > 0;
    }
}
