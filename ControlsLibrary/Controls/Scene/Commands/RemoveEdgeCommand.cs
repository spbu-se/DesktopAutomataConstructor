﻿using ControlsLibrary.ViewModel;
using GraphX.Controls;
using System.Linq;

namespace ControlsLibrary.Controls.Scene.Commands
{
    class RemoveEdgeCommand : ISceneCommand
    {
        private readonly GraphArea graphArea;
        private EdgeControl edgeControl; 
        public RemoveEdgeCommand(GraphArea graphArea, EdgeControl edgeControl)
        {
            this.graphArea = graphArea;
            this.edgeControl = edgeControl;
        }
        public bool CanBeUndone => true;

        public void Execute()
        {
            var data = edgeControl.Edge as EdgeViewModel;
            var source = data.Source;
            var target = data.Target;
            var copy = new EdgeControl(edgeControl.Source, edgeControl.Target, data);
            graphArea.RemoveEdge(data, true);
            UpdateEdgeRoutingPoints(source, target);
            edgeControl = copy;
        }

        public void Undo()
        {
            var copy = new EdgeControl(edgeControl.Source, edgeControl.Target, 
                edgeControl.Edge as EdgeViewModel);
            var command = new CreateEdgeCommand(graphArea, edgeControl);
            command.Execute();
            edgeControl = copy;
        }

        private void UpdateEdgeRoutingPoints(NodeViewModel source, NodeViewModel target)
        {
            var parallelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == target && e.Target == source);
            if (parallelEdge == null)
            {
                return;
            }

            var newEdge = new EdgeViewModel(parallelEdge.Source, parallelEdge.Target) { TransitionTokensString = parallelEdge.TransitionTokensString };
            var ec = new EdgeControl(graphArea.VertexList[parallelEdge.Source], graphArea.VertexList[parallelEdge.Target], newEdge);
            graphArea.RemoveEdge(parallelEdge, true);
            graphArea.InsertEdgeAndData(newEdge, ec, 0, true);
        }
    }
}
