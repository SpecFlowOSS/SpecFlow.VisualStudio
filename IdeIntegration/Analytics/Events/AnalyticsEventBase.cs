using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public abstract class AnalyticsEventBase : IAnalyticsEvent
    {
        protected AnalyticsEventBase(string ide, string ideVersion, DateTime utcDate, string userId)
        {
            Ide = ide;
            IdeVersion = ideVersion;
            UtcDate = utcDate;
            UserId = userId;
        }

        public abstract string EventName { get;  }
        public string Ide { get; }
        public string IdeVersion { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
    }
}
