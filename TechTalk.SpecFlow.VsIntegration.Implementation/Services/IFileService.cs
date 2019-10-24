
namespace TechTalk.SpecFlow.VsIntegration.Implementation.Services
{
    public interface IFileService
    {
        bool Exists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}
