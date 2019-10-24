using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionInstalledAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionInstalledAnalyticsEvent(DateTime utcDate, string userId, string ideVersion, string extensionVersion) : base(utcDate, userId)
        {
            IdeVersion = ideVersion;
            ExtensionVersion = extensionVersion;
        }

        public override string EventName => "Extension installed";

        public string IdeVersion { get; }
        public string ExtensionVersion { get; }
    }
}