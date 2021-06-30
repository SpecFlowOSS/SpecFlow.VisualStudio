namespace TechTalk.SpecFlow.IdeIntegration.Services
{
    public interface IDirectoryService
    {
        bool Exists(string path);
        void CreateDirectory(string path);
    }
}