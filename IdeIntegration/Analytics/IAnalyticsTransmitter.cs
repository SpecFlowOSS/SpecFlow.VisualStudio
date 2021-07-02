using System.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitExtensionLoadedEvent();
        void TransmitExtensionInstalledEvent();
        void TransmitExtensionUpgradedEvent(string oldExtensionVersion);
        void TransmitExtensionUsage(int daysOfUsage);

        void TransmitProjectTemplateWizardStartedEvent();
        void TransmitProjectTemplateWizardCompletedEvent(string selectedDotNetFramework, string selectedUnitTestFramework);

        void TransmitNotificationShownEvent(string notificationId);
        void TransmitNotificationLinkOpenedEvent(string notificationId);
    }
}
