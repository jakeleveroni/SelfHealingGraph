using System.Collections.Generic;
using SelfHealingNetwork.Interfaces;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.SearchAlgorithms
{
    public abstract class SearchAlgorithm : ISearchAlgorithm
    {
        public abstract List<Node<T>> Search<T>(Node<T> start, Node<T> end);
        
        protected static List<Node<T>> ReconstructPath<T>(Dictionary<Node<T>, Node<T>> parentMap, Node<T> start, Node<T> end)
        {
            var path = new List<Node<T>>();
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