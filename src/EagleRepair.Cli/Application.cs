using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IFaultTracker _faultTracker;
        private readonly ITimeTracker _timeTracker;

        public Application(ICmdLineReader cmdLineReader, IEngine engine, IChangeTracker changeTracker,
            ITimeTracker timeTracker, IFaultTracker faultTracker)
        {
            _cmdLineReader = cmdLineReader;
            _engine = engine;
            _changeTracker = changeTracker;
            _timeTracker = timeTracker;
            _faultTracker = faultTracker;
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

            var (totalFixes, displayString) = _changeTracker.FixesSummaryToDisplayString();
            Console.WriteLine(displayString);
            // print stats to console
            Console.WriteLine($"Finished in: {_timeTracker.GetElapsedTime()}");

            switch (cmdOptions.Verbose)
            {
                case true when !string.IsNullOrEmpty(cmdOptions.CsvPath):
                    {
                        var (numberOfErrors, errorDisplayString) = _faultTracker.ToDisplayString();
                        Console.WriteLine(errorDisplayString);
                        var totalDetected = totalFixes + numberOfErrors;
                        var csv = _changeTracker.StatisticsToCsv(totalDetected, totalFixes,
                            _timeTracker.GetElapsedTime());
                        await File.WriteAllTextAsync(cmdOptions.CsvPath, csv);
                        break;
                    }
                case true:
                    {
                        var (_, errorDisplayString) = _faultTracker.ToDisplayString();
                        Console.WriteLine(errorDisplayString);
                        break;
                    }
            }

            return succeeded;
        }
    }
}
