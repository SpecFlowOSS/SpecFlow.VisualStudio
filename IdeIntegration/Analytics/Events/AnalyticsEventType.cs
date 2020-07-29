using System.ComponentModel;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public enum AnalyticsEventType
    {
        ExtensionLoaded,
        ExtensionInstalled,
        ExtensionUpgraded,
        ExtensionTenDayUsage,
        ExtensionOneHundredDayUsage,
        ExtensionTwoHundredDayUsage,
        [Description("Project Template Wizard Completed")]
        ProjectTemplateWizardCompleted,
    }
}