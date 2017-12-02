using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Events;
using Redbus;

namespace SelfHealingNetwork.Structures
{
    public class NetworkGraph : IDisposable
    {
        private readonly List<Node> _nodes;
        private readonly EventBus _bus;
        private readonly List<SubscriptionToken> _eventTokens;
        
        public NetworkGraph()
        {
            _nodes = new List<Node>();
            _bus = new EventBus();
            _eventTokens = new List<SubscriptionToken> {_bus.Subscribe<NodeDroppedEvent>(OnNodeDropped)};
        }

        public void AddNode(Node node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        public void AddEdge(Node node1, Node node2, int weight)
        {
            if (!_nodes.Contains(node1) || !_nodes.Contains(node2)) return;
            
            var index = _nodes.FindIndex(x => node1 == x);

            if (index != -1)
            {
                _nodes[index].AddEdge(new WeightedEdge(node1, node2, weight));
            }
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

        private void InitializeCosts(Node  start)
        {
            _nodes.ForEach(n => n.Cost = int.MaxValue);
            start.Cost = 0;
        }

        private static void OnNodeDropped(NodeDroppedEvent e)
        {
            var droppedNode = e.DroppedNodeInformation;
            // TODO handle dropped node
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