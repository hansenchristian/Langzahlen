using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.IO;

namespace Langzahlen
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = "";
            string result = "";
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.verbose)
                {
                    Console.WriteLine("Start Programm");
                }
                if(!options.iFile.Equals(""))
                {
                    if (File.Exists(options.iFile))
                    {
                        StreamReader sr = File.OpenText(options.iFile);
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] splited = line.Split(new Char[] {' ',','});
                            if(splited[0].Equals("PZZ"))
                            {
                                int i = 2;
                                long number = long.Parse(splited[1]);
                                result = result + "PZZ from: " + number;

                                while (number > 1)
                                {
                                    if(number%i == 0)
                                    {
                                        result = result + i;
                                        number = number / i;
                                        i = 2;
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                                Console.WriteLine(result);

                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found!");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine(options.GetUsage());
                    Console.ReadKey();
                }
                
            }
        }
    }

    class Options
    {
        [Option('p', HelpText = "Calculate parallel", DefaultValue = true)]
        public bool parallel { get; set; }

        [Option('s', HelpText = "Calculate serial")]
        public bool serial { get; set; }

        [Option('i', HelpText = "Inputfile from", DefaultValue = "")]
        public string iFile { get; set; }

        [Option('o', HelpText = "Output filename", DefaultValue = "")]
        public string filename { get; set; }

        [Option('t', HelpText = "Processing time", DefaultValue = false)]
        public bool time { get; set; }

        [Option('v', HelpText = "Verbose", DefaultValue = false)]
        public bool verbose { get; set; }

        [Option('w', HelpText = "Wait before terminate")]
        public bool wait { get; set; }

        [Option('x', HelpText = "eXtensions")]
        public bool xtends { get; set; }

        [Option('h', HelpText = "Help", DefaultValue = false)]
        public bool help { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}