using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class AppInsightsEventConverter : IAppInsightsEventConverter
    {
        public EventTelemetry ConvertToAppInsightsEvent(IAnalyticsEvent analyticsEvent)
        {
            var eventTelemetry = new EventTelemetry(analyticsEvent.EventName)
            {
                Timestamp = analyticsEvent.UtcDate,
                Properties =
                {
                    { "UserId", analyticsEvent.UserId },
                    { "UtcDate", analyticsEvent.UtcDate.ToString("O") },
                }
            };

            if (analyticsEvent is ExtensionInstalledAnalyticsEvent extensionInstalledAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionInstalledAnalyticsEvent.ExtensionVersion);
                eventTelemetry.Properties.Add("IdeVersion", extensionInstalledAnalyticsEvent.IdeVersion);
            }
            if (analyticsEvent is ExtensionLoadedAnalyticsEvent extensionLoadedAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionLoadedAnalyticsEvent.ExtensionVersion);
                eventTelemetry.Properties.Add("IdeVersion", extensionLoadedAnalyticsEvent.IdeVersion);
                eventTelemetry.Properties.Add("Ide", extensionLoadedAnalyticsEvent.Ide);
                eventTelemetry.Properties.Add("ProjectTargetFramework", string.Join(";", extensionLoadedAnalyticsEvent.ProjectTargetFrameworks));
            }
            if (analyticsEvent is ExtensionUpgradedAnalyticsEvent extensionUpgradeAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionUpgradeAnalyticsEvent.ExtensionVersion);
                eventTelemetry.Properties.Add("OldExtensionVersion", extensionUpgradeAnalyticsEvent.OldExtensionVersion);
            }

            return eventTelemetry;
        }

    }
}
