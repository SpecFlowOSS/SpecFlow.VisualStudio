using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException("jsonContent");

            var jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(jsonContent);

            CultureInfo featureLanguage = specFlowConfiguration.FeatureLanguage;
            CultureInfo bindingCulture = specFlowConfiguration.BindingCulture;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            bool usesPlugins = false;
            string generatorPath = null;
            
            if (jsonConfig.Language != null)
            {
                if (!String.IsNullOrWhiteSpace(jsonConfig.Language.Feature))
                {
                    featureLanguage = CultureInfo.GetCultureInfo(jsonConfig.Language.Feature);
                }
            }

            if (jsonConfig.BindingCulture != null)
            {
                if (!String.IsNullOrWhiteSpace(jsonConfig.BindingCulture.Name))
                {
                    featureLanguage = CultureInfo.GetCultureInfo(jsonConfig.BindingCulture.Name);
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
                    usesPlugins = true;
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
