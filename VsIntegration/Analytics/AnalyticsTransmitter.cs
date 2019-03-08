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

        public void TransmitLogonEvent(string ide, string ideVersion, string extensionVersion)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
            {
                return;
            }
            
            var userUniqueId = _userUniqueIdStore.Get();
            var logonAnalyticsEvent = new LogonAnalyticsEvent(DateTime.UtcNow, userUniqueId, ide, ideVersion, extensionVersion);
            _analyticsTransmitterSink.TransmitEvent(logonAnalyticsEvent);
        }
    }
}
