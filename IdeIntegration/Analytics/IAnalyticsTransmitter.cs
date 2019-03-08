namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitLogonEvent(string ide, string ideVersion, string extensionVersion);
    }
}
