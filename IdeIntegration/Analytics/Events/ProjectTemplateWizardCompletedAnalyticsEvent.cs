using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardCompletedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardCompletedAnalyticsEvent(string ide, DateTime utcDate, string userId, string selectedDotNetFramework, string selectedUnitTestFramework) : base(ide, utcDate, userId)
        {
            SelectedDotNetFramework = selectedDotNetFramework;
            SelectedUnitTestFramework = selectedUnitTestFramework;
        }

        public override string EventName => "Project Template Wizard Completed";
        
        public string SelectedUnitTestFramework { get; }

        public string SelectedDotNetFramework { get; }
    }
}
