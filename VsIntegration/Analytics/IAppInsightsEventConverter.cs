using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public interface IAppInsightsEventConverter<TEvent> where TEvent : IAnalyticsEvent
    {
        EventTelemetry ConvertToAppInsightsEvent(TEvent analyticsEvent);
    }
}
