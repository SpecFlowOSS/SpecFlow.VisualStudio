namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitExtensionLoadedEvent();
        void TransmitExtensionInstallatedEvent();
        void TransmitExtensionUpgradedEvent(string oldExtensionVersion);
        void TransmitExtensionUsage(int daysOfUsage);
    }
}
