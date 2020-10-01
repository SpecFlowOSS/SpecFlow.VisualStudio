using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionTwentyDayUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionTwentyDayUsageAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "20 day usage";
    }
}