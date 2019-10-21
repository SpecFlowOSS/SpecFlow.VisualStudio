using Microsoft.Win32;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Services
{
    public class WindowsRegistry : IWindowsRegistry
    {
        public object GetValueForCurrentUser(string registryPath, string registryValueName, object defaultValue)
        {
            var rootKey = Registry.CurrentUser;
            var key = rootKey.OpenSubKey(registryPath);

            if (key == null)
            {
                return null;
            }

            using (key)
            {
                return key.GetValue(registryValueName, defaultValue);
            }
        }
    }
}
