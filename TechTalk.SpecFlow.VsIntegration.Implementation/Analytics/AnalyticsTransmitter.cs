using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;

        public AnalyticsTransmitter(IUserUniqueIdStore userUniqueIdStore, IEnableAnalyticsChecker enableAnalyticsChecker, IAnalyticsTransmitterSink analyticsTransmitterSink)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _analyticsTransmitterSink = analyticsTransmitterSink;
        }

        public void TransmitExtensionLoadedEvent(string ide, string ideVersion, string extensionVersion)
        {
            try
            {
                if (!_enableAnalyticsChecker.IsEnabled())
                {
                    return;
                }

                var userUniqueId = _userUniqueIdStore.Get();
                var logonAnalyticsEvent = new ExtensionLoadedAnalyticsEvent(DateTime.UtcNow, userUniqueId, ide, ideVersion, extensionVersion);
                _analyticsTransmitterSink.TransmitEvent(logonAnalyticsEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the whole extension simply because data transmission failed
            }
        }
    }
}
