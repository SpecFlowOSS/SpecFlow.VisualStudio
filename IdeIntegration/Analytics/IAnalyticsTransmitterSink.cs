using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        void TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
