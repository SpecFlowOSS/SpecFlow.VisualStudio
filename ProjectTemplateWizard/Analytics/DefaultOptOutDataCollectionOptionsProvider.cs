using System;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace ProjectTemplateWizard.Analytics
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
