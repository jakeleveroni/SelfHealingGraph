using System;
using System.Collections.Generic;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var graph = new NetworkGraph();
            graph.GenerateNetworkGraph();
            graph.PrintGraph();

            Console.ReadKey();
        }
    }
}