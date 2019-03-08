using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class ErrorAnalyticsEvent : IAnalyticsEvent
    {
        public ErrorAnalyticsEvent(DateTime utcDate, Guid userId, Exception exception)
        {
            UtcDate = utcDate;
            UserId = userId;
            Exception = exception;
        }

        public DateTime UtcDate { get; private set; }

        public Guid UserId { get; private set; }

        public Exception Exception { get; private set; }
    }
}
