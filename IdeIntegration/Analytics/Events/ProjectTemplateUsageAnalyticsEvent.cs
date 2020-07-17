using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateUsageAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateUsageAnalyticsEvent(DateTime utcDate, string userId, string selectedDotNetFramework, string selectedUnitTestFramework) : base(utcDate, userId)
        {
            SelectedDotNetFramework = selectedDotNetFramework;
            SelectedUnitTestFramework = selectedUnitTestFramework;
        }

        public override string EventName => "Project template usage";
        public string SelectedUnitTestFramework { get; }

        public string SelectedDotNetFramework { get; }
    }
}
