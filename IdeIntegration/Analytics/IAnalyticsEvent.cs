using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsEvent
    {
        string EventName { get; }
        DateTime UtcDate { get; }
        string UserId { get; }
    }
}
