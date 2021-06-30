using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public interface IAppInsightsEventConverter
    {
        EventTelemetry ConvertToAppInsightsEvent(IAnalyticsEvent analyticsEvent);
    }
}
