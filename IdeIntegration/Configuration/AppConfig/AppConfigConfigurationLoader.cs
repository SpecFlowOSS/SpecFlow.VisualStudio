using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.BindingSkeletons;
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
            CultureInfo bindingCulture = specFlowConfiguration.BindingCulture;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;

            if (IsSpecified(configSection.Language))
            {
                featureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                bindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement) element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            if (IsSpecified(configSection.Trace))
            {
                stepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            return new SpecFlowConfiguration(ConfigSource.AppConfig,
                featureLanguage,
                bindingCulture,
                additionalStepAssemblies,
                stepDefinitionSkeletonStyle
            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
