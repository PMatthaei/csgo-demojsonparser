using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfoModded;
using demojsonparser.src;

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
                        using (var demoparser = new DemoParser(File.OpenRead(path))) //Force garbage collection since outputstream of the parser cannot be changed
                        {
                            GameStateGenerator.GenerateJSONFile(demoparser, path);
                            Console.WriteLine("Press Q and ENTER to Quit");
                            string input = Console.ReadLine();
                            while (input != "q")
                            {
                                input = Console.ReadLine();
                            }
                            System.Environment.Exit(1);

                        }

                    }
                    else
                    {
                        Console.WriteLine("Path: " + path + "does not exist");
                        Console.WriteLine("Demofile will be skipped");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Commandline arguments missing? Closing in 4seconds.");

                System.Threading.Thread.Sleep(4000);
            }

        }
    }
}
