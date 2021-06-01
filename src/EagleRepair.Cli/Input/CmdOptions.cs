using System.Collections.Generic;
using CommandLine;
using EagleRepair.Ast;

namespace EagleRepair.Cli.Input
{
    public class CmdOptions
    {
        [Option('r', "rules", Required = false,
            HelpText =
                "Specify the set of rules to apply, e.g., R1 R2 R3. If no argument is passed, all rules are applied.")]
        public ICollection<string> InputRules
        {
            get;
            set;
        }

        public ICollection<Rule> Rules
        {
            get;
            set;
        }

        [Option('p', "path", Required = false, Default = ".",
            HelpText = "Specify the full path to the solution (.sln) file.")]
        public string SolutionPath
        {
            get;
            set;
        }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('o', "output", Required = false, HelpText = "Specify an output path for statistics.")]
        public string CsvPath { get; set; }
    }
}
