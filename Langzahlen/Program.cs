using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.IO;
using System.Numerics;

namespace Langzahlen
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = "";
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
                            splited = splited.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            if(splited[0].Equals("PZZ"))
                            {
                                Console.WriteLine("PZZ for " + splited[1]);
                                BigInteger number = BigInteger.Parse(splited[1]);
                                List<BigInteger> result = calculatePZZ(number);
                                foreach(BigInteger s in result)
                                {
                                    Console.Write(s + " ");
                                }
                               

                            }else if (splited[0].Equals("KGV"))
                            {
                                BigInteger result = 1;
                                Dictionary<BigInteger, int> highest = new Dictionary<BigInteger, int>();
                                for (int i =1; i<splited.Length; i++)
                                {
                                    BigInteger toCalc = BigInteger.Parse(splited[i]);
                                    List<BigInteger> claculated = calculatePZZ(toCalc);
                                    claculated.Sort();
                                    BigInteger temp = 2;
                                    int counter = 0;
                                    foreach (BigInteger bi in claculated)
                                    {
                                        if(bi == temp)
                                        {
                                            counter++;
                                        }else {
                                            if (highest.ContainsKey(bi))
                                            {
                                                if (highest[bi] < counter)
                                                {
                                                    highest[bi] = counter;
                                                }
                                            }
                                            temp = bi;
                                            counter = 1;
                                        }
                                    }
                                }
                                foreach(KeyValuePair<BigInteger,int> value in highest)
                                {
                                    result = result * value.Key * value.Value;
                                }
                                    
                            }else if (splited[0].Equals("GGT"))
                            {
                                //ToDo
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

        private static List<BigInteger> calculatePZZ(BigInteger number)
        {
            
            int i = 2;
            List<BigInteger> result = new List<BigInteger>();
             
            while (number > 1)
            {
                if (number % i == 0)
                {
                    result.Add(i);
                    number = number / i;
                    i = 2;
                }
                else
                {
                    i++;
                }
            }
            return result;
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