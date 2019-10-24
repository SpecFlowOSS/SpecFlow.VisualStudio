﻿using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class AppInsightsAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly TelemetryClientWrapper _telemetryClientWrapper;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent> _appInsightsEventConverter;

        public AppInsightsAnalyticsTransmitterSink(TelemetryClientWrapper telemetryClientWrapper, IEnableAnalyticsChecker enableAnalyticsChecker, IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent> appInsightsEventConverter)
        {
            _telemetryClientWrapper = telemetryClientWrapper;
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

            var telemetryClient = _telemetryClientWrapper.TelemetryClient;
            telemetryClient.TrackEvent(appInsightsEvent);
            telemetryClient.Flush();
        }
    }
}
