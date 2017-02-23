using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode
{
    class Program
    {
        static void Main(string[] args)
        {
            //Run(@"files\test.in", @"files\test.out");
            Run(@"files\kittens.in", @"files\kittens.out");
            Run(@"files\me_at_the_zoo.in", @"files\me_at_the_zoo.out");
            Run(@"files\trending_today.in", @"files\trending_today.out");
            Run(@"files\videos_worth_spreading.in", @"files\videos_worth_spreading.out");
        }

        static void Run(string inputFileName, string outputFileName)
        {

            List<Video> videos = new List<Video>();

            List<Request> requests = new List<Request>();
            List<CacheServer> cacheServers = new List<CacheServer>();
            List<Endpoint> endpoints = new List<Endpoint>();
            var dataCenter = new DataCenter();

            FileReaderHelper.ReadFile(inputFileName, videos, requests, endpoints, cacheServers, dataCenter);

            int tailleMax = cacheServers.First().Size;

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
