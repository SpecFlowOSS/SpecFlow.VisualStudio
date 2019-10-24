using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionOneHundredDayUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionOneHundredDayUsageAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "100 day usage";
    }
}