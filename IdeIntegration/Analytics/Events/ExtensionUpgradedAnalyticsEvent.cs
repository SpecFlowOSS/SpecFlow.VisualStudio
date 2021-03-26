using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionUpgradedAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionUpgradedAnalyticsEvent(string ide, DateTime utcDate, string userId, string oldExtensionVersion, string extensionVersion) : base(ide, utcDate, userId)
        {
            OldExtensionVersion = oldExtensionVersion;
            ExtensionVersion = extensionVersion;
        }

        public override string EventName => "Extension upgraded";

        public string OldExtensionVersion { get; }
        public string ExtensionVersion { get; }
    }
}