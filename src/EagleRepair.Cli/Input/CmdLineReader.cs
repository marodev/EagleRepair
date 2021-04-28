using System;
using System.Collections.Generic;
using CommandLine;

namespace EagleRepair.Cli.Input
{
    public class CmdLineReader : ICmdLineReader
    {
        private readonly ICmdLineValidator _cmdLineValidator;

        public CmdLineReader(ICmdLineValidator cmdLineValidator)
        {
            _cmdLineValidator = cmdLineValidator;
        }

        public CmdOptions Parse(IEnumerable<string> commandLineArgs)
        {
            try
            {
                return Parser.Default.ParseArguments<CmdOptions>(commandLineArgs)
                    .MapResult(options => _cmdLineValidator.Validate(options),
                        errors => null);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine($"Invalid arguments detected: {ae}");
                return null;
            }
        }
    }
}
