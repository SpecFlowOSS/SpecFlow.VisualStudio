using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public abstract class AnalyticsEventBase : IAnalyticsEvent
    {
        protected AnalyticsEventBase(DateTime utcDate, string userId)
        {
            UtcDate = utcDate;
            UserId = userId;
        }

        public abstract string EventName { get;  }
        public DateTime UtcDate { get; }
        public string UserId { get; }
    }
}
