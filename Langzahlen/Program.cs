﻿using System;
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
            string output = "";
            var options = new Options();
            var watch = System.Diagnostics.Stopwatch.StartNew();

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
                            var linewatch = System.Diagnostics.Stopwatch.StartNew();
                            string[] splited = line.Split(new Char[] {' ',','});
                            splited = splited.Where(x => !string.IsNullOrEmpty(x)).ToArray();


                            if (splited[0].Equals("PZZ"))
                            {
                                output = output + (System.Environment.NewLine + line);
                                BigInteger number = BigInteger.Parse(splited[1]);
                                List<BigInteger> result = calculatePZZ(number);
                                foreach(BigInteger s in result)
                                {
                                    output = output + (s + " ");
                                }
                                output = output + (System.Environment.NewLine);
                            }
                            else if (splited[0].Equals("KGV"))
                            {
                                if (options.parallel)
                                {
                                    BigInteger kgv = calculateKGVorGGT(splited, splited[0]);
                                    output = output + (System.Environment.NewLine + line + System.Environment.NewLine + kgv);
                                }
                                else
                                {
                                    BigInteger kgv = calculateKGVorGGT(splited, splited[0]);
                                    output = output + (System.Environment.NewLine + line + System.Environment.NewLine + kgv);
                                }
                                
                            }
                            else if (splited[0].Equals("GGT"))
                            {
                                BigInteger ggt = calculateKGVorGGT(splited, splited[0]);
                                output = output + (System.Environment.NewLine + line + System.Environment.NewLine + ggt);
                            }


                            if (options.time)
                            {
                                output = output + ("Time: " + linewatch.ElapsedMilliseconds + " ms");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found!");
                    }
                }
                else
                {
                    Console.WriteLine(options.GetUsage());
                }
            }

            watch.Stop();

            if (options.time)
            {
                output = output + (System.Environment.NewLine + "Overall Time: " + watch.ElapsedMilliseconds + " ms");
            }

            if (options.wait)
            {
                Console.ReadKey();
            }
            if (!options.oFile.Equals(""))
            {
                StreamWriter file = new StreamWriter("c:\\" + options.oFile);
                file.Write(output);
            }
            else
            {
                Console.Write(output);
            }
        }

        async private Task<BigInteger> calculateKGVorGGTAsync(string[] _split, string option)
        {
            BigInteger result = 1;
            Dictionary<BigInteger, int> primefactors = new Dictionary<BigInteger, int>();

            await Task.Run(() =>
            {
                Parallel.For(1, _split.Length, i =>
                {
                    BigInteger toCalc = BigInteger.Parse(_split[i]);
                    List<BigInteger> calculated = calculatePZZ(toCalc);

                    if (i == 1 || primefactors == null)
                    {
                        primefactors = calculated.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
                    }
                    else
                    {
                        Dictionary<BigInteger, int> tempfactors = calculated.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

                        if (option == "KGV")
                        {
                            Parallel.ForEach(tempfactors, (KeyValuePair<BigInteger, int> value) =>
                            {
                                if (!primefactors.ContainsKey(value.Key))
                                {
                                    primefactors.Add(value.Key, value.Value);
                                }
                                else if (primefactors[value.Key] < value.Value)
                                {
                                    primefactors[value.Key] = value.Value;
                                }
                            });
                        }
                        else if (option == "GGT")
                        {
                            Parallel.ForEach(primefactors, (KeyValuePair<BigInteger, int> value) =>
                        {
                            if (!tempfactors.ContainsKey(value.Key))
                            {
                                tempfactors.Add(value.Key, 0);
                            }
                        });

                            primefactors = primefactors.Concat(tempfactors.Where(kvp => primefactors.ContainsKey(kvp.Key)))
                           .GroupBy(x => x.Key)
                           .ToDictionary(x => x.Key, x => x.Min(y => y.Value));
                        }
                    }
                });
            });

            foreach (KeyValuePair<BigInteger, int> value in primefactors)
            {
                for (int i = 0; i < value.Value; i++)
                {
                    result = result * value.Key;
                }
            }

            return result;
        }

        private static BigInteger calculateKGVorGGT(string[] _split, string option)
        {
            BigInteger result = 1;
            Dictionary<BigInteger, int> primefactors = new Dictionary<BigInteger, int>();

            for (int i = 1; i < _split.Length; i++)
            {
                BigInteger toCalc = BigInteger.Parse(_split[i]);
                List<BigInteger> calculated = calculatePZZ(toCalc);

                if (i == 1 || primefactors == null)
                {
                    primefactors = calculated.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
                }
                else
                {
                    Dictionary<BigInteger, int> tempfactors = calculated.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

                    if (option == "KGV")
                    {
                        foreach (KeyValuePair<BigInteger, int> value in tempfactors)
                        {
                            if (!primefactors.ContainsKey(value.Key))
                            {
                                primefactors.Add(value.Key, value.Value);
                            }
                            else if (primefactors[value.Key] < value.Value)
                            {
                                primefactors[value.Key] = value.Value;
                            }
                        }
                    }
                    else if (option == "GGT")
                    {
                        foreach (KeyValuePair<BigInteger, int> value in primefactors)
                        {
                            if (!tempfactors.ContainsKey(value.Key))
                            {
                                tempfactors.Add(value.Key, 0);
                            }
                        }

                            primefactors = primefactors.Concat(tempfactors.Where(kvp => primefactors.ContainsKey(kvp.Key)))
                           .GroupBy(x => x.Key)
                           .ToDictionary(x => x.Key, x => x.Min(y => y.Value));
                    }
                }
            }

            foreach (KeyValuePair<BigInteger, int> value in primefactors)
            {
                for (int i = 0; i < value.Value; i++)
                {
                    result = result * value.Key;
                }
            }

            return result;
        }

        private static List<BigInteger> calculatePZZ(BigInteger number)
        {
            
            int i = 2;
            List<BigInteger> result = new List<BigInteger>();

            //while (number > 1)
            //{
            //    if (number % i == 0)
            //    {
            //        result.Add(i);
            //        number = number / i;
            //        i = 2;
            //    }
            //    else
            //    {
            //        i++;
            //    }
            //}

            if (isPrime(number) == true)
            {
                result.Add(number);
            }
            else
            {
                BigInteger nextFactor = 2;
                BigInteger tempFactor = 3;

                while (number > 1)
                {
                    if (number % nextFactor > 0)
                    {
                        nextFactor = tempFactor;
                        do
                        {
                            if (number % nextFactor == 0)
                            {
                                tempFactor = nextFactor;
                                break;
                            }

                            nextFactor += 2;
                            //Console.WriteLine(nextFactor);
                        } while (nextFactor < number);
                    }

                    number /= nextFactor;
                    result.Add(nextFactor);
                }
            }


            return result;
        }

        public static bool isPrime(BigInteger number)
        {
            if (number == 1) return false;
            if (number == 2) return true;

            var boundary = (BigInteger)Math.Floor(Math.Pow(Math.E, BigInteger.Log(number) / 2));

            for (BigInteger i = 2; i <= boundary; ++i)
            {
                if (number % i == 0) return false;
            }

            return true;
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
        public string oFile { get; set; }

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