using System;
using Microsoft.ApplicationInsights;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class AppInsightsAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent> _appInsightsEventConverter;

        public AppInsightsAnalyticsTransmitterSink(TelemetryClientWrapper telemetryClientWrapper, IEnableAnalyticsChecker enableAnalyticsChecker, IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent> appInsightsEventConverter)
        {
            _telemetryClient = telemetryClientWrapper.TelemetryClient;
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _appInsightsEventConverter = appInsightsEventConverter;
        }

        public void TransmitExtensionLoadedEvent(ExtensionLoadedAnalyticsEvent extensionLoadedAnalyticsEvent)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
            {
                throw new InvalidOperationException("This method should not be called because analytics transmission is disabled.");
            }

            var appInsightsEvent = _appInsightsEventConverter.ConvertToAppInsightsEvent(extensionLoadedAnalyticsEvent);
            _telemetryClient.TrackEvent(appInsightsEvent);
            _telemetryClient.Flush();
        }
    }
}
