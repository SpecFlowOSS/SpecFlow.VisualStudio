using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        void TransmitExtensionLoadedEvent(ExtensionLoadedAnalyticsEvent extensionLoadedAnalyticsEvent);
    }
}
