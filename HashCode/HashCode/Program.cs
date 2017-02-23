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
            //Run(@"files\test.in", @"files\test.out");
            Run(@"files\kittens.in", @"files\kittens.out");
            Run(@"files\me_at_the_zoo.in", @"files\me_at_the_zoo.out");
            Run(@"files\trending_today.in", @"files\trending_today.out");
            Run(@"files\videos_worth_spreading.in", @"files\videos_worth_spreading.out");
        }

        static void Run(string inputFileName, string outputFileName)
        {
            Console.WriteLine("Début du fichier : " + inputFileName);

            var videos = new Dictionary<int, Video>();
            var requests = new Dictionary<int, Request>();
            var cacheServers = new Dictionary<int, CacheServer>();
            var endpoints = new Dictionary<int, Endpoint>();

            var dataCenter = new DataCenter();

            FileReaderHelper.ReadFile(inputFileName, videos, requests, endpoints, cacheServers, dataCenter);

            int i = 0;
            foreach (var requestK in requests)
            {
                var request = requestK.Value;
                i++;

                if (i % 100 == 0)
                    Console.WriteLine("{0}%", i * 100 / requests.Count);

                foreach (var cacheServerK in cacheServers)
                {
                    var cacheServer = cacheServerK.Value;
                    var endpointLocal = endpoints[request.IdEndpoint];

                    if (!endpointLocal.IdCacheServerLatency.ContainsKey(cacheServer.Id))
                        continue;

                    var latenceCacheServer =
                       endpointLocal.IdCacheServerLatency[cacheServer.Id];

                    var calcul = ((endpointLocal.LatencyDataCenter - latenceCacheServer) * request.NbRequests) / videos[request.IdVideo].Size;

                    if (request.Score < calcul)
                        request.Score = calcul;
                }
            }

            foreach (var requestK in requests.OrderByDescending(re => re.Value.Score))
            {
                var request = requestK.Value;

                var maxScoring = 0;
                var idCacheServer = -1;
                //todo : check over cache
                var videoSize = videos[request.IdVideo].Size;

                foreach (var cacheServerK in cacheServers.Where(x => videoSize <= x.Value.SizeRemaining))
                {
                    var cacheServer = cacheServerK.Value;

                    var endpointLocal = endpoints[request.IdEndpoint];

                    if (!endpointLocal.IdCacheServerLatency.ContainsKey(cacheServer.Id))
                        continue;

                    var latenceCacheServer =
                       endpointLocal.IdCacheServerLatency[cacheServer.Id];

                    var score = (endpointLocal.LatencyDataCenter - latenceCacheServer) * request.NbRequests;

                    if (maxScoring < score)
                    {
                        maxScoring = score;
                        idCacheServer = cacheServer.Id;
                    }

                }
                if (idCacheServer > -1)
                {
                    var cache = cacheServers[idCacheServer];
                    var video = videos[request.IdVideo];

                    if (cache.Videos.Find(v => v.Id == request.IdVideo) == null)
                    {
                        cache.SizeRemaining -= video.Size;
                        cache.Videos.Add(video);
                    }
                }
            }

            Console.WriteLine("Ecriture du fichier : " + outputFileName);
            using (var fileOut = new StreamWriter(outputFileName))
            {
                fileOut.WriteLine(cacheServers.Count);

                foreach (var server in cacheServers)
                {
                    fileOut.Write(server.Value.Id);

                    foreach (var video in server.Value.Videos)
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
