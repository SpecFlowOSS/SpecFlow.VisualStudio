using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionUsageAnalyticsEvent : AnalyticsEventBase
    {
        private readonly int _daysUsage;

        public ExtensionUsageAnalyticsEvent(string ide, DateTime utcDate, string userId, string ideVersion, int daysUsage) : base(ide, ideVersion, utcDate, userId)
        {
            _daysUsage = daysUsage;
        }

        public override string EventName => $"{_daysUsage} day usage";
    }
}
