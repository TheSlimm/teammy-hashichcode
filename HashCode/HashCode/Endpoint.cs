﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode
{
    public class Endpoint
    {
        public int Id { get; set; }
        public Dictionary<int,int> IdCacheServerLatency { get; set; }
        public int LatencyDataCenter { get; set; }
    }
}
