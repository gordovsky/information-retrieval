//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SuffixTree {

//    public class SuffixTree
//    {
//        public char? Canonizator { get; set; }
//        public string Word { get; set; }
//        private int CurrentSuffixStartIndex { get; set; }
//        private int CurrentSuffixEndIndex { get; set; }
//        private int UnresolvedSuffixes { get; set; }
//        private int DistanceIntoActiveEdge { get; set; }
//        private int NextNode { get; set; }
//        private int NextEdge { get; set; }
//        private char LastCharacterOfCurrentSuffix { get; set; }
//        public Node Root { get; private set; }
//        private Node ActiveNode { get; set; }
//        private Edge ActiveEdge { get; set; } 
//        private Node LastCreatedNode { get; set; }
//        public SuffixTree(string word)
//        {
//            Word = word;
//            Root = new Node(this);
//            ActiveNode = Root;
//        }
//        public event Action<SuffixTree> Changed;
//        private void TriggerChanged()
//        {
//            var handler = Changed;
//            if (handler != null)
//                handler(this);
//        }

//        public event Action<string, object[]> Messege;

//        private void SendMessage(string format, params object[] args)
//        {
//            var handler = Messege;
//            if (handler != null)
//                handler(format, args);
//        }

//        public static SuffixTree Create(string word, char Canonizator = '$')
//        {
//            var tree = new SuffixTree(word);
//            tree.bui
//        }

//        public void Build(char canonizator)
//        {
//            var n = Word.IndexOf(Word[Word.Length - 1]);
//            var mustCanonize = n < Word.Length - 1;
//            if(mustCanonize)
//            {
//                Canonizator = canonizator;
//                Word = string.Concat(Word, canonizator);
//            }

//            for(CurrentSuffixEndIndex = 0; CurrentSuffixStartIndex < Word.Length; CurrentSuffixEndIndex++)
//            {
//                SendMessage("iteration: {0}", CurrentSuffixEndIndex);
//                LastCreatedNode = null;
//                LastCharacterOfCurrentSuffix = Word[CurrentSuffixEndIndex];

//                for(CurrentSuffixStartindex = CurrentSuffixEndIndex - UnresolvedSuffixes;
//                    CurrentSuffixStartIndex <= CurrentSuffixEndIndex; CurrentSuffixStartIndex++)
//                {
//                    var wasImplicitlyAdded = !AddNextSuffix();
//                    if(wasImplicitlyAdded)
//                    {
//                        UnresolvedSuffixes++;
//                        break;
//                    }
//                    if (UnresolvedSuffixes > 0)
//                        UnresolvedSuffixes--;
//                }
//            }
//        }

//        private bool AddNextSuffix()
//        {
//            var suffix = string.Concat(Word.Substring(CurrentSuffixStartIndex, CurrentSuffixEndIndex - CurrentSuffixStartIndex), "{", Word[CurrentSuffixEndIndex], "}");
//            SendMessage("The next suffix of '{0}' to add is '{1}' at indixes {2},{3}", Word, suffix, CurrentSuffixStartIndex, CurrentSuffixEndIndex);
//            if (ActiveEdge != null)
//                return AddCurrentSuffixToActiveEdge();
//            if (GetExistingEdgeAndSetAsActive())
//                return false;

//            ActiveEdge.AddNewEdge();
//            TriggerChanged();

//            UpdateActivePointAfterAddingNewEdge();

//            return true;
//        }

//        private bool GetExistingEdgeAndSetAsActive()
//        {
//            Edge edge;
//            if(ActiveNode.Edges.TryGetValue(LastCharacterOfCurrentSuffix, out edge))
//            {
//                SendMessage("Existing edge for {0} starting with '{1}' found. Values adjusted to:", ActiveNode, LastCharacterOfCurrentSuffix);
//                ActiveEdge = edge;
//                DistanceIntoActiveEdge = 1;
//                TriggerChanged();

//                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(ActiveEdge.StartIndex);
//                SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
//                SendMessage(" => DistanceIntoActiveEdge is now: {0}", DistanceIntoActiveEdge);
//                SendMessage(" => UnresolvedSuffixes is now: {0}", UnresolvedSuffixes);

//                return true;
//            }
//            SendMessage("Existing edge for {0} starting with '{1}' not found", ActiveNode, LastCharacterOfCurrentSuffix);
//            return false;
//        }

//        private bool AddCurrentSuffixToActiveEdge()
//        {
//            var nextCharOnEdge = Word[ActiveEdge.StartIndex + DistanceIntoActiveEdge];
//            if(nextCharacterOnEdge == LastCharacterOfCurrentSuffix)
//            {
//                SendMessage("The next character on the current edge is '{0}' (suffix added implicitly)", LastCharacterOfCurrentSuffix);
//                DistanceIntoActiveEdge++;
//                TriggerChanged();

