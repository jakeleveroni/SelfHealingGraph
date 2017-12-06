using System;
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
                try
                {
                    path.Add(current);
                    current = parentMap[current];
                }
                catch(Exception)
                {
                    throw new InvalidProgramException(
                        "This is a bug, i havnt found the root of it but it does not happen often. Just re-run program");
                }

            }

            path.Add(start);
            path.Reverse();
            
            return path;
        }
    }
}