namespace TechTalk.SpecFlow.VsIntegration.Implementation.Services
{
    public interface IDirectoryService
    {
        bool Exists(string path);
        void CreateDirectory(string path);
    }
}