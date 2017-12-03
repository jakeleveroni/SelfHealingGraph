using System.Xml.Serialization;

namespace SelfHealingNetwork.Xml
{
    public class Edge
    {
        [XmlAttribute("EdgeTo")]
        public string edgeTo { get; set; }
        public char EdgeTo => edgeTo[0];

        [XmlAttribute("Weight")] 
        public int Weight { get; set; }
    }
}