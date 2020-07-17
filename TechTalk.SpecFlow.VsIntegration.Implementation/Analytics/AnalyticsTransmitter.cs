using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;
using TechTalk.SpecFlow.IdeIntegration.Install;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
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

        private IAnalyticsEvent CreateAnalyticsEvent(AnalyticsEventType analyticsEventType, string oldExtensionVersion = null, string selectedDotNetFramework = null, string selectedUnitTestFramework = null)
        {
            switch (analyticsEventType)
            {
                case AnalyticsEventType.ExtensionLoaded:
                    return new ExtensionLoadedAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value, _ideName.Value, _ideVersion.Value, _extensionVersion.Value, _targetFrameworks.Value);
                case AnalyticsEventType.ExtensionInstalled:
                    return new ExtensionInstalledAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value, _ideVersion.Value, _extensionVersion.Value);
                case AnalyticsEventType.ExtensionUpgraded:
                    return new ExtensionUpgradedAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value, oldExtensionVersion, _extensionVersion.Value);
                case AnalyticsEventType.ExtensionTenDayUsage:
                    return new ExtensionTenDayUsageAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value);
                case AnalyticsEventType.ExtensionOneHundredDayUsage:
                    return new ExtensionOneHundredDayUsageAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value);
                case AnalyticsEventType.ExtensionTwoHundredDayUsage:
                    return new ExtensionTwoHundredDayUsageAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value);
                case AnalyticsEventType.ProjectTemplateUsage:
                    return new ProjectTemplateUsageAnalyticsEvent(DateTime.UtcNow, _userUniqueId.Value, selectedDotNetFramework, selectedUnitTestFramework);
                default:
                    throw new ArgumentOutOfRangeException(nameof(analyticsEventType), analyticsEventType, null);
            }
        }

        private void TransmitAnalyticsEvent(AnalyticsEventType analyticsEventType, string oldExtensionVersion = null, string selectedDotNetFramework = null, string selectedUnitTestFramework = null)
        {
            try
            {
                if (!_enableAnalyticsChecker.IsEnabled())
                {
                    return;
                }

                var analyticsEvent = CreateAnalyticsEvent(analyticsEventType, oldExtensionVersion, selectedDotNetFramework, selectedUnitTestFramework);

                _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
            }
            catch (Exception ex)
            {
                TransmitException(ex);
            }
        }

        public void TransmitExtensionLoadedEvent()
        {
            TransmitAnalyticsEvent(AnalyticsEventType.ExtensionLoaded);
        }

        public void TransmitExtensionInstalledEvent()
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

        public void TransmitProjectTemplateUsage(string selectedDotNetFramework, string selectedUnitTestFramework)
        {
            TransmitAnalyticsEvent(AnalyticsEventType.ProjectTemplateUsage, null, selectedDotNetFramework, selectedUnitTestFramework);
        }

        private void TransmitException(Exception exception)
        {
            try
            {
                var exceptionAnalyticsEvent = new ExceptionAnalyticsEvent(exception.GetType().ToString(), DateTime.UtcNow);
                _analyticsTransmitterSink.TransmitEvent(exceptionAnalyticsEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the whole extension simply because data transmission failed
            }
        }
    }
}
