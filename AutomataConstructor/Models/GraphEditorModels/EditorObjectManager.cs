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
        private EdgeBlueprint edgeBlueprint;

        public EditorObjectManager(DFAGraphArea graphArea, ZoomControl zoomControl)
        {
            this.graphArea = graphArea;
            this.zoomControl = zoomControl;
            resourceDictionary = new ResourceDictionary();
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
