using EagleRepair.Cli.Wrapper;

namespace EagleRepair.IntegrationTests.Mock
{
    public class FileWrapperMock : IFileWrapper
    {
        public bool Exists(string filePath)
        {
            return true;
        }
    }
}
