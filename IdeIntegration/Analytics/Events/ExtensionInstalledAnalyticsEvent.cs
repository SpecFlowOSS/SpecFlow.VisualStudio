using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionInstalledAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionInstalledAnalyticsEvent(string ide, DateTime utcDate, string userId, string ideVersion, string extensionVersion) : base(ide, ideVersion, utcDate, userId)
        {
            ExtensionVersion = extensionVersion;
        }

        public override string EventName => "Extension installed";
        
        public string ExtensionVersion { get; }
    }
}