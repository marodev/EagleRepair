using EagleRepair.Monitor;

namespace EagleRepair.IntegrationTests.Mock
{
    public class ProgressBarMock : IProgressBar
    {
        public void Report(double value, string message)
        {
            // do nothing
        }
    }
}
