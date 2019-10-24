using System.IO;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}