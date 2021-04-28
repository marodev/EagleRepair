using System.IO;

namespace EagleRepair.Cli.Wrapper
{
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
