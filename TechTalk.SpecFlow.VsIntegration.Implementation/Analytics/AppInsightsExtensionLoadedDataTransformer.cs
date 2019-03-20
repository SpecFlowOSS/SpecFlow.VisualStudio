using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class AppInsightsExtensionLoadedDataTransformer : IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent>
    {
        public EventTelemetry ConvertToAppInsightsEvent(ExtensionLoadedAnalyticsEvent analyticsEvent)
        {
            return new EventTelemetry("Extension loaded")
            {
                Timestamp = analyticsEvent.UtcDate,
                Properties =
                {
                    { "UserId", analyticsEvent.UserId.ToString("B") },
                    { "UtcDate", analyticsEvent.UtcDate.ToString("O") },
                    { "ExtensionVersion", analyticsEvent.ExtensionVersion },
                    { "Ide", analyticsEvent.Ide },
                    { "IdeVersion", analyticsEvent.IdeVersion },
                    { "ProjectTargetFramework", string.Join(";", analyticsEvent.ProjectTargetFrameworks) }
                }
            };
        }
    }
}
