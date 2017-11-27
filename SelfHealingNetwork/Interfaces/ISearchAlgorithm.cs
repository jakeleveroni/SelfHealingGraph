using System.Collections.Generic;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.Interfaces
{
    public interface ISearchAlgorithm
    {
        List<Node<T>> Search<T>(Node<T> start, Node<T> end);
    }
}