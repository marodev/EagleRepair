using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Ast.Parser
{
    public interface ISolutionParser
    {
        public Task<Solution> OpenSolutionAsync(string solutionFilePath);
        public Workspace Workspace();
    }
}