using System;
using EnumsNET;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardCompletedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardCompletedAnalyticsEvent(DateTime utcDate, string userId, string selectedDotNetFramework, string selectedUnitTestFramework) : base(utcDate, userId)
        {
            SelectedDotNetFramework = selectedDotNetFramework;
            SelectedUnitTestFramework = selectedUnitTestFramework;
        }

        public override string EventName => AnalyticsEventType.ProjectTemplateWizardCompleted.AsString(EnumFormat.Description, EnumFormat.Name);
        
        public string SelectedUnitTestFramework { get; }

        public string SelectedDotNetFramework { get; }
    }
}
