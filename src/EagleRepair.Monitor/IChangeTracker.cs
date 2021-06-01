using System.Collections.Generic;

namespace EagleRepair.Monitor
{
    public interface IChangeTracker
    {
        public void Stage(Message message);
        public void Confirm();
        public void Revert();
        public Dictionary<string, IList<Message>> All();

        public string FixesSummaryToDisplayString();

        public string StatisticsToCsv();
    }
}
