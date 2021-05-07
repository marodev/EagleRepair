using System.Collections.Generic;

namespace EagleRepair.Monitor
{
    public interface IChangeTracker
    {
        public void Add(Message message);
        public Dictionary<string, IList<Message>> All();

        public string ToDisplayString();
    }
}
