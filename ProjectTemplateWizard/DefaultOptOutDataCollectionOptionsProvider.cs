using System;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Implementation.Options;

namespace ProjectTemplateWizard
{
    public class DefaultOptOutDataCollectionOptionsProvider: IIntegrationOptionsProvider
    {
        public IntegrationOptions GetOptions() => new IntegrationOptions
        {
            OptOutDataCollection = OptionDefaultValues.DefaultOptOutDataCollection
        };

        public void ClearCache()
        {
            throw new NotImplementedException();
        }
    }
}
