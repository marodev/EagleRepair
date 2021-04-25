using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ast
{
    public interface IEngine
    {
        public Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules);
    }
}