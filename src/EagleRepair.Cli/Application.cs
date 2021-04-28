using System.Collections.Generic;
using System.Threading.Tasks;
using EagleRepair.Ast;
using EagleRepair.Cli.Input;

namespace EagleRepair.Cli
{
    public class Application : IApplication
    {
        private readonly ICmdLineReader _cmdLineReader;
        private readonly IEngine _engine;

        public Application(ICmdLineReader cmdLineReader, IEngine engine)
        {
            _cmdLineReader = cmdLineReader;
            _engine = engine;
        }

        public async Task<bool> Run(IEnumerable<string> commandLineArgs)
        {
            var cmdOptions = _cmdLineReader.Parse(commandLineArgs);
            if (cmdOptions == null)
            {
                return false;
            }

            return await _engine.RunAsync(cmdOptions.SolutionPath, cmdOptions.Rules);
        }
    }
}
