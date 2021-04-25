using System.Collections.Generic;
using System.Threading.Tasks;

namespace EagleRepair.Cli
{
    public interface IApplication
    {
        public Task<bool> Run(IEnumerable<string> commandLineArgs);
    }
}