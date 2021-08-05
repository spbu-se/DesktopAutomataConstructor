using System;
using System.Collections.Generic;
using System.Linq;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using GraphX.Common;
using QuickGraph;

namespace ControlsLibrary.Model
{
    public static class NfaToDfaConverter
    {
        public static BidirectionalGraph<NodeViewModel, EdgeViewModel> Convert(
            BidirectionalGraph<NodeViewModel, EdgeViewModel> nfaGraph)
        {
            RequireValidNfa(nfaGraph);

            var dfaGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            var nfa = FiniteAutomata.ConvertGraphToAutomata(nfaGraph.Edges.ToList(), nfaGraph.Vertices.ToList());

            var unhandledQueue = new Queue<Tuple<HashSet<int>, NodeViewModel>>();
            var handledStates = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
            var statesToNodeMap = new Dictionary<HashSet<int>, NodeViewModel>(HashSet<int>.CreateSetComparer());

            var initStates = nfa.EpsilonClosure(
                nfaGraph.Vertices
                    .Where(v => v.IsInitial)
                    .Select(v => (int) v.ID).ToList()
            ).ToHashSet();
            var initNodes = initStates.Select(id => nfaGraph.Vertices.FirstOrDefault(v => v.ID == id)).ToList();
            var initCompositeNode = CreateCompositeNode(initNodes);
            initCompositeNode.IsInitial = true;
            unhandledQueue.Enqueue(new Tuple<HashSet<int>, NodeViewModel>(initStates, initCompositeNode));
            statesToNodeMap[initStates] = initCompositeNode;
            dfaGraph.AddVertex(initCompositeNode);

            while (unhandledQueue.Count != 0)
            {
                var (sourceStates, sourceNode) = unhandledQueue.Dequeue();
                if (!handledStates.Add(sourceStates))
                {
                    continue;
                }
                var newEdges = new Dictionary<NodeViewModel, List<char>>();
                foreach (var ch in nfa.Alphabet)
                {
                    var targetStates = nfa.EpsilonClosure(nfa.GetAllNewStates(sourceStates.ToList(), ch)).ToHashSet();
                    if (targetStates.Count == 0)
                    {
                        continue;
                    }
                    var targetNodes = targetStates
                        .Select(id => nfaGraph.Vertices.FirstOrDefault(v => v.ID == id))
                        .ToList();
                    NodeViewModel targetNode;
                    if (statesToNodeMap.ContainsKey(targetStates))
                    {
                        targetNode = statesToNodeMap[targetStates];
                    }
                    else
                    {
                        targetNode = CreateCompositeNode(targetNodes);
                        statesToNodeMap[targetStates] = targetNode;
                        dfaGraph.AddVertex(targetNode);
                    }

                    if (newEdges.ContainsKey(targetNode))
                    {
                        newEdges[targetNode].Add(ch);
                    }
                    else
                    {
                        newEdges[targetNode] = new List<char> {ch};
                    }

                    unhandledQueue.Enqueue(new Tuple<HashSet<int>, NodeViewModel>(targetStates, targetNode));
                }

                newEdges.ForEach(entry =>
                    dfaGraph.AddEdge(new EdgeViewModel(sourceNode, entry.Key)
                    {
                        TransitionTokensString = new string(entry.Value.ToArray())
                    })
                );
            }

            return dfaGraph;
        }

        private static NodeViewModel CreateCompositeNode(IReadOnlyList<NodeViewModel> nodes) =>
            nodes.Count == 1
                ? nodes[0]
                : new NodeViewModel
                {
                    Name = String.Join(",", nodes.Select(v => v.Name)),
                    IsFinal = nodes.Any(v => v.IsFinal),
                    IsInitial = false,
                    IsExpanded = false
                };

        private static void RequireValidNfa(BidirectionalGraph<NodeViewModel, EdgeViewModel> nfaGraph)
        {
            var errors = FAAnalyzer.GetErrors(nfaGraph);
            if (FAAnalyzer.GetType(nfaGraph) == FATypeEnum.DFA)
            {
                errors.Add(Lang.alreadyDeterministic);
            }

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(String.Join("\n", errors));
            }
        }
    }
}