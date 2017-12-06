using System;
using System.Collections.Generic;
using System.Linq;

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
            catch (Exception)
            {
                PotentialShortestPaths.Add(source, new List<Tuple<char, int>>() { Tuple.Create(dest, weight)});  
            }
        }

        public void TrimExcessPaths()
        {
            var keys = PotentialShortestPaths.Keys;
            var shortest = int.MaxValue;
            
            foreach (var key in keys)
            {
                shortest = PotentialShortestPaths[key].Select(path => path.Item2).Concat(new[] {shortest}).Min();

                PotentialShortestPaths[key].RemoveAll(n => n.Item2 != shortest);
                
                // case if multiple shortest paths
                if (PotentialShortestPaths[key].Count > 1)
                {
                    PotentialShortestPaths[key].RemoveRange(1, PotentialShortestPaths[key].Count - 1);
                }
            }
        }

        public int GetSize()
        {
            int size = 0;

            foreach (var key in PotentialShortestPaths.Keys)
            {
                size += PotentialShortestPaths[key].Count;
            }

            return size;
        }
    }
}