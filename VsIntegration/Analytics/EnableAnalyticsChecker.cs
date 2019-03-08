using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class EnableAnalyticsChecker : IEnableAnalyticsChecker
    {
        private readonly IIntegrationOptionsProvider _integrationOptionsProvider;

        public EnableAnalyticsChecker(IIntegrationOptionsProvider integrationOptionsProvider)
        {
            _integrationOptionsProvider = integrationOptionsProvider;
        }

        public bool IsEnabled()
        {
            var options = _integrationOptionsProvider.GetOptions();
            return !options.OptOutDataCollection;
        }
    }
}
