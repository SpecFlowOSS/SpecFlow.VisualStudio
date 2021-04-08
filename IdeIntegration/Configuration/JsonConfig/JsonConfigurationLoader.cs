using System;
using System.Globalization;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                // TODO: dei clarify whether to upgrade to C# 6 since support for VS2013 has been dropped
                throw new ArgumentNullException("jsonContent");
            }

            var jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(jsonContent);

            var featureLanguage = specFlowConfiguration.FeatureLanguage;
            var bindingCulture = specFlowConfiguration.BindingCulture;
            var additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            var stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            bool usesPlugins = false;
            string generatorPath = null;
            
            if (jsonConfig.Language != null)
            {
                if (!string.IsNullOrWhiteSpace(jsonConfig.Language.Feature))
                {
                    featureLanguage = CultureInfo.GetCultureInfo(jsonConfig.Language.Feature);
                }
            }

            if (jsonConfig.BindingCulture != null)
            {
                if (!string.IsNullOrWhiteSpace(jsonConfig.BindingCulture.Name))
                {
                    bindingCulture = CultureInfo.GetCultureInfo(jsonConfig.BindingCulture.Name);
                }
            }

            if (jsonConfig.StepAssemblies != null)
            {
                foreach (var stepAssemblyEntry in jsonConfig.StepAssemblies)
                {
                    additionalStepAssemblies.Add(stepAssemblyEntry.Assembly);
                }
            }

            if (jsonConfig.Trace != null)
            {
                stepDefinitionSkeletonStyle = jsonConfig.Trace.StepDefinitionSkeletonStyle;
            }

            if (jsonConfig.Generator != null)
            {
                generatorPath = jsonConfig.Generator.GeneratorPath;
                if (jsonConfig.Generator.Dependencies != null)
                {
                    usesPlugins = true;
                }
            }

            if(jsonConfig.UnitTestProvider != null && !string.IsNullOrEmpty(jsonConfig.UnitTestProvider.GeneratorProvider))
            {
                usesPlugins = true;
            }

            return new SpecFlowConfiguration(ConfigSource.Json,
                                            featureLanguage,
                                            bindingCulture,
                                            additionalStepAssemblies,
                                            stepDefinitionSkeletonStyle,
                                            usesPlugins,
                                            generatorPath);
        }
    }
}
