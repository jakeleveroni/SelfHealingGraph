using Redbus.Events;
using SelfHealingNetwork.Structures;

namespace SelfHealingNetwork.Events
{
    public class NodeDroppedEvent : EventBase
    {
        public readonly Node DroppedNodeInformation;

        public NodeDroppedEvent(Node n)
        {
            DroppedNodeInformation = n;
        }
    }
}