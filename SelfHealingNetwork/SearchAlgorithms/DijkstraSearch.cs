using System;
using System.Collections.Generic;
using SelfHealingNetwork.Structures;
using ConcurrentPriorityQueue;

namespace SelfHealingNetwork.SearchAlgorithms
{
    public class DijkstraSearch : SearchAlgorithm
    {
        public override List<Node> Search(Node start, Node end)
        {
            var parentMap = new Dictionary<Node, Node>();
            var priorityQueue = new ConcurrentPriorityQueue<Node, double>();

            priorityQueue.Enqueue(start, start.Cost);

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (current.IsVisited) continue;
                
                current.IsVisited = true;

                if (current.Equals(end)) break;

                foreach (var edge in current.Edges)
                {
                    var neighbor = edge.End;
                    var newCost = current.Cost + edge.Weight;
                    var neighborCost = neighbor.Cost;

                    if (newCost > neighborCost) continue;
                        
                    neighbor.Cost = newCost;
                    try
                    {
                        parentMap.Add(neighbor, current);
                        var priority = newCost;
                        priorityQueue.Enqueue(neighbor, priority);
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Tried adding node that already exists. Look into this");
                    }

                }
            }
            
            return ReconstructPath(parentMap, start, end);
        }
    }
}