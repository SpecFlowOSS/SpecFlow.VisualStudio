using System;
using EnumsNET;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ProjectTemplateWizardStartedAnalyticsEvent : AnalyticsEventBase
    {
        public ProjectTemplateWizardStartedAnalyticsEvent(DateTime utcDate, string userId) : base(utcDate, userId)
        {
        }

        public override string EventName => "Project Template Wizard Started";
    }
}
