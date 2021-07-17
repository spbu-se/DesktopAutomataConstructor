using System;
using System.Collections.Generic;
using System.Linq;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using NUnit.Framework;
using QuickGraph;

namespace ControlsLibrary.Tests
{
    public class NfaToDfaConverterTests
    {
        [Test]
        public void WhenConvertingDfaInvalidOperationExceptionShouldBeThrown()
        {
            var s0 = new NodeViewModel {Name = "S0", IsInitial = true, IsFinal = true};
            var nfaGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            nfaGraph.AddVertex(s0);
            Assert.Throws<InvalidOperationException>(() => NfaToDfaConverter.Convert(nfaGraph));
        }

        [Test]
        public void NfaShouldBeCorrectlyConverted()
        {
            var nextId = 0;
            var s0 = new NodeViewModel {Name = "S0", IsInitial = true, ID = nextId++};
            var s1 = new NodeViewModel {Name = "S1", ID = nextId++};
            var s2 = new NodeViewModel {Name = "S2", IsFinal = true, ID = nextId};
            var nfaGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            nfaGraph.AddVertex(s0);
            nfaGraph.AddVertex(s1);
            nfaGraph.AddVertex(s2);
            nfaGraph.AddEdge(new EdgeViewModel(s0, s0) {TransitionTokensString = "0"});
            nfaGraph.AddEdge(new EdgeViewModel(s0, s1) {TransitionTokensString = "1"});
            nfaGraph.AddEdge(new EdgeViewModel(s1, s1) {TransitionTokensString = "01"});
            nfaGraph.AddEdge(new EdgeViewModel(s1, s2) {TransitionTokensString = "0"});
            nfaGraph.AddEdge(new EdgeViewModel(s2, s1) {TransitionTokensString = "1"});
            nfaGraph.AddEdge(new EdgeViewModel(s2, s2) {TransitionTokensString = "01"});

            var dfaGraph = NfaToDfaConverter.Convert(nfaGraph);

            CollectionAssert.AreEquivalent(
                dfaGraph.Vertices.Select(v => new Tuple<string, bool, bool>(v.Name, v.IsInitial, v.IsFinal)).ToList(),
                new List<Tuple<string, bool, bool>>
                {
                    new Tuple<string, bool, bool>("S0", true, false),
                    new Tuple<string, bool, bool>("S1", false, false),
                    new Tuple<string, bool, bool>("S1,S2", false, true),
                });

            CollectionAssert.AreEquivalent(
                dfaGraph.Edges
                    .Select(e =>
                        new Tuple<string, string, string>(e.Source.Name, e.Target.Name, ToStringSortedByChars(e.TransitionTokensString))
                    ).ToList(),
                new List<Tuple<string, string, string>>
                {
                    new Tuple<string, string, string>("S0", "S0", "0"),
                    new Tuple<string, string, string>("S0", "S1", "1"),
                    new Tuple<string, string, string>("S1", "S1", "1"),
                    new Tuple<string, string, string>("S1", "S1,S2", "0"),
                    new Tuple<string, string, string>("S1,S2", "S1,S2", "01")
                });
        }

        [Test]
        public void EpsilonNfaShouldBeCorrectlyConverted()
        {
            var nextId = 0;
            var s0 = new NodeViewModel {Name = "S0", IsInitial = true, ID = nextId++};
            var s1 = new NodeViewModel {Name = "S1", ID = nextId++};
            var s2 = new NodeViewModel {Name = "S2", ID = nextId++};
            var s3 = new NodeViewModel {Name = "S3", ID = nextId++};
            var s4 = new NodeViewModel {Name = "S4", IsFinal = true, ID = nextId};
            var nfaGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            nfaGraph.AddVertex(s0);
            nfaGraph.AddVertex(s1);
            nfaGraph.AddVertex(s2);
            nfaGraph.AddVertex(s3);
            nfaGraph.AddVertex(s4);
            nfaGraph.AddEdge(new EdgeViewModel(s0, s1) {TransitionTokensString = "", IsEpsilon = true});
            nfaGraph.AddEdge(new EdgeViewModel(s0, s2) {TransitionTokensString = "", IsEpsilon = true});
            nfaGraph.AddEdge(new EdgeViewModel(s1, s3) {TransitionTokensString = "0"});
            nfaGraph.AddEdge(new EdgeViewModel(s2, s3) {TransitionTokensString = "1"});
            nfaGraph.AddEdge(new EdgeViewModel(s3, s4) {TransitionTokensString = "1"});

            var dfaGraph = NfaToDfaConverter.Convert(nfaGraph);

            CollectionAssert.AreEquivalent(
                dfaGraph.Vertices.Select(v => new Tuple<string, bool, bool>(v.Name, v.IsInitial, v.IsFinal)).ToList(),
                new List<Tuple<string, bool, bool>>
                {
                    new Tuple<string, bool, bool>("S0,S1,S2", true, false),
                    new Tuple<string, bool, bool>("S3", false, false),
                    new Tuple<string, bool, bool>("S4", false, true),
                });

            CollectionAssert.AreEquivalent(
                dfaGraph.Edges
                    .Select(e =>
                        new Tuple<string, string, string>(e.Source.Name, e.Target.Name, ToStringSortedByChars(e.TransitionTokensString))
                    ).ToList(),
                new List<Tuple<string, string, string>>
                {
                    new Tuple<string, string, string>("S0,S1,S2", "S3", "01"),
                    new Tuple<string, string, string>("S3", "S4", "1")
                });
        }

        private static string ToStringSortedByChars(string str)
        {
            var arr = str.ToCharArray();
            Array.Sort(arr);
            return new string(arr);
        }
    }
}