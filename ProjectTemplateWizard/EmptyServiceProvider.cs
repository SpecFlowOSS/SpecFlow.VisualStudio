using System;

namespace ProjectTemplateWizard
{
    // This implementation is only needed for making dependency resolution work.
    // In other words, we want to avoid exception "Interface cannot be resolved: System.IServiceProvider"
    // when we try to resolve IAnalyticsTransmitter from the DI container.
    public class EmptyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) => throw new NotImplementedException();
    }
}
