using System;
using System.Collections.Generic;
using System.Text;
using Redbus;
using SelfHealingNetwork.Events;

namespace SelfHealingNetwork.Structures
{
    public class Node<T> : IDisposable
    {
        private readonly List<Node<T>> _neighbors;
        public readonly List<WeightedEdge<T>> Edges;
        public T Value { get; }
        private static readonly EventBus _bus = new EventBus();
        public int FailureProbability;
        private static Random _rng = new Random();
        
        public bool IsVisited { get; set; }
        public double Cost { get; set; }

        public Node(T value)
        {
            Value = value;
            IsVisited = false;
            _neighbors = new List<Node<T>>();
            Edges = new List<WeightedEdge<T>>();
            FailureProbability = _rng.Next(1, 101);
        }

        public Node(T value, List<Node<T>> neighbors)
        {
            Value = value;
            IsVisited = false;
            _neighbors = neighbors;
            Edges = new List<WeightedEdge<T>>();
            FailureProbability = _rng.Next(1, 101);
        }

        public override string ToString()
        {
            var s = new StringBuilder("{value} :");
            _neighbors.ForEach(n => s.Append($"{n.Value} "));
            return s.ToString();
        }

        public bool WillFail() => _rng.Next(1, 101) < FailureProbability;       
        public void AddNeighbor(Node<T> node) => _neighbors?.Add(node);
        public void AddEdge(WeightedEdge<T> edge) => Edges?.Add(edge);
        
        public void Dispose()
        {
            _bus.Publish(new NodeDroppedEvent<T>(this));
        }
    }
}