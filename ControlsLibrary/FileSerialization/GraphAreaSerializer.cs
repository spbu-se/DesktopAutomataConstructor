using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using ControlsLibrary.Controls.Scene;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using GraphX.Common;
using GraphX.Common.Models;
using GraphX.Controls;
using static System.String;

namespace ControlsLibrary.FileSerialization
{
    public static class GraphAreaSerializer
    {
        public static void Save(GraphArea graphArea, string path)
        {
            var dataList = graphArea.ExtractSerializationData();
            dataList.ForEach(data =>
            {
                if (data.Data.GetType() == typeof(NodeViewModel))
                {
                    data.HasLabel = false;
                }
            });
            FileServiceProviderWpf.SerializeDataToFile(path, dataList);
        }

        public static void Open(GraphArea graphArea, string path)
        {
            var document = new XmlDocument();
            document.Load(path);
            var root = document.DocumentElement ?? throw new XmlException(Lang.Serializer_MissingXmlRoot);
            if (root.Name == "ListOfGraphSerializationData")
            {
                var dataList = FileServiceProviderWpf.DeserializeGraphDataFromFile<GraphSerializationData>(path);
                graphArea.RebuildFromSerializationData(dataList);
            }
            else
            {
                OpenJflapFile(graphArea, root);
            }
        }

        private static void OpenJflapFile(GraphArea graphArea, XmlElement root)
        {
            var automatonType = root.SelectSingleNode("type")?.InnerText ?? throw new XmlException(Format(Lang.Serializer_MissingJflapTag, "<type>"));
            if (automatonType != "fa")
            {
                throw new XmlException(Format(Lang.Serializer_UnsupportedAutomatonType, automatonType));
            }

            var automatonXmlNode = root.SelectSingleNode("automaton") ?? root;
            
            var states = ReadJflapStates(automatonXmlNode, out var relayoutRequired);
            var transitions = ReadJflapTransitions(automatonXmlNode, states);

            graphArea.RemoveAllEdges(true);
            graphArea.RemoveAllVertices(true);
            states.Values.ForEach(state => graphArea.AddVertexAndData((NodeViewModel) state.Vertex, state, true));
            transitions.ForEach(transition => graphArea.InsertEdgeAndData((EdgeViewModel) transition.Edge, transition, 0, true));
            if (relayoutRequired)
            {
                graphArea.RelayoutGraph();
            }
        }

        private static IDictionary<long, VertexControl> ReadJflapStates(XmlNode automatonXmlNode, out bool relayoutRequired)
        {
            relayoutRequired = false;
            var states = new Dictionary<long, VertexControl>();
            var xmlNodes = automatonXmlNode.ChildNodes;
            // can't use foreach here since XmlNodeList doesn't implement IEnumerable<XmlNode>
            for (var i = 0; i < xmlNodes.Count; i++)
            {
                var xmlNode = xmlNodes[i];
                if (xmlNode.Name == "state" && xmlNode.NodeType == XmlNodeType.Element)
                {
                    var idStr = xmlNode.Attributes["id"]?.Value ?? throw new XmlException(Lang.Serializer_StateWithoutIdTag);
                    if (!long.TryParse(idStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                    {
                        throw new XmlException(Format(Lang.Serializer_StateWithNonIntegerId, idStr));
                    }

                    var nodeViewModel = new NodeViewModel
                    {
                        ID = id,
                        Name = xmlNode.Attributes["name"]?.Value ?? "q" + id,
                        IsInitial = xmlNode.SelectSingleNode("initial") != null,
                        IsFinal = xmlNode.SelectSingleNode("final") != null
                    };

                    double x = 0;
                    var xStr = xmlNode.SelectSingleNode("x")?.InnerText;
                    if (xStr != null && !double.TryParse(xStr, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                    {
                        throw new XmlException(Format(Lang.Serializer_StateWithNonNumericalProperty, nodeViewModel.Name, "x", xStr));
                    }

                    double y = 0;
                    var yStr = xmlNode.SelectSingleNode("y")?.InnerText;
                    if (yStr != null && !double.TryParse(yStr, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                    {
                        throw new XmlException(Format(Lang.Serializer_StateWithNonNumericalProperty, nodeViewModel.Name, "y", yStr));
                    }

                    var vertexControl = new VertexControl(nodeViewModel);
                    if (xStr == null || yStr == null)
                    {
                        relayoutRequired = true;
                    }
                    else
                    {
                        vertexControl.SetPosition(x, y);
                    }

                    states.Add(id, vertexControl);
                }
            }

            return states;
        }

        private static ICollection<EdgeControl> ReadJflapTransitions(XmlNode automatonXmlNode, IDictionary<long, VertexControl> states)
        {
            var transitions = new Dictionary<(VertexControl, VertexControl), EdgeControl>();
            var xmlNodes = automatonXmlNode.ChildNodes;
            // can't use foreach here since XmlNodeList doesn't implement IEnumerable<XmlNode>
            for (var i = 0; i < xmlNodes.Count; i++)
            {
                var xmlNode = xmlNodes[i];
                if (xmlNode.Name == "transition" && xmlNode.NodeType == XmlNodeType.Element)
                {
                    var fromStr = xmlNode.SelectSingleNode("from")?.InnerText ?? throw new XmlException(Format(Lang.Serializer_TransitionWithoutTag, "<from>"));
                    if (!long.TryParse(fromStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fromId))
                    {
                        throw new XmlException(Format(Lang.Serializer_TransitionWithNonIntegerProperty, "from", fromStr));
                    }

                    if (!states.TryGetValue(fromId, out var from))
                    {
                        throw new XmlException(Format(Lang.Serializer_TransitionWithNonExistent, "from", fromStr));
                    }

                    var toStr = xmlNode.SelectSingleNode("to")?.InnerText ?? throw new XmlException(Format(Lang.Serializer_TransitionWithoutTag, "<to>"));
                    if (!long.TryParse(toStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var toId))
                    {
                        throw new XmlException(Format(Lang.Serializer_TransitionWithNonIntegerProperty, "to", toStr));
                    }

                    if (!states.TryGetValue(toId, out var to))
                    {
                        throw new XmlException(Format(Lang.Serializer_TransitionWithNonExistent, "to", toStr));
                    }

                    var transitionToken = xmlNode.SelectSingleNode("read")?.InnerText ?? "";
                    if (transitionToken.Length > 1)
                    {
                        throw new XmlException(Lang.Serializer_MultiCharTransitionTokenUnsupported);
                    }

                    if (!transitions.TryGetValue((from, to), out var edgeControl))
                    {
                        edgeControl = new EdgeControl(from, to, new EdgeViewModel((NodeViewModel) from.Vertex, (NodeViewModel) to.Vertex));
                        transitions[(from, to)] = edgeControl;
                    }

                    var edgeViewModel = (EdgeViewModel) edgeControl.Edge;
                    if (transitionToken == "")
                    {
                        edgeViewModel.IsEpsilon = true;
                    }

                    edgeViewModel.TransitionTokensString += transitionToken;
                }
            }

            return transitions.Values;
        }
    }
}