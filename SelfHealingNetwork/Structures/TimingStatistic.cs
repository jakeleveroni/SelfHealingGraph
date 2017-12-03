using System;

namespace SelfHealingNetwork.Structures
{
    public class TimingStatistic
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public struct NodeInfo
        {
            public char NodeName;
            public int NumberOfEdges;
        }

        public NodeInfo NodeInformation;

        public int ElapsedTime => (_endTime - _startTime).Milliseconds;
        public void Start() => _startTime = DateTime.Now;
        public void Stop() => _endTime = DateTime.Now;
    }
}