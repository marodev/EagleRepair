using System;
using Ast;
using CommandLine;

namespace Cli
{
    public static class InputReader
    {
        public static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<CmdOptions>(args)
                    .MapResult(options =>
                        {
                            options.Validate();
                            var success = Engine.RunAsync(options.SolutionPath, options.Rules).Result;
                            return success ? 0 : 1;
                        },
                        errors => 1);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine($"Invalid arguments detected: {ae}");
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception occurred: {e}");
                return 1;
            }
        }
    }
}