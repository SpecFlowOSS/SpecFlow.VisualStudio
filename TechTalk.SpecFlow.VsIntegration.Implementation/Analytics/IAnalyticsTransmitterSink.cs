using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        void TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
