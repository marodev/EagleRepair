using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ast;
using CommandLine;

namespace Cli
{
    public class CmdOptions
    {
        [Option('r', "rules", Required = false, HelpText = "Specify the rules to apply.")]
        public ICollection<string> InputRules { get; set; }

        public ICollection<Rule> Rules { get; set; }

        [Option('p', "path", Required = false, Default = ".",
            HelpText = "Specify the full path to the solution (.sln) file.")]
        public string SolutionPath { get; set; }

        private void ValidateSolutionPath()
        {
            if (SolutionPath.EndsWith(".sln") && File.Exists(SolutionPath))
            {
                return;
            }

            SolutionPath = FindSolution();
        }

        private void ValidateRules()
        {
            Rules = Ast.Parser.RuleParser.Parse(InputRules);
        }

        public void Validate()
        {
            ValidateSolutionPath();
            ValidateRules();
        }

        private static string FindSolution()
        {
            var currentDir = Environment.CurrentDirectory;
            var paths = Directory.GetFiles(currentDir, "*.sln");

            return paths.Length switch
            {
                0 => throw new ArgumentException($"Solution (*.sln) file not found in: {currentDir}"),
                > 1 => throw new ArgumentException($"Multiple solution (*.sln) files found in: {currentDir}"),
                _ => paths.First()
            };
        }
    }
}