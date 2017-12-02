using System;
using System.Collections.Generic;
using System.Text;
using Redbus;
using SelfHealingNetwork.Events;

namespace SelfHealingNetwork.Structures
{
    public class Node : IDisposable
    {
        private readonly List<Node> _neighbors;
        public readonly List<WeightedEdge> Edges;
        public char Value { get; }
        private static readonly EventBus _bus = new EventBus();
        public int FailureProbability;
        private static Random _rng = new Random();
        
        public bool IsVisited { get; set; }
        public double Cost { get; set; }

        public Node(char value)
        {
            Value = value;
            IsVisited = false;
            _neighbors = new List<Node>();
            Edges = new List<WeightedEdge>();
            FailureProbability = _rng.Next(1, 101);
        }

        public Node(char value, List<Node> neighbors)
        {
            Value = value;
            IsVisited = false;
            _neighbors = neighbors;
            Edges = new List<WeightedEdge>();
            FailureProbability = _rng.Next(1, 101);
        }

        public override string ToString()
        {
            var s = new StringBuilder("{value} :");
            _neighbors.ForEach(n => s.Append($"{n.Value} "));
            return s.ToString();
        }

        public bool WillFail() => _rng.Next(1, 101) < FailureProbability;       
        public void AddNeighbor(Node node) => _neighbors?.Add(node);

        public void AddEdge(WeightedEdge edge)
        {
            if (!Edges.Contains(edge))
            {
                Edges?.Add(edge);
            }
        }

        public bool EdgeExists(Node n1, Node n2)
        {
            var edge = Edges.Find(e => e.Start == n1 && e.End == n2);
            return edge != default(WeightedEdge);
        }

        public void Dispose()
        {
            _bus.Publish(new NodeDroppedEvent(this));
        }
    }
}