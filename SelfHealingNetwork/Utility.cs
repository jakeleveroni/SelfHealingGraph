using System;
using System.Collections.Generic;
using System.Linq;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork
{
    public static class Utility
    {
        private static string _nodeVals= @"ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[{]}\|;:',<.>/?";
        public static int MaxNodes { get => _nodeVals.Length;  }

        public static List<NodeValue> GetPossibleNodeValues()
        {
            return _nodeVals.Select(letter => new NodeValue(letter)).ToList();
        }

        
        
        /*
         * Extension Methods
        */
        public static NodeValue Next(this List<NodeValue> l)
        {
            var rng = new Random();

            while (true)
            {
                var index = rng.Next(0, l.Count);

                if (l[index].Taken) continue;
                
                var val = l[index];
                l.RemoveAt(index);
                return val;
            }
        }

        public static int CalculatePathCost(this IEnumerable<Node> l) => l.Sum(n => (int) n.Cost);

        public static void RemoveEdgeBetween(this List<WeightedEdge> l, Node n1, Node n2)
        {
            var index = l.FindIndex(e => e.Start == n1 && e.End == n2);
            l.RemoveAt(index);
        }
    }

    public class NodeValue
    {
        public readonly char Value;
        public readonly bool Taken;

        public NodeValue(char val)
        {
            Value = val;
            Taken = false;
        }
    }
}