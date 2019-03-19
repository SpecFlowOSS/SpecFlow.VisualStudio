using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;
        private readonly IIdeInformationStore _ideInformationStore;
        private readonly IProjectTargetFrameworksProvider _projectTargetFrameworksProvider;

        public AnalyticsTransmitter(IUserUniqueIdStore userUniqueIdStore, IEnableAnalyticsChecker enableAnalyticsChecker, IAnalyticsTransmitterSink analyticsTransmitterSink, IIdeInformationStore ideInformationStore, IProjectTargetFrameworksProvider projectTargetFrameworksProvider)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _analyticsTransmitterSink = analyticsTransmitterSink;
            _ideInformationStore = ideInformationStore;
            _projectTargetFrameworksProvider = projectTargetFrameworksProvider;
        }

        public void TransmitExtensionLoadedEvent(string extensionVersion)
        {
            try
            {
                if (!_enableAnalyticsChecker.IsEnabled())
                {
                    return;
                }

                var userUniqueId = _userUniqueIdStore.Get();
                string ideName = _ideInformationStore.GetName();
                string ideVersion = _ideInformationStore.GetVersion();
                var targetFrameworks = _projectTargetFrameworksProvider.GetProjectTargetFrameworks();
                var extensionLoadedAnalyticsEvent = new ExtensionLoadedAnalyticsEvent(DateTime.UtcNow, userUniqueId, ideName, ideVersion, extensionVersion, targetFrameworks);
                _analyticsTransmitterSink.TransmitExtensionLoadedEvent(extensionLoadedAnalyticsEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the whole extension simply because data transmission failed
            }
        }
    }
}
