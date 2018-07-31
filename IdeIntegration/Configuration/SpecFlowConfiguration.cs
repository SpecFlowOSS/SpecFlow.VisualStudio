using System.Collections.Generic;
using System.Globalization;

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
            List<string> additionalStepAssemblies)
        {
            ConfigSource = configSource;
            FeatureLanguage = featureLanguage;
            AdditionalStepAssemblies = additionalStepAssemblies;
        }

        public ConfigSource ConfigSource { get; set; }

        public CultureInfo FeatureLanguage { get; set; }

        public List<string> AdditionalStepAssemblies { get; set; }
    }
}
