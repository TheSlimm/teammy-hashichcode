﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode
{
    public class CacheServer
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public List<Video> Videos { get; set; }
        public int SizeRemaining { get; set; }
    }
}
