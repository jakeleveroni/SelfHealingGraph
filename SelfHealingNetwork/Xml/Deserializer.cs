using System;
using System.IO;
using System.Xml.Serialization;

namespace SelfHealingNetwork.Xml
{
    public class Deserializer
    {
        private const string  _fp = @"./../../Xml/graph.xml";

        public Graph LoadGraph()
        {
            Graph graphData;

            if (!File.Exists(_fp)) return null;
            
            var ser = new XmlSerializer(typeof(Graph));

            using (var reader = new StreamReader(_fp))
            {
                try
                {
                    graphData = ser.Deserialize(reader) as Graph;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not desieralize graph, {e}");
                    throw;
                }
            }

            return graphData;
        }
    }
}