//                SendMessage(" => DistanceIntoActiveEdge is now: {0}", DistanceIntoActiveEdge);
//                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(ActiveEdge.StartIndex);

//                return false;
//            }

//            SplitActiveEdge();
//            ActiveEdge.Tail.AddNewEdge();
//            TriggerChanged();

//            UpdateActivePointAfterAddingNewEdge();

//            return true;
//        }
//        private void UpdateActivePointAfterAddingNewEdge()
//        {
//            if (ReferenceEquals(ActiveNode, RootNode))
//            {
//                if (DistanceIntoActiveEdge > 0)
//                {
//                    SendMessage("New edge has been added and the active node is root. The active edge will now be updated.");
//                    DistanceIntoActiveEdge--;
//                    SendMessage(" => DistanceIntoActiveEdge decremented to: {0}", DistanceIntoActiveEdge);
//                    ActiveEdge = DistanceIntoActiveEdge == 0 ? null : ActiveNode.Edges[Word[CurrentSuffixStartIndex + 1]];
//                    SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
//                    TriggerChanged();

//                    NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(CurrentSuffixStartIndex + 1);
//                }
//            }
//            else
//                UpdateActivePointToLinkedNodeOrRoot();
//        }

//        private void NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(int firstIndexOfOriginalActiveEdge)
//        {
//            var walkDistance = 0;
//            while (ActiveEdge != null && DistanceIntoActiveEdge >= ActiveEdge.Length)
//            {
//                SendMessage("Active point is at or beyond edge boundary and will be moved until it falls inside an edge boundary");
//                DistanceIntoActiveEdge -= ActiveEdge.Length;
//                ActiveNode = ActiveEdge.Tail ?? RootNode;
//                if (DistanceIntoActiveEdge == 0)
//                    ActiveEdge = null;
//                else
//                {
//                    walkDistance += ActiveEdge.Length;
//                    var c = Word[firstIndexOfOriginalActiveEdge + walkDistance];
//                    ActiveEdge = ActiveNode.Edges[c];
//                }
//                TriggerChanged();
//            }
//        }

//        private void SplitActiveEdge()
//        {
//            ActiveEdge = ActiveEdge.SplitAtIndex(ActiveEdge.StartIndex + DistanceIntoActiveEdge);
//            SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
//            TriggerChanged();
//            if (LastCreatedNodeInCurrentIteration != null)
//            {
//                LastCreatedNodeInCurrentIteration.LinkedNode = ActiveEdge.Tail;
//                SendMessage(" => Connected {0} to {1}", LastCreatedNodeInCurrentIteration, ActiveEdge.Tail);
//                TriggerChanged();
//            }
//            LastCreatedNodeInCurrentIteration = ActiveEdge.Tail;
//        }

//        private void UpdateActivePointToLinkedNodeOrRoot()
//        {
//            SendMessage("The linked node for active node {0} is {1}", ActiveNode, ActiveNode.LinkedNode == null ? "[null]" : ActiveNode.LinkedNode.ToString());
//            if (ActiveNode.LinkedNode != null)
//            {
//                ActiveNode = ActiveNode.LinkedNode;
//                SendMessage(" => ActiveNode is now: {0}", ActiveNode);
//            }
//            else
//            {
//                ActiveNode = RootNode;
//                SendMessage(" => ActiveNode is now ROOT", ActiveNode);
//            }
//            TriggerChanged();

//            if (ActiveEdge != null)
//            {
//                var firstIndexOfOriginalActiveEdge = ActiveEdge.StartIndex;
//                ActiveEdge = ActiveNode.Edges[Word[ActiveEdge.StartIndex]];
//                TriggerChanged();
//                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(firstIndexOfOriginalActiveEdge);
//            }
//        }
//    public class Edge
//    {
//        private readonly SuffixTree Tree;
//        public Node Head { get; private set; }
//        public Node Tail { get; private set; }
//        public int StartIndex { get; private set; }
//        public int? EndIndex { get; set; }
//        public int EdgeNumber { get; set; }
//        public int Length { get { return (EndIndex ?? Tree.Word.Length - 1) - StartIndex + 1; } }
//    }
//    public class Node
//    {
//        private readonly SuffixTree Tree;
//        public Dictionary<char, Edge> Edges { get; private set; }
//        public Node LinkedNode { get; set; }
//        public int NodeNumber { get; private set; }
//        public Node(SuffixTree tree)
//        {
//            Tree = tree;
//            Edges = new Dictionary<char, Edge>();
//            NodeNumber = Tree.NextNodeNumber++;
//        }
//    }


//}