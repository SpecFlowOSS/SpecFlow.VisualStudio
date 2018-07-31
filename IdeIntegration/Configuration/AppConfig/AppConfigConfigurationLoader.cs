using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.AppConfig
{
    public class AppConfigConfigurationLoader
    {
        public SpecFlowConfiguration LoadAppConfig(SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            CultureInfo featureLanguage = specFlowConfiguration.FeatureLanguage;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;

            if (IsSpecified(configSection.Language))
            {
                featureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement) element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            return new SpecFlowConfiguration(ConfigSource.AppConfig,
                featureLanguage,
                additionalStepAssemblies
            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
