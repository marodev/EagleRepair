using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace AstRewriter
{
    public class SolutionParser : IDisposable
    {
        private readonly MSBuildWorkspace _workspace;
        
        public SolutionParser(string solutionFilePath)
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
            return await _workspace.OpenSolutionAsync(solutionFilePath);
        }

        private void ReleaseUnmanagedResources()
        {
            _workspace?.Dispose();
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