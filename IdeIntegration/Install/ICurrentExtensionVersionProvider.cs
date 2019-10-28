using System;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface ICurrentExtensionVersionProvider
    {
        Version GetCurrentExtensionVersion();
    }
}
