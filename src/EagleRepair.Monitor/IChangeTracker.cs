using System;
using System.Collections.Generic;

namespace EagleRepair.Monitor
{
    public interface IChangeTracker
    {
        public void Stage(Message message);
        public void Confirm();
        public void Revert();
        public Dictionary<string, IList<Message>> All();

        public Tuple<int, string> FixesSummaryToDisplayString();

        public string StatisticsToCsv(int totalDetected = 0, int totalFixes = 0, string runtime = null);
    }
}
