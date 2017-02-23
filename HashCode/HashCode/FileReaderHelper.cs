using System.Collections.Generic;
using System.IO;

namespace HashCode
{
    public class FileReaderHelper
    {
        public static void ReadFile(string filePath,
            Dictionary<int, Video> videos, Dictionary<int, Request> requests, Dictionary<int, Endpoint> endpoints,
            Dictionary<int, CacheServer> cacheServers, DataCenter dataCenter)
        {
            var lines = File.ReadAllLines(filePath);

            // First line
            var lineIndex = 0;
            var dataRead = lines[lineIndex].Split(' ');

            var nbVideo = int.Parse(dataRead[0]);
            var nbEndpoints = int.Parse(dataRead[1]);
            var nbRequests = int.Parse(dataRead[2]);
            var nbCache = int.Parse(dataRead[3]);
            var cacheSize = int.Parse(dataRead[4]);

            // Création des caches
            for (var cacheIndex = 0; cacheIndex < nbCache; cacheIndex++)
            {
                cacheServers.Add(cacheIndex, new CacheServer() { Id = cacheIndex, Size = cacheSize,SizeRemaining = cacheSize, Videos = new List<Video>() });
            }

            // Second line
            lineIndex++;
            dataRead = lines[lineIndex].Split(' ');

            for (var idVideo = 0; idVideo < nbVideo; idVideo++)
            {
                videos.Add(idVideo, new Video { Id = idVideo, Size = int.Parse(dataRead[idVideo]) });
            }

            // Endpoints lines
            for (var idEndpoint = 0; idEndpoint < nbEndpoints; idEndpoint++)
            {
                lineIndex++;
                dataRead = lines[lineIndex].Split(' ');
                var newEndpoint = new Endpoint
                {
                    Id = idEndpoint,
                    LatencyDataCenter = int.Parse(dataRead[0]),
                    IdCacheServerLatency = new Dictionary<int, int>()
                };

                var nbCacheForEndpoint = int.Parse(dataRead[1]); // Nb de ligne de cache
                for (var cacheIndexForEndpoint = 0; cacheIndexForEndpoint < nbCacheForEndpoint; cacheIndexForEndpoint++)
                {
                    lineIndex++;
                    dataRead = lines[lineIndex].Split(' ');
                    newEndpoint.IdCacheServerLatency.Add(int.Parse(dataRead[0]), int.Parse(dataRead[1]));
                }

                endpoints.Add(idEndpoint, newEndpoint);
            }

            var x = 0;
            // Requests lines
            for (var indexRequest = 0; indexRequest < nbRequests; indexRequest++)
            {
                lineIndex++;
                dataRead = lines[lineIndex].Split(' ');

                requests.Add(x++, new Request()
                {
                    IdVideo  = int.Parse(dataRead[0]),
                    IdEndpoint = int.Parse(dataRead[1]),
                    NbRequests = int.Parse(dataRead[2])
                });
            }
        }


    }
}