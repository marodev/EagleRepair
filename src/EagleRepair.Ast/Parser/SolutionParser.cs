using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace EagleRepair.Ast.Parser
{
    public class SolutionParser : IDisposable, ISolutionParser
    {
        private readonly MSBuildWorkspace _workspace;
        public SolutionParser()
        {
            MSBuildLocator.RegisterDefaults();
            _workspace = MSBuildWorkspace.Create();
        }

        public async Task<Solution> OpenSolutionAsync(string solutionFilePath)
        {
            if (!File.Exists(solutionFilePath))
            {
                throw new ArgumentException($"Path {solutionFilePath} provided for {nameof(solutionFilePath)} does not exist.");
            }
            
            Console.WriteLine($"await Workspace.OpenSolutionAsync({solutionFilePath})");
            return await _workspace.OpenSolutionAsync(solutionFilePath);
        }

        public Workspace Workspace()
        {
            return _workspace;
        }

        private void ReleaseUnmanagedResources()
        {
            _workspace?.Dispose();
        }

        public void Dispose()
        {
            Console.WriteLine("SolutionParser.Dispose() called.");
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SolutionParser()
        {
            ReleaseUnmanagedResources();
        }
    }
}