using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionTenDayUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionTenDayUsageAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "10 day usage";
    }
}