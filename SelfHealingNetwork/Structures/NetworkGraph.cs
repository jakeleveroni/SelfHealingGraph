using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Events;
using SelfHealingNetwork.SearchAlgorithms;
using SelfHealingNetwork.Xml;
using Redbus;


namespace SelfHealingNetwork.Structures
{
    public class NetworkGraph
    {
        private readonly List<Node> _nodes;
        private readonly EventBus _bus;
        private delegate void NodeDroppedDel(NodeDroppedEvent e);
        private NodeDroppedDel NodeDroppedHandler;

        public NetworkGraph(string type="jakes")
        {
            _nodes = new List<Node>();
            _bus = new EventBus();

            NodeDroppedHandler = (type == "jakes") ? new NodeDroppedDel(JakesAlgorithm) : new NodeDroppedDel(KruskalsAlgorithm);
        }

        private void AddNode(Node node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }


        public static NetworkGraph BuildGraphFromXmlGraph(Graph graphData, string type="jakes")
        {
            var graph = new NetworkGraph(type);

            foreach (var node in graphData.Nodes)
            {
                graph.AddNode(new Node(node.Value));
            }

            foreach (var node in graphData.Nodes)
            {
                foreach (var edge in node.Edges)
                {
                    graph.AddEdge(node.Value, edge.EdgeTo, edge.Weight);
                }
            }

            return graph;
        }

        private void AddEdge(char nodeValueOne, char nodeValueTwo, int weight)
        {
            var n1 = _nodes.Find(n => n.Value == nodeValueOne);
            var n2 = _nodes.Find(n => n.Value == nodeValueTwo);

            if (n1 == null || n2 == null) return;

            n1.AddEdge(new WeightedEdge(n1, n2, weight));
        }

        private static void RemoveEdge(Node n1, Node n2)
        {
            if (!n1.EdgeExists(n1, n2)) return;
            
            n1.Edges.RemoveEdgeBetween(n1, n2);
            n2.Edges.RemoveEdgeBetween(n2, n1);
        }
        
        public void RemoveNode(Node node) => _nodes.Remove(node);

        public bool GenerateNetworkGraph(int nodes = 10, int edges = 12)
        {
            if (nodes > Utility.MaxNodes)
                return false;
            
            var nodeVals = Utility.GetPossibleNodeValues();
            
            for (var i = 0; i < nodes; ++i)
            {
                var node = new Node(nodeVals.Next().Value);
                _nodes.Add(node);
            }
            
            for (var i = 0; i < edges; ++i)
            {
                var rng = new Random();
                var index1 = 0;
                var index2 = 0;
                
                while (index1 == index2)
                {
                    index1 = rng.Next(0, _nodes.Count);
                    index2 = rng.Next(0, _nodes.Count);
                    
                    
                    if (_nodes[index1].EdgeExists(_nodes[index1], _nodes[index2]))
                    {
                        index1 = index2;
                    }
                }
                
                var weight = rng.Next(1, 100);
                var edge = new WeightedEdge(_nodes[index1], _nodes[index2], weight);
                var revEdge = new WeightedEdge(_nodes[index2], _nodes[index1], weight);
                _nodes[index1].AddEdge(edge);
                _nodes[index2].AddEdge(revEdge);
            }

            var edgeCount = _nodes.Sum(n => n.Edges.Count);
            Console.WriteLine($"Graph of size {_nodes.Count} nodes and {edgeCount} edges generated");
            return true;
        }


        private IEnumerable<Node> ShortestPath<TSearchAlgorithm>(Node start, Node end) where TSearchAlgorithm : ISearchAlgorithm, new()
        {
            var searchAlgorithm = new TSearchAlgorithm();
            InitializeCosts(start);
            var path = searchAlgorithm.Search(start, end);
            return path;
        }

        private void InitializeCosts(Node start)
        {
            _nodes.ForEach(n => n.Cost = int.MaxValue);
            _nodes.ForEach(n => n.IsVisited = false);

            start.Cost = 0;
        }

