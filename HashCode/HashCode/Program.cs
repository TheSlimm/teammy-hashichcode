using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            int tailleMax = 100;
            List<Video> videos = new List<Video>();

            List<Request> requests = new List<Request>();
            List<CacheServer> cacheServers = new List<CacheServer>();
            var dataCenter = new DataCenter();

            foreach (var req in requests.GroupBy(c => c.IdVideo).OrderByDescending(c => c.Sum(x => x.NbRequests)))
            {
                var idVideo = req.Key;
                foreach (var server in cacheServers)
                {
                    var video = videos.Find(x => x.Id == idVideo);
                    if (server.Videos.Sum(c => c.Size) + video.Size > tailleMax)
                    {
                        continue;
                    }

                    server.Videos.Add(video);
                }
            }

            using (var fileOut = new StreamWriter(outputFileName))
            {
                fileOut.WriteLine(cacheServers.Count);

                foreach (var server in cacheServers)
                {
                    fileOut.Write(server.Id);

                    foreach (var video in server.Videos)
                    {
                        fileOut.Write(" " + video.Id);
                    }

                    fileOut.WriteLine();
                }

                fileOut.Close();
            }

            
        }
    }
}
