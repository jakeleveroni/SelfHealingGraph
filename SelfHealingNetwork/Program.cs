using System;
using System.Collections.Generic;
using System.Threading;
using SelfHealingNetwork.Structures;
using SelfHealingNetwork.Xml;

namespace SelfHealingNetwork
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var deser = new Deserializer();
            var graphData = deser.LoadGraph();

            var graph = NetworkGraph.BuildGraphFromXmlGraph(graphData);
            
            while (true)
            {
                if (!graph.KillNode()) continue;
                
                // simulate dropped node every 5 seconds
                Thread.Sleep(10000);
            }
        }
    }
}