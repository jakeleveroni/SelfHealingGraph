using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Events;
using Redbus;
using SelfHealingNetwork.SearchAlgorithms;
using SelfHealingNetwork.Xml;

namespace SelfHealingNetwork.Structures
{
    public class NetworkGraph : IDisposable
    {
        private readonly List<Node> _nodes;
        private readonly EventBus _bus;
        private readonly List<SubscriptionToken> _eventTokens;

        private NetworkGraph()
        {
            _nodes = new List<Node>();
            _bus = new EventBus();
            _eventTokens = new List<SubscriptionToken> {_bus.Subscribe<NodeDroppedEvent>(OnNodeDropped)};
        }

        private void AddNode(Node node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }


        public static NetworkGraph BuildGraphFromXmlGraph(Graph graphData)
        {
            var graph = new NetworkGraph();

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
            if (nodes > 26)
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

            return true;
        }


        private IEnumerable<Node> ShortestPath<TSearchAlgorithm>(Node start, Node end) where TSearchAlgorithm : ISearchAlgorithm, new()
        {
            var searchAlgorithm = new TSearchAlgorithm();
            InitializeCosts(start);
            var path = searchAlgorithm.Search(start, end);
            Console.WriteLine($"FROM: {start.Value} TO: {end.Value} COST: {path.CalculatePathCost()}");
            return path;
        }

        private void InitializeCosts(Node start)
        {
            _nodes.ForEach(n => n.Cost = int.MaxValue);
//            _nodes.ForEach(n => n.IsVisited = false);
            start.Cost = 0;
        }

        private void OnNodeDropped(NodeDroppedEvent e)
        {
            var timing = new TimingStatistic();
            var droppedNode = e.DroppedNodeInformation;
            Console.WriteLine($"Node {droppedNode.Value} dropped, starting recovery process...");
            
            timing.Start();

            var paths = new ShortestPathsTable();
            
            foreach (var neighbor in droppedNode.Neighbors)
            {
                foreach (var otherNeighbor in droppedNode.Neighbors)
                {
                    if (neighbor == otherNeighbor) continue;
                    var shortestPath = ShortestPath<DijkstraSearch>(neighbor, otherNeighbor);
                    paths.AddPath(otherNeighbor.Value, neighbor.Value, shortestPath.CalculatePathCost());
                }
            }
            
            RemoveRedundantEdges(droppedNode.Neighbors, droppedNode);
            _nodes.RemoveAll(n => n.Value == droppedNode.Value);
            
            paths.TrimExcessPaths();
            FixEdges(paths);
            
            timing.Stop();
            timing.NodeInformation.NodeName = droppedNode.Value;
            timing.NodeInformation.NumberOfEdges = droppedNode.Edges.Count;
            Console.WriteLine($"Graph recovered in {timing.ElapsedTime}msecs");
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
                AddEdge(key, paths.PotentialShortestPaths[key][0].Item1, paths.PotentialShortestPaths[key][0].Item2);
            }
        }

        public bool KillNode()
        {
            var failingNode = CheckForFailingNode();

            if (failingNode == default(Node))
            {
                return false;
            }
            
            _bus.Publish(new NodeDroppedEvent(failingNode));
            return true;
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
        public void Dispose() => _eventTokens.ForEach(t => _bus.Unsubscribe(t));    
    }
}