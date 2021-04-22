using AutomataConstructor.FileSerialization;
using AutomataConstructor.Infrastructure.Command;
using ControlsLibrary.Controls.Scene;
using Microsoft.Win32;
using System.Windows.Input;

namespace AutomataConstructor.ViewModels
{
    internal class SaveManagerViewModel
    {
        public SaveManagerViewModel()
        {
            SaveLayoutCommand = new RelayCommand(OnSaveLayoutCommandExecuted, CanSaveLayoutCommandExecute);
        }

        private GraphArea graph;
        public GraphArea Graph { get; set; }

        public ICommand SaveLayoutCommand { get; set; }

        private void OnSaveLayoutCommandExecuted(object p)
        {
            var dlg = new SaveFileDialog { Filter = "All files|*.*", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dlg.ShowDialog() == true)
            {
                FileServiceProviderWpf.SerializeDataToFile(dlg.FileName, graph.ExtractSerializationData());
            }
        }

        private bool CanSaveLayoutCommandExecute(object p) => graph.LogicCore.Graph != null && graph.VertexList.Count > 0;

    }
}
