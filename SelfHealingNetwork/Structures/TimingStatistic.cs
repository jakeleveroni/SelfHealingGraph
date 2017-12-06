using System;

namespace SelfHealingNetwork.Structures
{
    public static class TimingStatistic
    {
        [ThreadStatic]
        private static DateTime _startTime;

        [ThreadStatic]
        private static DateTime _endTime;

        public struct NodeInfo
        {
            public char NodeName;
            public int NumberOfEdges;
        }

        public static NodeInfo NodeInformation;

        public static int ElapsedTime => (_endTime - _startTime).Milliseconds;
        public static void Start() => _startTime = DateTime.Now;
        public static void Stop() => _endTime = DateTime.Now;
    }
}