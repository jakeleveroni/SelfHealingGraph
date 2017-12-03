using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly List<TimingStatistic> _timings;
        
        public NetworkGraph()
        {
            _nodes = new List<Node>();
            _bus = new EventBus();
            _eventTokens = new List<SubscriptionToken> {_bus.Subscribe<NodeDroppedEvent>(OnNodeDropped)};
            _timings = new List<TimingStatistic>();
        }

        public void AddNode(Node node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        public Node FindNodeByValue(char value)
        {
            return _nodes.Find(n => n.Value == value);
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
        
        public void RemoveEdge(Node n1, Node n2)
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
        

        public List<Node> ShortestPath<TSearchAlgorithm>(Node start, Node end) where TSearchAlgorithm : ISearchAlgorithm, new()
        {
            var searchAlgorithm = new TSearchAlgorithm();
            InitializeCosts(start);
            return searchAlgorithm.Search(start, end);      
        }

        private void InitializeCosts(Node start)
        {
            _nodes.ForEach(n => n.Cost = int.MaxValue);
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
                foreach (var otherNeighbor in neighbor.Neighbors)
                {
                    var shortestPath = ShortestPath<DijkstraSearch>(neighbor, otherNeighbor);
                    paths.AddPath(otherNeighbor.Value, neighbor.Value, shortestPath.CalculatePathCost());
                }
            }

            RemoveRedundantEdges(droppedNode.Neighbors, droppedNode.Value);
            _nodes.RemoveAll(n => n.Value == droppedNode.Value);
            
            paths.TrimExcessPaths();
            FixEdges(paths);
            
            timing.Stop();
            _timings.Add(timing);
            
            Console.WriteLine($"Graph recovered in {timing.ElapsedTime}msecs");
        }

        private void RemoveRedundantEdges(List<Node> neighbors, char droppedNodeValue)
        {
            foreach (var node in neighbors)
            {
                for (var i = 0; i < node.Edges.Count; ++i)
                {
                    if (node.Edges[i].End.Value == droppedNodeValue)
                    {
                        node.Edges.RemoveAt(i);
                    }
                }                
            }
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
            
            _bus.PublishAsync(new NodeDroppedEvent(failingNode));
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

        public Node CheckForFailingNode() =>_nodes.FirstOrDefault(node => node.WillFail());
        private int FindNodeIndexByValue(char val) => _nodes.FindIndex(n => n.Value == val);
        public void Dispose() => _eventTokens.ForEach(t => _bus.Unsubscribe(t));    
    }
}