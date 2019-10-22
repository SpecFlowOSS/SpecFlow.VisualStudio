using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public interface IAnalyticsEvent
    {
        DateTime UtcDate { get; }
        string UserId { get; }
    }
}
