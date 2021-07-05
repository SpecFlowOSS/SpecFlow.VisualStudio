using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardStartedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardStartedAnalyticsEvent(string ide, DateTime utcDate, string userId, string ideVersion) : base(ide, ideVersion, utcDate, userId)
        {
        }

        public override string EventName => "Project Template Wizard Started";
    }
}
