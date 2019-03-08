using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class LogonAnalyticsEvent : IAnalyticsEvent
    {
        public LogonAnalyticsEvent(DateTime utcDate, Guid userId, string ide, string ideVersion, string extensionVersion)
        {
            UtcDate = utcDate;
            UserId = userId;
            Ide = ide;
            IdeVersion = ideVersion;
            ExtensionVersion = extensionVersion;
        }

        public DateTime UtcDate { get; private set; }

        public Guid UserId { get; private set; }

        public string Ide { get; private set; }

        public string IdeVersion { get; private set; }

        public string ExtensionVersion { get; private set; }
    }
}
