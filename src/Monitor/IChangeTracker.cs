using System.Collections.Generic;

namespace Monitor
{
    public interface IChangeTracker
    {
        public void Add(Message message);
        public ICollection<Message> All();
    }
}