using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfHealingNetwork
{
    public static class Utility
    {
        private static string _nodeVals= "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static List<NodeValue> GetPossibleNodeValues()
        {
            return _nodeVals.Select(letter => new NodeValue(letter)).ToList();
        }

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