
namespace TechTalk.SpecFlow.IdeIntegration.Services
{
    public interface IWindowsRegistry
    {
        object GetValueForCurrentUser(string registryPath, string registryValueName, object defaultValue);
    }
}
