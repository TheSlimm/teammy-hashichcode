using System.Collections.Generic;

namespace HashCode
{
    public class Endpoint
    {
        public int Id { get; set; }

        public int LatencyDataCenter { get; set; }

        public Dictionary<int, int> IdCacheServerLatency { get; set; }
    }
}