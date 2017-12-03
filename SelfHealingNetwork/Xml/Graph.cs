using System.Collections.Generic;
using System.Xml.Serialization;

namespace SelfHealingNetwork.Xml
{
    [XmlRoot(ElementName = "Graph", Namespace = "SelfHealingGraph")]
    public class Graph
    {
        [XmlElement("Node")] 
        public List<Node> Nodes { get; set; }
    }
}