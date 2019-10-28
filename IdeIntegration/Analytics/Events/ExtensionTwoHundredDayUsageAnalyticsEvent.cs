using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionTwoHundredDayUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionTwoHundredDayUsageAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "200 day usage";
    }
}