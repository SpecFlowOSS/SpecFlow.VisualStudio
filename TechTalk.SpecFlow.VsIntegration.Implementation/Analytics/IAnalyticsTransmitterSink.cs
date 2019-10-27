using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        void TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
