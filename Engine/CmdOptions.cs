using System.Collections.Generic;
using CommandLine;

namespace Engine
{
    public class CmdOptions
    {
        [Option('r', "rules", Required = false, HelpText = "Specify the rules to apply.")]
        public IEnumerable<string> Rules { get; set; }
        
        [Option('p', "path", Required = false, Default = ".", HelpText = "Specify the full path to the solution (.sln) file.")]
        public string Path { get; set; }
    }
}