using System;
using System.Collections.Generic;
using SelfHealingNetwork.Structures;
using SelfHealingNetwork.Xml;

namespace SelfHealingNetwork
{
    internal class Program
    {
        public static void Main(string[] args)
        {
//            var graph = new NetworkGraph();
//            graph.GenerateNetworkGraph();
//            graph.PrintGraph();
            
            var deser = new Deserializer();
            var graphData = deser.LoadGraph();

            var graph = NetworkGraph.BuildGraphFromXmlGraph(graphData);
            graph.PrintGraph();
            Console.ReadKey();
        }
    }
}