using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using JSON;
using Generator;

namespace demojsonparser_console
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("");
                    Console.WriteLine("-help:");
                    Console.WriteLine("Use like the following: demojsonparser-console file1.dem file2.dem file3.dem ...");
                    Console.WriteLine("JSON file(s) will be dropped in the same directory");
                    Console.WriteLine("Please do not use .dem older than July 1st 2015");
                    return;
                }

                foreach (string path in args)
                {
                    if (File.Exists(path))
                    {
                        Console.WriteLine("Start parsing file: " + path);

                        using (var demoparser = new DemoParser(File.OpenRead(path)))
                        {
                            ParseTask p = new ParseTask
                            {
                                destpath = path,
                                srcpath = path,
                                usepretty = true,
                                showsteps = true,
                                specialevents = true,
                                highdetailplayer = true,
                                positioninterval = 8
                            };

                            GameStateGenerator.GenerateJSONFile(demoparser, p);

                        }

                    }
                    else
                    {
                        Console.WriteLine("Path: " + path + "does not exist");
                        Console.WriteLine("Demofile will be skipped.");
                    }
                }

                Console.WriteLine("Any key to Quit");
                Console.ReadLine();
                System.Environment.Exit(1);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Trace: ");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Commandline arguments missing? Closing in 10 seconds.");

                System.Threading.Thread.Sleep(10000);
            }

        }
    }
}
