using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;

namespace demojsonparser_modded
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var demoparser = new DemoParser(File.OpenRead(args[0]))) //Force garbage collection since outputstream of the parser cannot be changed
            {
                demoparser.ParseHeader();

                while (demoparser.ParseNextTick())
                {

                }
            }
        }
    }
}
