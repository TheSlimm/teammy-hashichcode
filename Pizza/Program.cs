using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        struct Splice
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;
        }

        static void Main(string[] args)
        {
            Run(@"files\big.in", @"files\big.out");
            Run(@"files\small.in", @"files\small.out");
            Run(@"files\medium.in", @"files\medium.out");
            Run(@"files\example.in", @"files\example.out");
        }

        private static void Run(string filenameIn, string filenameOut)
        {
            var splices = new List<Splice>();

            using (var file = new StreamReader(filenameIn))
            {
                int nbm = 0;
                int nbt = 0;
                int lineNumber = 0;

                var line = file.ReadLine();
                int maxm = int.Parse(line.Split(' ')[2]);
                int maxt = int.Parse(line.Split(' ')[3]);

                while ((line = file.ReadLine()) != null)
                {
                    int i = 0;
                    int start = 0;
                    foreach (var t in line)
                    {
                        if (t == 'T') nbt++;
                        if (t == 'M') nbm++;
                        if (nbt >= maxm && nbm >= maxm)
                        {
                            if (nbt + nbm > maxt)
                            {
                                nbt = 0;
                                nbm = 0;
                                start = i + 1;
                            }
                            else
                            {
                                nbt = 0;
                                nbm = 0;
                                splices.Add(new Splice { x1 = lineNumber, y1 = start, x2 = lineNumber, y2 = i });
                                start = i + 1;
                            }
                        }
                        i++;
                    }
                    nbt = 0;
                    nbm = 0;
                    lineNumber++;
                }
                file.Close();
            } 

            using (var fileOut = new StreamWriter(filenameOut))
            {
                fileOut.WriteLine(splices.Count);
                foreach (var t in splices)
                {
                    fileOut.WriteLine(string.Format("{0} {1} {2} {3}", t.x1, t.y1, t.x2, t.y2));
                }
                fileOut.Close();
            }
        }
    }
}
