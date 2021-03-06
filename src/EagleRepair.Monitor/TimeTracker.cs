using System;
using System.Diagnostics;

namespace EagleRepair.Monitor
{
    public class TimeTracker : ITimeTracker
    {
        private readonly Stopwatch _stopWatch;

        public TimeTracker()
        {
            _stopWatch = new Stopwatch();
        }

        public void Start()
        {
            _stopWatch.Start();
        }

        public void Stop()
        {
            _stopWatch.Stop();
        }

        public string GetElapsedTime()
        {
            // Get the elapsed time as a TimeSpan value.
            var ts = _stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            var roundedSeconds = TimeSpan.FromSeconds(Math.Round(ts.TotalSeconds));
            var elapsedTime = $"{roundedSeconds.Seconds} s";
            if (!ts.Minutes.Equals(0))
            {
                elapsedTime = $"{ts.Minutes} min " + elapsedTime;
            }

            return elapsedTime;
        }
    }
}
