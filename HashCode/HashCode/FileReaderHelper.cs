using System.Collections.Generic;
using System.IO;

namespace HashCode
{
    public class FileReaderHelper
    {
        public static void ReadFile(string filePath,
            Dictionary<int, Video> videos, Dictionary<int, Request> requests, Dictionary<int, Endpoint> endpoints,
            Dictionary<int, CacheServer> cacheServers)
        {
            string[] lines = File.ReadAllLines(filePath);

            // First line
            int lineIndex = 0;
            string[] dataRead = lines[lineIndex].Split(' ');

            int nbVideo = int.Parse(dataRead[0]);
            int nbEndpoints = int.Parse(dataRead[1]);
            int nbRequests = int.Parse(dataRead[2]);
            int nbCache = int.Parse(dataRead[3]);
            int cacheSize = int.Parse(dataRead[4]);

            // Création des caches
            for (int cacheIndex = 0; cacheIndex < nbCache; cacheIndex++)
            {
                cacheServers.Add(cacheIndex, new CacheServer { Id = cacheIndex, Size = cacheSize, SizeRemaining = cacheSize, Videos = new List<Video>() });
            }

            // Second line
            lineIndex++;
            dataRead = lines[lineIndex].Split(' ');

            for (int idVideo = 0; idVideo < nbVideo; idVideo++)
            {
                videos.Add(idVideo, new Video { Id = idVideo, Size = int.Parse(dataRead[idVideo]) });
            }

            // Endpoints lines
            for (int idEndpoint = 0; idEndpoint < nbEndpoints; idEndpoint++)
            {
                lineIndex++;
                dataRead = lines[lineIndex].Split(' ');
                Endpoint newEndpoint = new Endpoint
                {
                    Id = idEndpoint,
                    LatencyDataCenter = int.Parse(dataRead[0]),
                    IdCacheServerLatency = new Dictionary<int, int>()
                };

                int nbCacheForEndpoint = int.Parse(dataRead[1]); // Nb de ligne de cache
                for (int cacheIndexForEndpoint = 0; cacheIndexForEndpoint < nbCacheForEndpoint; cacheIndexForEndpoint++)
                {
                    lineIndex++;
                    dataRead = lines[lineIndex].Split(' ');
                    newEndpoint.IdCacheServerLatency.Add(int.Parse(dataRead[0]), int.Parse(dataRead[1]));
                }

                endpoints.Add(idEndpoint, newEndpoint);
            }

            int x = 0;
            // Requests lines
            for (int indexRequest = 0; indexRequest < nbRequests; indexRequest++)
            {
                lineIndex++;
                dataRead = lines[lineIndex].Split(' ');

                requests.Add(x++, new Request()
                {
                    IdVideo = int.Parse(dataRead[0]),
                    IdEndpoint = int.Parse(dataRead[1]),
                    NbRequests = int.Parse(dataRead[2])
                });
            }
        }
    }
}