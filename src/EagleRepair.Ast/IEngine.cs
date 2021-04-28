using System.Collections.Generic;
using System.Threading.Tasks;

namespace EagleRepair.Ast
{
    public interface IEngine
    {
        public Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules);
    }
}
