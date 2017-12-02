using System.Collections.Generic;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.SearchAlgorithms
{
    public abstract class SearchAlgorithm : ISearchAlgorithm
    {
        public abstract List<Node> Search(Node start, Node end);
        
        protected static List<Node> ReconstructPath(Dictionary<Node, Node> parentMap, Node start, Node end)
        {
            var path = new List<Node>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = parentMap[current];
            }

            path.Add(start);
            path.Reverse();
            
            return path;
        }
    }
}