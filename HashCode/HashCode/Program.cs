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
            //Run(@"files\kittens.in", @"files\kittens.out");
            Run(@"files\me_at_the_zoo.in", @"files\me_at_the_zoo.out");
            Run(@"files\trending_today.in", @"files\trending_today.out");
            Run(@"files\videos_worth_spreading.in", @"files\videos_worth_spreading.out");
        }

        static void Run(string inputFileName, string outputFileName)
        {
            Console.WriteLine("Début du fichier : " + inputFileName);
            List<Video> videos = new List<Video>();

            List<Request> requests = new List<Request>();
            List<CacheServer> cacheServers = new List<CacheServer>();
            List<Endpoint> endpoints = new List<Endpoint>();
            var dataCenter = new DataCenter();

            FileReaderHelper.ReadFile(inputFileName, videos, requests, endpoints, cacheServers, dataCenter);

            int tailleMax = cacheServers.First().Size;

            //var counter = 0;

            //var r = requests.GroupBy(c => c.IdVideo).OrderByDescending(c => c.Sum(x => x.NbRequests));

            //bool finish = false;
            //foreach (var req in r)
            //{

            //    var idVideo = req.Key;
            //    Console.WriteLine("Compteur de vidéos : {0}/{1} ", ++counter, r.Count());
            //    foreach (var server in cacheServers)
            //    {
            //        var video = videos.Find(x => x.Id == idVideo);
            //        if (server.Videos.Sum(c => c.Size) + video.Size > tailleMax)
            //        {
            //            finish = true;
            //            continue;
            //        }

            //        server.Videos.Add(video);
            //    }

            //    //if (finish) break;
            //}

            //foreach (var endpoint in endpoints)
            //{
            //    foreach (var serverCache in endpoint.IdCacheServerLatency)
            //    {
            //        var cacheServer = cacheServers.Find(s => s.Id == serverCache.Key);

            //        var allRequest = requests.Where(e => e.IdEndpoint == endpoint.Id);


            //    }
            //}

            foreach (var request in requests)
            {
                request.Score = request.NbRequests/videos.Find(v => v.Id == request.IdVideo).Size;
            }

            foreach (var request in requests.OrderBy( re => re.Score))
            {
                var maxScoring = 0;
                var idCacheServer = -1;
                //todo : check over cache
                var videoSize = videos.Find(v => v.Id == request.IdVideo).Size;

                foreach (var cacheServer in cacheServers.Where(x => videoSize <= x.SizeRemaining))
                {
                   
                    var endpointLocal = endpoints.Find(p => p.Id == request.IdEndpoint);
                    if (!endpointLocal.IdCacheServerLatency.ContainsKey(cacheServer.Id))
                        continue;
                    var latenceCacheServer =
                       endpointLocal.IdCacheServerLatency[cacheServer.Id];

                    var score = (endpointLocal.LatencyDataCenter - latenceCacheServer)*request.NbRequests;

                    if (maxScoring < score)
                    {
                        maxScoring = score;
                        idCacheServer = cacheServer.Id;
                    }

                }
                if (idCacheServer > -1)
                {
                    var cache = cacheServers.Find(ca => ca.Id == idCacheServer);
                    var video = videos.Find(v => v.Id == request.IdVideo);

                    if (cache.Videos.Find(v => v.Id == request.IdVideo) == null)
                    {
                        cache.SizeRemaining -= video.Size;
                        cache.Videos.Add(video);
                    }
                }

               // request.Score = request.NbRequests / videos.Find(v => v.Id == request.IdVideo).Size;
            }

            Console.WriteLine("Ecriture du fichier : " + outputFileName);
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
