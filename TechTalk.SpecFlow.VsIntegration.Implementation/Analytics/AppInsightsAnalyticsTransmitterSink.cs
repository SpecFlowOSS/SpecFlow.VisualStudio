using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class AppInsightsAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly TelemetryClientWrapper _telemetryClientWrapper;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAppInsightsEventConverter _appInsightsEventConverter;

        public AppInsightsAnalyticsTransmitterSink(TelemetryClientWrapper telemetryClientWrapper, IEnableAnalyticsChecker enableAnalyticsChecker, IAppInsightsEventConverter appInsightsEventConverter)
        {
            _telemetryClientWrapper = telemetryClientWrapper;
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _appInsightsEventConverter = appInsightsEventConverter;
        }

        public void TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
            {
                throw new InvalidOperationException("This method should not be called because analytics transmission is disabled.");
            }

            var appInsightsEvent = _appInsightsEventConverter.ConvertToAppInsightsEvent(analyticsEvent);
            var telemetryClient = _telemetryClientWrapper.TelemetryClient;
            telemetryClient.TrackEvent(appInsightsEvent);
            telemetryClient.Flush();
        }
    }
}
