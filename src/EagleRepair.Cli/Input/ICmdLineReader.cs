using System.Collections.Generic;

namespace EagleRepair.Cli.Input
{
    public interface ICmdLineReader
    {
        public CmdOptions Parse(IEnumerable<string> commandLineArgs);
    }
}
