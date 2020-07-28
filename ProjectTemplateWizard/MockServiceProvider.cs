using System;

namespace ProjectTemplateWizard
{
    public class MockIServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) => throw new NotImplementedException();
    }
}
