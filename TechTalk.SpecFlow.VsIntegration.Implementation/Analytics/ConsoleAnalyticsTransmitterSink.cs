using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class ConsoleAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;

        public ConsoleAnalyticsTransmitterSink(IEnableAnalyticsChecker enableAnalyticsChecker)
        {
            _enableAnalyticsChecker = enableAnalyticsChecker;
        }

        public void TransmitExtensionLoadedEvent(ExtensionLoadedAnalyticsEvent extensionLoadedAnalyticsEvent)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
            {
                throw new InvalidOperationException("This method should not be called because analytics transmission is disabled.");
            }

            Console.WriteLine(extensionLoadedAnalyticsEvent.ToString());
        }
    }
}
