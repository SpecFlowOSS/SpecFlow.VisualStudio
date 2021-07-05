using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAppInsightsEventConverter
    {
        EventTelemetry ConvertToAppInsightsEvent(IAnalyticsEvent analyticsEvent);
    }
}