        private void JakesAlgorithm(NodeDroppedEvent e)
        {
            var droppedNode = e.DroppedNodeInformation;
            Console.WriteLine($"Node {droppedNode.Value} dropped, starting recovery process...");
            
            TimingStatistic.Start();

            var paths = new ShortestPathsTable();
            
            foreach (var neighbor in droppedNode.Neighbors)
            {
                foreach (var otherNeighbor in droppedNode.Neighbors)
                {
                    //Console.WriteLine($"{neighbor.Value} {otherNeighbor.Value}");
                    if (neighbor == otherNeighbor) continue;

                    if (!EdgeAlreadyExists(neighbor.Value, otherNeighbor.Value))
                    {
                        var shortestPath = ShortestPath<DijkstraSearch>(neighbor, otherNeighbor);
                        paths.AddPath(otherNeighbor.Value, neighbor.Value, shortestPath.CalculatePathCost());
                    }
                }
            }
            
            RemoveRedundantEdges(droppedNode.Neighbors, droppedNode);
            _nodes.RemoveAll(n => n.Value == droppedNode.Value);
            
            paths.TrimExcessPaths();
            FixEdges(paths);

            //PrintGraph();

            TimingStatistic.Stop();
            TimingStatistic.NodeInformation.NodeName = droppedNode.Value;
            TimingStatistic.NodeInformation.NumberOfEdges = droppedNode.Edges.Count;
            Console.WriteLine($"Dropped Node had {droppedNode.Neighbors.Count} neighbors");
            Console.WriteLine($"Graph recovered in {TimingStatistic.ElapsedTime}msecs, {paths.GetSize()} edges added");
        }

        private void KruskalsAlgorithm(NodeDroppedEvent e)
        {
            
        }

        private bool EdgeAlreadyExists(char value1, char value2)
        {
            var index = FindNodeIndexByValue(value1);
            var index2 = FindNodeIndexByValue(value2);

            if (_nodes[index].Edges.FindIndex(e => e.End.Value == value2) == -1 ||
               _nodes[index2].Edges.FindIndex(e => e.End.Value == value1) == -1)
            {
                return false;
            }

            return true;
        }

        private static void RemoveRedundantEdges(IEnumerable<Node> neighbors, Node droppedNode)
        {
            foreach (var node in neighbors)
                RemoveEdge(node, droppedNode);
        }

        private void FixEdges(ShortestPathsTable paths)
        {
            foreach (var key in paths.PotentialShortestPaths.Keys)
            {
                if (paths.PotentialShortestPaths[key].Count > 0)
                {
                    AddEdge(key, paths.PotentialShortestPaths[key][0].Item1, paths.PotentialShortestPaths[key][0].Item2);
                }
            }
        }

        public bool KillNode()
        {
            var failingNode = CheckForFailingNode();

            if (failingNode == default(Node))
            {
                return false;
            }
            
            NodeDroppedHandler(new NodeDroppedEvent(failingNode));
            return true;
        }

        public bool DebugKillNode(char nodeVal)
        {
            var node = FindNodeByValue(nodeVal);

            if (node != default(Node))
            {
                NodeDroppedHandler(new NodeDroppedEvent(node));
                return true;
            }

            return false;
        }

        private void ApplyGraphTransformations(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var n = FindNodeByValue(node.Value);

                n.Edges.Clear();
                n.Edges.AddRange(node.Edges);
            }
        }

        public void PrintGraph()
        {
            var builder = new StringBuilder();
            
            foreach (var t in _nodes)
            {
                builder.Append($"Node {t.Value}\n");
                
                foreach (var edge in t.Edges)
                {
                    builder.Append($"\t{edge.Start.Value} - {edge.Weight} -> {edge.End.Value}\n");
                }
            }
            
            Console.WriteLine(builder.ToString());
        }

        private Node CheckForFailingNode() =>_nodes.FirstOrDefault(node => node.WillFail());
        private int FindNodeIndexByValue(char val) => _nodes.FindIndex(n => n.Value == val);
        public Node FindNodeByValue(char value) =>  _nodes.Find(n => n.Value == value);
    }
}