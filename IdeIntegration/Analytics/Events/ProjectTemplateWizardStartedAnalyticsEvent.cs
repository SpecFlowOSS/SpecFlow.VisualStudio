using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardStartedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardStartedAnalyticsEvent(string ide, DateTime utcDate, string userId) : base(ide, utcDate, userId)
        {
        }

        public override string EventName => "Project Template Wizard Started";
    }
}
