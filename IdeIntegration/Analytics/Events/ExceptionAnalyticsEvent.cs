using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExceptionAnalyticsEvent : IAnalyticsEvent
    {
        public ExceptionAnalyticsEvent(string ide, string ideVersion, string exceptionType, DateTime utcDate)
        {
            Ide = ide;
            IdeVersion = ideVersion;
            ExceptionType = exceptionType;
            UtcDate = utcDate;
            UserId = null;
        }

        public string EventName => "Visual Studio Extension Exception";
        public string Ide { get; }
        public string IdeVersion { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
        public string ExceptionType { get; set; }
    }
}