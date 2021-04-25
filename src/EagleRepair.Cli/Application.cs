using System.Collections.Generic;
using System.Threading.Tasks;
using Ast;

namespace Cli
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