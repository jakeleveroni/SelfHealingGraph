using System.Collections.Generic;
using System.Xml.Serialization;

namespace SelfHealingNetwork.Xml
{
    public class Node
    {
        [XmlAttribute("Value")] 
        public string value { get; set; }
        public char Value => value[0];

        [XmlElement("Edge")] 
        public List<Edge> Edges { get; set; } 
    }
}