using AutomataConstructor.Models.GraphModels;
using GraphX.Controls;
using System;
using System.Windows;

namespace AutomataConstructor.Models.GraphEditorModels
{
    internal class EditorObjectManager : IDisposable
    {
        private readonly DFAGraphArea graphArea;
        private readonly ZoomControl zoomControl;
        private readonly ResourceDictionary resourceDictionary;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
