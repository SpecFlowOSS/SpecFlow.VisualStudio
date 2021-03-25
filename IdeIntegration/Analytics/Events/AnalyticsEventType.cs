using System.ComponentModel;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public enum AnalyticsEventType
    {
        ExtensionLoaded,
        ExtensionInstalled,
        ExtensionUpgraded,
        ExtensionUsage,
        [Description("Project Template Wizard Started")]
        ProjectTemplateWizardStarted,
        [Description("Project Template Wizard Completed")]
        ProjectTemplateWizardCompleted,
    }
}