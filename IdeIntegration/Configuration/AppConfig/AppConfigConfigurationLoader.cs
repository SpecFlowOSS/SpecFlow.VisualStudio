using System;
using System.Configuration;
using System.Globalization;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.AppConfig
{
    public class AppConfigConfigurationLoader
    {
        public SpecFlowConfiguration LoadAppConfig(SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler configSection)
        {
            if (configSection == null)
            {
                // TODO: dei clarify whether to upgrade to C# 6 since support for VS2013 has been dropped
                throw new ArgumentNullException("configSection");
            }

            var featureLanguage = specFlowConfiguration.FeatureLanguage;
            var bindingCulture = specFlowConfiguration.BindingCulture;
            var additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            var stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            bool usesPlugins = false;
            string generatorPath = null;

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
                string assemblyName = ((StepAssemblyConfigElement) element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            if (IsSpecified(configSection.Trace))
            {
                stepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            if (IsSpecified(configSection.Generator))
            {
                generatorPath = configSection.Generator.GeneratorPath;
                if (IsSpecified(configSection.Generator.Dependencies))
                {
                    usesPlugins = true;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider) && !string.IsNullOrEmpty(configSection.UnitTestProvider.GeneratorProvider))
            {
                usesPlugins = true;
            }

            return new SpecFlowConfiguration(ConfigSource.AppConfig,
                featureLanguage,
                bindingCulture,
                additionalStepAssemblies,
                stepDefinitionSkeletonStyle,
                usesPlugins,
                generatorPath
            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
