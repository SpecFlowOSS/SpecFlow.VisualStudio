namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitExtensionLoadedEvent(string extensionVersion);
    }
}
