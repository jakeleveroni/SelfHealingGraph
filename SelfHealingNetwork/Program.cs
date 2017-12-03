using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
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

            _graph = NetworkGraph.BuildGraphFromXmlGraph(graphData);
            _graph.PrintGraph();

            var programTimer = new System.Timers.Timer(10000);
            programTimer.Elapsed += OnTimedEvent;
            programTimer.Interval = 10000;
            programTimer.Enabled = true;

            _graph.DebugKillNode('F');

            Console.ReadKey();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _graph.KillNode();
        }
        
    }
}