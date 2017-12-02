using System.Collections.Generic;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.Interfaces
{
    public interface ISearchAlgorithm
    {
        List<Node> Search(Node start, Node end);
    }
}