using System;

namespace SelfHealingNetwork.Structures
{
    public class TimingStatistic
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public struct NodeInfo
        {
            public string NodeName;
            public int NumberOfEdges;
        }
        
        public TimeSpan ElapsedTime => _endTime - _startTime;

        public void Start()
        {
            _startTime = DateTime.Now;
        }

        public void Stop()
        {
            _endTime = DateTime.Now;
        }
    }
}