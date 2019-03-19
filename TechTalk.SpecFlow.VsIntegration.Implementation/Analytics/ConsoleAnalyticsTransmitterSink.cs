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

        public void TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
            {
                throw new InvalidOperationException("This method may not be called when analytics transmission is disabled.");
            }

            Console.WriteLine(@"Simulating transmission of {0}", analyticsEvent);
        }
    }
}
