using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;
using TechTalk.SpecFlow.IdeIntegration.Install;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;

        private readonly Lazy<string> _userUniqueId; 
        private readonly Lazy<string> _ideName;
        private readonly Lazy<string> _ideVersion;
        private readonly Lazy<IEnumerable<string>> _targetFrameworks;
        private readonly Lazy<string> _extensionVersion;

        public AnalyticsTransmitter(IUserUniqueIdStore userUniqueIdStore, IEnableAnalyticsChecker enableAnalyticsChecker, IAnalyticsTransmitterSink analyticsTransmitterSink, IIdeInformationStore ideInformationStore, IProjectTargetFrameworksProvider projectTargetFrameworksProvider, ICurrentExtensionVersionProvider currentExtensionVersionProvider)
        {
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _analyticsTransmitterSink = analyticsTransmitterSink;

            _userUniqueId = new Lazy<string>(userUniqueIdStore.GetUserId);
            _ideName = new Lazy<string>(ideInformationStore.GetName);
            _ideVersion = new Lazy<string>(ideInformationStore.GetVersion);
            _targetFrameworks = new Lazy<IEnumerable<string>>(projectTargetFrameworksProvider.GetProjectTargetFrameworks);
            _extensionVersion = new Lazy<string>(() => currentExtensionVersionProvider.GetCurrentExtensionVersion().ToString());
        }

        private void Execute(Func<IAnalyticsEvent> createEvent)
        {
            try
            {
                if (!_enableAnalyticsChecker.IsEnabled())
                {
                    return;
                }

                var analyticsEvent = createEvent();

                _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
            }
            catch (Exception ex)
            {
                TransmitException(ex);
            }

        }

        public void TransmitExtensionLoadedEvent()
        {
            Execute(() =>
                new ExtensionLoadedAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, _ideVersion.Value, _extensionVersion.Value, _targetFrameworks.Value));
        }

        public void TransmitExtensionInstalledEvent()
        {
            Execute(() =>
                new ExtensionInstalledAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, _ideVersion.Value, _extensionVersion.Value));
        }

        public void TransmitExtensionUpgradedEvent(string oldExtensionVersion)
        {
            Execute(() =>
                new ExtensionUpgradedAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, oldExtensionVersion, _extensionVersion.Value));
        }

        public void TransmitExtensionUsage(int daysOfUsage)
        {
            Execute(() =>
                new ExtensionUsageAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, daysOfUsage));
        }

        public void TransmitProjectTemplateWizardStartedEvent()
        {
            Execute(() =>
                new ProjectTemplateWizardStartedAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value));
        }

        public void TransmitProjectTemplateWizardCompletedEvent(string selectedDotNetFramework, string selectedUnitTestFramework)
        {
            Execute(() =>
                new ProjectTemplateWizardCompletedAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, selectedDotNetFramework, selectedUnitTestFramework));
        }

        public void TransmitNotificationShownEvent(string notificationId)
        {
            Execute(() =>
                new NotificationShownAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, notificationId));
        }

        public void TransmitNotificationLinkOpenedEvent(string notificationId)
        {
            Execute(() =>
                        new NotificationLinkOpenedAnalyticsEvent(_ideName.Value, DateTime.UtcNow, _userUniqueId.Value, notificationId));
        }

        private void TransmitException(Exception exception)
        {
            try
            {
                var exceptionAnalyticsEvent = new ExceptionAnalyticsEvent(_ideName.Value, exception.GetType().ToString(), DateTime.UtcNow);
                _analyticsTransmitterSink.TransmitEvent(exceptionAnalyticsEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the whole extension simply because data transmission failed
            }
        }
    }
}
