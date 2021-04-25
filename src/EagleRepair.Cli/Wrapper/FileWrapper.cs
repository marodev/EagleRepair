using System.IO;

namespace Cli.Wrapper
{
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}