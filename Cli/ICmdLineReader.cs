using System.Collections.Generic;

namespace Cli
{
    public interface ICmdLineReader
    {
        public CmdOptions Parse(IEnumerable<string> commandLineArgs);
    }
}