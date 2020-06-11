using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
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
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle,
            bool usesPlugins,
            string generatorPath
            )
        {
            ConfigSource = configSource;
            FeatureLanguage = featureLanguage;
            BindingCulture = bindingCulture;
            AdditionalStepAssemblies = additionalStepAssemblies;
            StepDefinitionSkeletonStyle = stepDefinitionSkeletonStyle;
            UsesPlugins = usesPlugins;
            GeneratorPath = generatorPath;
        }

        public ConfigSource ConfigSource { get; set; }

        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        public List<string> AdditionalStepAssemblies { get; set; }

        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        //old stuff
        public bool UsesPlugins { get; set; }
        public string GeneratorPath { get; set; }
    }
}
