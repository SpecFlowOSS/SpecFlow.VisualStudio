using System;
using System.Reflection;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class CurrentExtensionVersionProvider : ICurrentExtensionVersionProvider
    {
        public Version GetCurrentExtensionVersion()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return new Version(assemblyVersion.Major, assemblyVersion.Minor);
        }
    }
}