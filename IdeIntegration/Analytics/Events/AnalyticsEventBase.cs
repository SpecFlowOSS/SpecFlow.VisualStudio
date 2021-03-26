using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public abstract class AnalyticsEventBase : IAnalyticsEvent
    {
        protected AnalyticsEventBase(string ide, DateTime utcDate, string userId)
        {
            Ide = ide;
            UtcDate = utcDate;
            UserId = userId;
        }

        public abstract string EventName { get;  }
        public string Ide { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
    }
}
