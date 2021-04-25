using System.Collections.Generic;
using CommandLine;
using EagleRepair.Ast;

namespace EagleRepair.Cli.Input
{
    public class CmdOptions
    {
        [Option('r', "rules", Required = false, HelpText = "Specify the rules to apply.")]
        public ICollection<string> InputRules { get; set; }

        public ICollection<Rule> Rules { get; set; }

        [Option('p', "path", Required = false, Default = ".",
            HelpText = "Specify the full path to the solution (.sln) file.")]
        public string SolutionPath { get; set; }
    }
}