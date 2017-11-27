using System.Data;

namespace SelfHealingNetwork.Structures
{
    public class WeightedEdge<T>
    {
        public int Weight { get; }
        private Node<T> Start { get; }
        public Node<T> End { get; }

        public WeightedEdge(Node<T> start, Node<T> end, int weight)
        {
            Start = start;
            End = end;
            Weight = weight;
            start.AddNeighbor(end);
            start.AddEdge(this);
        }

        public bool ContainsNode(Node<T> node)
        {
            return Start == node || End == node;
        }
        
        public override string ToString()
        {
            return $"start: {Start.Value} | Weight: {Weight} | End: {End.Value}";
        }
    }
}