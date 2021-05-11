using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EagleRepair.Ast;
using EagleRepair.Cli.Input;
using EagleRepair.Monitor;

namespace EagleRepair.Cli
{
    public class Application : IApplication
    {
        private readonly IChangeTracker _changeTracker;
        private readonly ICmdLineReader _cmdLineReader;
        private readonly IEngine _engine;
        private readonly ITimeTracker _timeTracker;

        public Application(ICmdLineReader cmdLineReader, IEngine engine, IChangeTracker changeTracker,
            ITimeTracker timeTracker)
        {
            _cmdLineReader = cmdLineReader;
            _engine = engine;
            _changeTracker = changeTracker;
            _timeTracker = timeTracker;
        }

        public async Task<bool> Run(IEnumerable<string> commandLineArgs)
        {
            _timeTracker.Start();
            var cmdOptions = _cmdLineReader.Parse(commandLineArgs);
            if (cmdOptions == null)
            {
                return false;
            }

            var succeeded = await _engine.RunAsync(cmdOptions.SolutionPath, cmdOptions.Rules);
            _timeTracker.Stop();

            // print stats to console
            Console.WriteLine(_changeTracker.FixesSummaryToDisplayString());
            Console.WriteLine($"Finished in: {_timeTracker.GetElapsedTime()}");

            return succeeded;
        }
    }
}
