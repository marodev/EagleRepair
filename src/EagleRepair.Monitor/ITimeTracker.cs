namespace EagleRepair.Monitor
{
    public interface ITimeTracker
    {
        public void Start();
        public void Stop();
        public string GetElapsedTime();
    }
}
