using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardCompletedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardCompletedAnalyticsEvent(string ide, DateTime utcDate, string userId, string ideVersion, string selectedDotNetFramework, string selectedUnitTestFramework) : base(ide, ideVersion, utcDate, userId)
        {
            SelectedDotNetFramework = selectedDotNetFramework;
            SelectedUnitTestFramework = selectedUnitTestFramework;
        }

        public override string EventName => "Project Template Wizard Completed";
        
        public string SelectedUnitTestFramework { get; }

        public string SelectedDotNetFramework { get; }
    }
}
