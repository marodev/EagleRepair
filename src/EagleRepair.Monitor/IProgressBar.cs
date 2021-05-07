namespace EagleRepair.Monitor
{
    public interface IProgressBar
    {
        public void Report(double value, string message = "");
    }
}
