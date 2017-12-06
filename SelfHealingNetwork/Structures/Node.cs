using System;
using System.Collections.Generic;
using System.Text;
using Redbus;
using SelfHealingNetwork.Events;

namespace SelfHealingNetwork.Structures
{
    public class Node : IDisposable
    {
        public readonly List<Node> Neighbors;
        public readonly List<WeightedEdge> Edges;
        public char Value { get; }
        private static readonly EventBus Bus = new EventBus();
        private readonly int _failureProbability;
        private static readonly Random Rng = new Random();
        
        public bool IsVisited { get; set; }
        public double Cost { get; set; }

        public Node(char value)
        {
            Value = value;
            IsVisited = false;
            Neighbors = new List<Node>();
            Edges = new List<WeightedEdge>();
            _failureProbability = Rng.Next(1, 101);
        }

        public Node(Node other)
        {
            Value = other.Value;
            IsVisited = other.IsVisited;
            Neighbors = new List<Node>(other.Neighbors);
            Edges = new List<WeightedEdge>(other.Edges);
            _failureProbability = other._failureProbability;
        }

        public override string ToString()
        {
            var s = new StringBuilder("{value} :");
            Neighbors.ForEach(n => s.Append($"{n.Value} "));
            return s.ToString();
        }

        public bool WillFail() => Rng.Next(1, 101) < _failureProbability;       
        public void AddNeighbor(Node node) => Neighbors?.Add(node);

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
            Bus.Publish(new NodeDroppedEvent(this));
        }
    }
}