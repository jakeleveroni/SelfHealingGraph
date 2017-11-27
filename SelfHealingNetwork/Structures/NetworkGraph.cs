using System;
using System.Collections.Generic;
using System.Linq;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Events;
using Redbus;

namespace SelfHealingNetwork.Structures
{
    public class NetworkGraph<T> : IDisposable
    {
        private readonly List<Node<T>> _nodes;
        private readonly List<WeightedEdge<T>> _edges;
        private readonly EventBus _bus;
        private readonly List<SubscriptionToken> _eventTokens;
        
        public NetworkGraph(List<Node<T>> nodes, List<WeightedEdge<T>> edges)
        {
            _nodes = nodes;
            _edges = edges;
            _bus = new EventBus();
            _eventTokens = new List<SubscriptionToken> {_bus.Subscribe<NodeDroppedEvent<T>>(OnNodeDropped)};
        }

        public void AddNode(Node<T> node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        public List<Node<T>> ShortestPath<TSearchAlgorithm>(Node<T> start, Node<T> end) where TSearchAlgorithm : ISearchAlgorithm, new()
        {
            var searchAlgorithm = new TSearchAlgorithm();
            InitializeCosts(start);
            return searchAlgorithm.Search(start, end);      
        }

        private void InitializeCosts(Node<T> start)
        {
            _nodes.ForEach(n => n.Cost = int.MaxValue);
            start.Cost = 0;
        }

        private static void OnNodeDropped(NodeDroppedEvent<T> e)
        {
            var droppedNode = e.DroppedNodeInformation;
            // TODO handle dropped node
        }

        public void RemoveNode(Node<T> node) => _nodes.Remove(node);
        public Node<T> CheckForailingNode() =>_nodes.FirstOrDefault(node => node.WillFail());
        public void Dispose() => _eventTokens.ForEach(t => _bus.Unsubscribe(t));    
        public void AddEdge(WeightedEdge<T> newEdge) => _edges.Add(newEdge);
        public void RemoveEdge(WeightedEdge<T> edge) => _edges.Remove(edge);
    }
}