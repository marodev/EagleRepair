using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Ast.Parser
{
    public class SolutionParser : IDisposable
    {
        public SolutionParser()
        {
            MSBuildLocator.RegisterDefaults();
            Workspace = MSBuildWorkspace.Create();
        }
        
        public MSBuildWorkspace Workspace { get; }

        public async Task<Solution> OpenSolutionAsync(string solutionFilePath)
        {
            if (!File.Exists(solutionFilePath))
            {
                throw new ArgumentException($"Path {solutionFilePath} provided for {nameof(solutionFilePath)} does not exist.");
            }
            Console.WriteLine($"await Workspace.OpenSolutionAsync({solutionFilePath})");
            return await Workspace.OpenSolutionAsync(solutionFilePath);
        }

        private void ReleaseUnmanagedResources()
        {
            Workspace?.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SolutionParser()
        {
            ReleaseUnmanagedResources();
        }
    }
}