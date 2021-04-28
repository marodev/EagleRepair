using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast.Parser
{
    public interface ISolutionParser
    {
        public Task<Solution> OpenSolutionAsync(string solutionFilePath);
        public Workspace Workspace();
    }
}
