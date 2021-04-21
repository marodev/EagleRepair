using System;
using System.Collections.Generic;
using CommandLine;

namespace Engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        
        private static void RunOptions(CmdOptions opts)
        {
            Console.WriteLine("You provided: " + opts.Path);
            foreach (var optsRule in opts.Rules)
            {
                Console.WriteLine("Rule: " + optsRule);
            }

            //handle options
        }
        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var error in errs)
            {
                Console.WriteLine("Error: " + error);
            }
            //handle errors
        }
    }
}