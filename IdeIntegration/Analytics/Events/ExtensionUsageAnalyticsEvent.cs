using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionUsageAnalyticsEvent : AnalyticsEventBase
    {
        private readonly int _daysUsage;

        public ExtensionUsageAnalyticsEvent(DateTime utcDate, string userId, int daysUsage) : base(utcDate, userId)
        {
            _daysUsage = daysUsage;
        }

        public override string EventName => $"{_daysUsage} day usage";
    }
}
