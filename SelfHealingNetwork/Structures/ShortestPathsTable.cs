using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SelfHealingNetwork.Structures
{
    public class ShortestPathsTable
    {
        public readonly Dictionary<char, List<Tuple<char, int>>> PotentialShortestPaths;

        public ShortestPathsTable()
        {
            PotentialShortestPaths = new Dictionary<char, List<Tuple<char, int>>>();
        }

        public void AddPath(char source, char dest, int weight)
        {
            try
            {
                PotentialShortestPaths[source].Add(Tuple.Create(dest, weight));
            }
            catch (Exception e)
            {
                PotentialShortestPaths.Add(source, new List<Tuple<char, int>>() { Tuple.Create(dest, weight)});  
            }
        }

        public void RemovePath(char src, char dest, int weight)
        {
            var index = PotentialShortestPaths[src].FindIndex(p => p.Item1 == dest && p.Item2 == weight);

            if (index == -1)
                return;
            
            PotentialShortestPaths[src].RemoveAt(index);
        }

        public void TrimExcessPaths()
        {
            var keys = PotentialShortestPaths.Keys;
            var shortest = int.MaxValue;
            
            foreach (var key in keys)
            {
                foreach (var path in PotentialShortestPaths[key])
                {
                    if (path.Item2 < shortest)
                    {
                        shortest = path.Item2;                        
                    }
                }

                PotentialShortestPaths[key].RemoveAll(n => n.Item2 != shortest);
                
                // case if multiple shortest paths
                if (PotentialShortestPaths[key].Count > 1)
                {
                    PotentialShortestPaths[key].RemoveRange(1, PotentialShortestPaths[key].Count);
                }
            }
        }
    }
}