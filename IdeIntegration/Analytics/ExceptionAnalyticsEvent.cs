using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class ExceptionAnalyticsEvent : IAnalyticsEvent
    {
        public ExceptionAnalyticsEvent(string eventName, DateTime utcDate)
        {
            EventName = eventName;
            UtcDate = utcDate;
            UserId = null;
        }

        public string EventName { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
    }
}