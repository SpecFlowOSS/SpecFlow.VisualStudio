using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
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
                throw new InvalidOperationException("This method should not be called because analytics transmission is disabled.");
            }

            Console.WriteLine(analyticsEvent.ToString());
        }
    }
}
