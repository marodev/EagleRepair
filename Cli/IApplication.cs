using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cli
{
    public interface IApplication
    {
        public Task<bool> Run(IEnumerable<string> commandLineArgs);
    }
}