using Redbus.Events;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.Events
{
    public class NodeDroppedEvent<T> : EventBase
    {
        public readonly Node<T> DroppedNodeInformation;

        public NodeDroppedEvent(Node<T> n)
        {
            DroppedNodeInformation = n;
        }
    }
}