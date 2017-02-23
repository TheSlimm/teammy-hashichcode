using System;
using System.Collections.Generic;

namespace HashCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(@"file\kittens.in", @"file\kittens.out");
            Run(@"file\me_at_the_zoo.in", @"file\me_at_the_zoo.out");
            Run(@"file\trending_today.in", @"file\trending_today.out");
            Run(@"file\videos_worth_spreading.in", @"file\videos_worth_spreading.out");
        }

        static void Run(string inputFileName, string outputFileName)
        {
            List<Video> videos = new List<Video>();

            List<Request> requests = new List<Request>();
            List<CacheServer> cacheServers = new List<CacheServer>();
            var dataCenter = new DataCenter();


        }
    }
}
