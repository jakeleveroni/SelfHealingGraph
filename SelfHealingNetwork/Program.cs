using System;
using System.Timers;
using System.Threading;
using SelfHealingNetwork.Structures;
using SelfHealingNetwork.Xml;

namespace SelfHealingNetwork
{
    internal class Program
    {
        private static NetworkGraph _graph;
            
        public static void Main(string[] args)
        {
            _graph = new NetworkGraph();


            if (args[0] == "gen")
            {
                _graph.GenerateNetworkGraph(Utility.MaxNodes, 100);

            }
            else if (args[0] == "xml" || args.Length == 0)
            {
                var deser = new Deserializer();
                var graphData = deser.LoadGraph();
                _graph = NetworkGraph.BuildGraphFromXmlGraph(graphData);
            }
            else
            {
                Console.WriteLine("Invalid command line argument valid options are: 'gen', 'xml', and ''");
                return;
            }

            while (true)
            {
                _graph.KillNode();
                Thread.Sleep(2000);
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _graph.KillNode();
        }  
    }
}