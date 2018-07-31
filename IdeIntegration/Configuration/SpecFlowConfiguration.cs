using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public enum ConfigSource
    {
        AppConfig,
        Json,
        Default
    }

    public enum ObsoleteBehavior
    {
        None = 0,
        Warn = 1,
        Pending = 2,
        Error = 3
    }

    public class SpecFlowConfiguration
    {
        public SpecFlowConfiguration(ConfigSource configSource,
            CultureInfo featureLanguage,
            CultureInfo bindingCulture,
            List<string> additionalStepAssemblies,
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle)
        {
            ConfigSource = configSource;
            FeatureLanguage = featureLanguage;
            BindingCulture = bindingCulture;
            AdditionalStepAssemblies = additionalStepAssemblies;
            StepDefinitionSkeletonStyle = stepDefinitionSkeletonStyle;
        }

        public ConfigSource ConfigSource { get; set; }

        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        public List<string> AdditionalStepAssemblies { get; set; }

        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }
    }
}
