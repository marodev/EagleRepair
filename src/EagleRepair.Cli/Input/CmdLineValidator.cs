using System;
using System.IO;
using System.Linq;
using EagleRepair.Ast.Parser;
using EagleRepair.Cli.Wrapper;

namespace EagleRepair.Cli.Input
{
    public class CmdLineValidator : ICmdLineValidator
    {
        private readonly IRuleParser _ruleParser;
        private readonly IFileWrapper _fileWrapper;

        public CmdLineValidator(IRuleParser ruleParser, IFileWrapper fileWrapper)
        {
            _ruleParser = ruleParser;
            _fileWrapper = fileWrapper;
        }

        public CmdOptions Validate(CmdOptions options)
        {
            options.Rules = _ruleParser.Parse(options.InputRules);
            options.SolutionPath = ValidateSolutionPath(options.SolutionPath);

            return options;
        }

        private string ValidateSolutionPath(string solutionPath)
        {
            if (solutionPath is null or ".")
            {
                return FindSolution();
            }

            if (!_fileWrapper.Exists(solutionPath))
            {
                throw new ArgumentException($"Solution file not found: {solutionPath}");
            }
            
            if (!solutionPath.EndsWith(".sln"))
            {
                throw new ArgumentException("Only solution (*.sln) files are supported yet");
            }

            return solutionPath;

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