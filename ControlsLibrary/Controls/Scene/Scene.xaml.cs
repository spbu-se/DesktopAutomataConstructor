﻿using ControlsLibrary.Controls.AttributesPanel;
using ControlsLibrary.Model;
using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.Logic.Models;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlsLibrary.Controls.Toolbar;
using System;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Interaction logic for Scene.xaml
    /// </summary>
    public partial class Scene : UserControl
    {
        public AttributesPanelViewModel AttributesPanel { get; } = new AttributesPanelViewModel();

        public ToolbarViewModel toolbar;

        public Scene(ToolbarViewModel toolbar)
        {
            InitializeComponent();
            SetZoomControlProperties();
            SetGraphAreaProperties();
            editor = new EditorObjectManager(graphArea, zoomControl);
            this.toolbar = toolbar;
            toolbar.SelectedToolChanged += ToolbarButton_Checked;
        }

        private void SetZoomControlProperties()
        {
            zoomControl.Zoom = 2;
            zoomControl.MinZoom = .5;
            zoomControl.MaxZoom = 50;
            zoomControl.ZoomSensitivity = 25;
            zoomControl.IsAnimationEnabled = false;
            ZoomControl.SetViewFinderVisibility(zoomControl, Visibility.Hidden);
            zoomControl.MouseDown += zoomControl_MouseDown;
        }

        private void SetGraphAreaProperties()
        {
            var graphLogic = new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>();
            graphArea.LogicCore = graphLogic;
            graphLogic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Custom;
            graphLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            graphLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            graphLogic.EdgeCurvingEnabled = true;
            //graphLogic.EnableParallelEdges = true;
            //graphLogic.ParallelEdgeDistance = 50;
            graphArea.VertexSelected += graphArea_VertexSelected;
            graphArea.EdgeSelected += graphArea_EdgeSelected;
        }

        private void graphArea_EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && selectedTool == SelectedTool.Delete)
            {
                graphArea.RemoveEdge(args.EdgeControl.Edge as EdgeViewModel, true);
                return;
            }
        }

        private VertexControl selectedVertex;
        private SelectedTool selectedTool;
        private readonly EditorObjectManager editor;

        private void zoomControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedTool == SelectedTool.Edit)
                {
                    var pos = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
                    pos.Offset(-22.5, -22.5);
                    var vc = CreateVertexControl(pos);
                    if (selectedVertex != null)
                        CreateEdgeControl(vc);
                }
                else if (selectedTool == SelectedTool.Select)
                {
                    ClearSelectMode(true);
                }
            }
        }

        private void CreateEdgeControl(VertexControl vc)
        {
            if (selectedVertex == null)
            {
                editor.CreateVirtualEdge(vc, vc.GetPosition());
                selectedVertex = vc;
                HighlightBehaviour.SetHighlighted(selectedVertex, true);
                return;
            }

            var data = new EdgeViewModel((NodeViewModel)selectedVertex.Vertex, (NodeViewModel)vc.Vertex, "", new List<char>() { 'a' });
            var ec = new EdgeControl(selectedVertex, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);

            HighlightBehaviour.SetHighlighted(selectedVertex, false);
            selectedVertex = null;
            editor.DestroyVirtualEdge();
        }

        private VertexControl CreateVertexControl(Point position)
        {
            var data = new NodeViewModel() { Name = "Vertex " + (graphArea.VertexList.Count + 1), IsFinal = false, IsInitial = false };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);
            return vc;
        }

        void ToolbarButton_Checked(object sender, EventArgs e)
        {
            if (toolbar.SelectedTool == SelectedTool.EditAttributes)
            {
                zoomControl.Cursor = Cursors.Pen;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (toolbar.SelectedTool == SelectedTool.Delete)
            {
                zoomControl.Cursor = Cursors.Help;
                ClearEditMode();
                ClearSelectMode();
                ClearEditPropertiesMode();
                return;
            }
            if (toolbar.SelectedTool == SelectedTool.Edit)
            {
                zoomControl.Cursor = Cursors.Pen;
                ClearSelectMode();
                ClearEditPropertiesMode();
                return;
            }
            if (toolbar.SelectedTool == SelectedTool.Select)
            {
                zoomControl.Cursor = Cursors.Hand;
                ClearEditMode();
                ClearEditPropertiesMode();
                graphArea.SetVerticesDrag(true, true);
                graphArea.SetEdgesDrag(true);
                return;
            }
        }

        private void ClearSelectMode(bool soft = false)
        {
            graphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    HighlightBehaviour.SetHighlighted(a, false);
                    DragBehaviour.SetIsTagged(a, false);
                });

            if (!soft)
                graphArea.SetVerticesDrag(false);
        }

        private void ClearEditMode()
        {
            if (selectedVertex != null) HighlightBehaviour.SetHighlighted(selectedVertex, false);
            editor.DestroyVirtualEdge();
            selectedVertex = null;
        }

        private void ClearEditPropertiesMode() => AttributesPanel.Attributes.Clear();

        void graphArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                switch (selectedTool)
                {
                    case SelectedTool.Edit:
                        CreateEdgeControl(args.VertexControl);
                        break;
                    case SelectedTool.Delete:
                        SafeRemoveVertex(args.VertexControl);
                        break;
                    case SelectedTool.EditAttributes:
                        var tb = new TextBox();
                        //properties.Items.Clear();
                        //properties.Items.Add(tb);
                        break;
                    default:
                        if (selectedTool == SelectedTool.Select && args.Modifiers == ModifierKeys.Control)
                            SelectVertex(args.VertexControl);
                        break;
                }
            }
        }

        private static void SelectVertex(DependencyObject vc)
        {
            if (DragBehaviour.GetIsTagged(vc))
            {
                HighlightBehaviour.SetHighlighted(vc, false);
                DragBehaviour.SetIsTagged(vc, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vc, true);
                DragBehaviour.SetIsTagged(vc, true);
            }
        }

        private void SafeRemoveVertex(VertexControl vc)
        {
            graphArea.RemoveVertexAndEdges(vc.Vertex as NodeViewModel);
        }

        public void Dispose()
        {
            if (editor != null)
                editor.Dispose();
            if (graphArea != null)
                graphArea.Dispose();
        }
    }
}