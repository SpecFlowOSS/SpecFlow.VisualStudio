using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;
using TechTalk.SpecFlow.IdeIntegration.Install;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;
        private readonly IIdeInformationStore _ideInformationStore;
        private readonly IProjectTargetFrameworksProvider _projectTargetFrameworksProvider;
        private readonly ICurrentExtensionVersionProvider _currentExtensionVersionProvider;

        public AnalyticsTransmitter(IUserUniqueIdStore userUniqueIdStore, IEnableAnalyticsChecker enableAnalyticsChecker, IAnalyticsTransmitterSink analyticsTransmitterSink, IIdeInformationStore ideInformationStore, IProjectTargetFrameworksProvider projectTargetFrameworksProvider, ICurrentExtensionVersionProvider currentExtensionVersionProvider)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _analyticsTransmitterSink = analyticsTransmitterSink;
            _ideInformationStore = ideInformationStore;
            _projectTargetFrameworksProvider = projectTargetFrameworksProvider;
            _currentExtensionVersionProvider = currentExtensionVersionProvider;
        }

        private IAnalyticsEvent CreateAnalyticsEvent(AnalyticsEventType analyticsEventType, string oldExtensionVersion = null)
        {
            var userUniqueId = new Lazy<string>(_userUniqueIdStore.GetUserId);
            var ideName = new Lazy<string>(_ideInformationStore.GetName);
            var ideVersion = new Lazy<string>(_ideInformationStore.GetVersion);
            var targetFrameworks = new Lazy<IEnumerable<string>>(_projectTargetFrameworksProvider.GetProjectTargetFrameworks);
            var extensionVersion = new Lazy<string>(_currentExtensionVersionProvider.GetCurrentExtensionVersion().ToString);

            switch (analyticsEventType)
            {
                case AnalyticsEventType.ExtensionLoaded:
                    return new ExtensionLoadedAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value, ideName.Value, ideVersion.Value, extensionVersion.Value, targetFrameworks.Value);
                case AnalyticsEventType.ExtensionInstalled:
                    return new ExtensionInstalledAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value, ideVersion.Value, extensionVersion.Value);
                case AnalyticsEventType.ExtensionUpgraded:
                    return new ExtensionUpgradedAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value, oldExtensionVersion, extensionVersion.Value);
                case AnalyticsEventType.ExtensionTenDayUsage:
                    return new ExtensionTenDayUsageAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value);
                case AnalyticsEventType.ExtensionOneHundredDayUsage:
                    return new ExtensionOneHundredDayUsageAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value);
                case AnalyticsEventType.ExtensionTwoHundredDayUsage:
                    return new ExtensionTwoHundredDayUsageAnalyticsEvent(DateTime.UtcNow, userUniqueId.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(analyticsEventType), analyticsEventType, null);
            }
        }

        private void TransmitAnalyticsEvent(AnalyticsEventType analyticsEventType, string oldExtensionVersion = null)
        {
            try
            {
                if (!_enableAnalyticsChecker.IsEnabled())
                {
                    return;
                }

                var analyticsEvent = CreateAnalyticsEvent(analyticsEventType, oldExtensionVersion);

                _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the whole extension simply because data transmission failed
            }
        }

        public void TransmitExtensionLoadedEvent()
        {
            TransmitAnalyticsEvent(AnalyticsEventType.ExtensionLoaded);
        }

        public void TransmitExtensionIstallatedEvent()
        {
            TransmitAnalyticsEvent(AnalyticsEventType.ExtensionInstalled);
        }

        public void TransmitExtensionUpgradedEvent(string oldExtensionVersion)
        {
            TransmitAnalyticsEvent(AnalyticsEventType.ExtensionUpgraded, oldExtensionVersion);
        }

        public void TransmitExtensionUsage(int daysOfUsage)
        {
            switch (daysOfUsage)
            {
                case InstallServices.AFTER_RAMP_UP_DAYS:
                    TransmitAnalyticsEvent(AnalyticsEventType.ExtensionTenDayUsage);
                    break;
                case InstallServices.EXPERIENCED_DAYS:
                    TransmitAnalyticsEvent(AnalyticsEventType.ExtensionOneHundredDayUsage);
                    break;
                case InstallServices.VETERAN_DAYS:
                    TransmitAnalyticsEvent(AnalyticsEventType.ExtensionTwoHundredDayUsage);
                    break;
                default:
                    break;
            }
        }
    }
}
