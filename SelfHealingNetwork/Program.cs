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
            var deser = new Deserializer();
            var graphData = deser.LoadGraph();

            //_graph = NetworkGraph.BuildGraphFromXmlGraph(graphData);
            _graph = new NetworkGraph();
            _graph.GenerateNetworkGraph(Utility.MaxNodes, 100);

            //var programTimer = new Timer(10000);
            //programTimer.Elapsed += OnTimedEvent;
            //programTimer.Enabled = true;

            //_graph.KillNode();
            //_graph.DebugKillNode('F');

            while (true)
            {
                _graph.KillNode();
                Thread.Sleep(10000);
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _graph.KillNode();
        }  
    }
}