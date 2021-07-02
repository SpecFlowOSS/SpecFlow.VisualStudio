using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
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
                    { "Ide", analyticsEvent.Ide },
                    { "IdeVersion", analyticsEvent.IdeVersion },
                }
            };

            if (analyticsEvent is ExceptionAnalyticsEvent exceptionAnalyticsEvent)
            {
                eventTelemetry.Properties.Remove("UserId");
                eventTelemetry.Properties.Add("ExceptionType", exceptionAnalyticsEvent.ExceptionType);
                return eventTelemetry;
            }
            if (analyticsEvent is ExtensionInstalledAnalyticsEvent extensionInstalledAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionInstalledAnalyticsEvent.ExtensionVersion);
            }
            if (analyticsEvent is ExtensionLoadedAnalyticsEvent extensionLoadedAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionLoadedAnalyticsEvent.ExtensionVersion);
                eventTelemetry.Properties.Add("ProjectTargetFramework", string.Join(";", extensionLoadedAnalyticsEvent.ProjectTargetFrameworks));
            }
            if (analyticsEvent is ExtensionUpgradedAnalyticsEvent extensionUpgradeAnalyticsEvent)
            {
                eventTelemetry.Properties.Add("ExtensionVersion", extensionUpgradeAnalyticsEvent.ExtensionVersion);
                eventTelemetry.Properties.Add("OldExtensionVersion", extensionUpgradeAnalyticsEvent.OldExtensionVersion);
            }
            if (analyticsEvent is ProjectTemplateWizardCompletedAnalyticsEvent projectTemplateWizardCompleted)
            {
                eventTelemetry.Properties.Add("SelectedDotNetFramework", projectTemplateWizardCompleted.SelectedDotNetFramework);
                eventTelemetry.Properties.Add("SelectedUnitTestFramework", projectTemplateWizardCompleted.SelectedUnitTestFramework);
            }

            return eventTelemetry;
        }

    }
}
