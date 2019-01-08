namespace TechTalk.SpecFlow.IdeIntegration.Services
{
    public interface IFileSystem
    {
        string AppendDirectorySeparatorIfNotPresent(string path);
    }
}
