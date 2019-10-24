
namespace TechTalk.SpecFlow.VsIntegration.Implementation.Services
{
    public interface IWindowsRegistry
    {
        object GetValueForCurrentUser(string registryPath, string registryValueName, object defaultValue);
    }
}
