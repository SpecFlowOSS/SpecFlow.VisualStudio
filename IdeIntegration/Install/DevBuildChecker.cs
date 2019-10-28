using System;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class DevBuildChecker : IDevBuildChecker
    {
        private readonly ICurrentExtensionVersionProvider _currentExtensionVersionProvider;

        public DevBuildChecker(ICurrentExtensionVersionProvider currentExtensionVersionProvider)
        {
            _currentExtensionVersionProvider = currentExtensionVersionProvider;
        }

        public bool IsDevBuild()
        {
            return _currentExtensionVersionProvider.GetCurrentExtensionVersion().Equals(new Version(1, 0));
        }
    }
}