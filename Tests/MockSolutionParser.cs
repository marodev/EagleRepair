using System.Threading.Tasks;
using Ast.Parser;
using Microsoft.CodeAnalysis;

namespace Tests
{
    public class MockSolutionParser : ISolutionParser
    {
        private readonly Solution _solution;
        public MockSolutionParser(Solution solution)
        {
            _solution = solution;
        }
        public Task<Solution> OpenSolutionAsync(string solutionFilePath)
        {
            return Task.FromResult(_solution);
        }

        public Workspace Workspace()
        {
            return _solution.Workspace;
        }
    }
}