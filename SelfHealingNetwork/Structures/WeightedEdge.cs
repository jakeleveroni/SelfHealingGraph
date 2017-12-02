using System.Data;

namespace SelfHealingNetwork.Structures
{
    public class WeightedEdge
    {
        public int Weight { get; }
        public Node Start { get; }
        public Node End { get; }

        public WeightedEdge(Node start, Node end, int weight)
        {
            Start = start;
            End = end;
            Weight = weight;
            start.AddNeighbor(end);
            start.AddEdge(this);
        }

        public bool ContainsNode(Node node)
        {
            return Start == node || End == node;
        }
        
        public override string ToString()
        {
            return $"start: {Start.Value} | Weight: {Weight} | End: {End.Value}";
        }
    }
}