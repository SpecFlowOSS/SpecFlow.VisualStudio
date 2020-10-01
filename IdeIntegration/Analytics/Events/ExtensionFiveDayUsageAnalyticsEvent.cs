using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionFiveDayUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionFiveDayUsageAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "5 day usage";
    }
}
