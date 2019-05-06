using Microsoft.ApplicationInsights.DataContracts;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public interface IAppInsightsEventConverter<TEvent> where TEvent : IAnalyticsEvent
    {
        EventTelemetry ConvertToAppInsightsEvent(TEvent analyticsEvent);
    }
}
