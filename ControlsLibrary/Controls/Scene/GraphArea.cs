using ControlsLibrary.Model;
using GraphX.Controls;
using QuickGraph;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Visual representation of an editor GraphX graph, used with zoom control to draw a graph on a scene
    /// </summary>
    public class GraphArea : GraphArea<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
    {
    }
}
