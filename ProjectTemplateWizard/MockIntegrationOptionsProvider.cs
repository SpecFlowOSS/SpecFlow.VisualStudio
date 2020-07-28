using System;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace ProjectTemplateWizard
{
    public class MockIntegrationOptionsProvider: IIntegrationOptionsProvider
    {
        public IntegrationOptions GetOptions() => new IntegrationOptions
        {
            OptOutDataCollection = false
        };

        public void ClearCache()
        {
            throw new NotImplementedException();
        }
    }
}
