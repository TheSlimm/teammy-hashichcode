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

            Dictionary<int, Video> videos = new Dictionary<int, Video>();
            Dictionary<int, Request> requests = new Dictionary<int, Request>();
            Dictionary<int, CacheServer> cacheServers = new Dictionary<int, CacheServer>();
            Dictionary<int, Endpoint> endpoints = new Dictionary<int, Endpoint>();

            FileReaderHelper.ReadFile(inputFileName, videos, requests, endpoints, cacheServers);

            int i = 0;
            foreach (KeyValuePair<int, Request> requestK in requests)
            {
                Request request = requestK.Value;
                i++;

                if (i % 100 == 0)
                    Console.WriteLine("{0}%", i * 100 / requests.Count);

                foreach (KeyValuePair<int, CacheServer> cacheServerK in cacheServers)
                {
                    CacheServer cacheServer = cacheServerK.Value;
                    Endpoint endpointLocal = endpoints[request.IdEndpoint];

                    if (!endpointLocal.IdCacheServerLatency.ContainsKey(cacheServer.Id))
                        continue;

                    int latenceCacheServer = endpointLocal.IdCacheServerLatency[cacheServer.Id];
                    int calcul = ((endpointLocal.LatencyDataCenter - latenceCacheServer) * request.NbRequests) / videos[request.IdVideo].Size;

                    if (request.Score < calcul)
                        request.Score = calcul;
                }
            }

            foreach (KeyValuePair<int, Request> requestK in requests.OrderByDescending(re => re.Value.Score))
            {
                Request request = requestK.Value;
                int maxScoring = 0;
                int idCacheServer = -1;
                //todo : check over cache
                int videoSize = videos[request.IdVideo].Size;

                foreach (KeyValuePair<int, CacheServer> cacheServerK in cacheServers.Where(x => videoSize <= x.Value.SizeRemaining))
                {
                    CacheServer cacheServer = cacheServerK.Value;
                    Endpoint endpointLocal = endpoints[request.IdEndpoint];

                    if (!endpointLocal.IdCacheServerLatency.ContainsKey(cacheServer.Id))
                        continue;

                    int latenceCacheServer = endpointLocal.IdCacheServerLatency[cacheServer.Id];
                    int score = (endpointLocal.LatencyDataCenter - latenceCacheServer) * request.NbRequests;

                    if (maxScoring < score)
                    {
                        maxScoring = score;
                        idCacheServer = cacheServer.Id;
                    }
                }

                if (idCacheServer > -1)
                {
                    CacheServer cache = cacheServers[idCacheServer];
                    Video video = videos[request.IdVideo];

                    if (cache.Videos.Find(v => v.Id == request.IdVideo) == null)
                    {
                        cache.SizeRemaining -= video.Size;
                        cache.Videos.Add(video);
                    }
                }
            }

            Console.WriteLine("Ecriture du fichier : " + outputFileName);
            using (StreamWriter fileOut = new StreamWriter(outputFileName))
            {
                fileOut.WriteLine(cacheServers.Count);

                foreach (KeyValuePair<int, CacheServer> server in cacheServers)
                {
                    fileOut.Write(server.Value.Id);

                    foreach (Video video in server.Value.Videos)
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