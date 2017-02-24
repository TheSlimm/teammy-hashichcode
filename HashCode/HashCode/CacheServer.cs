using System.Collections.Generic;

namespace HashCode
{
    public class CacheServer
    {
        public int Id { get; set; }

        public int Size { get; set; }

        public int SizeRemaining { get; set; }

        public List<Video> Videos { get; set; }
    }
}