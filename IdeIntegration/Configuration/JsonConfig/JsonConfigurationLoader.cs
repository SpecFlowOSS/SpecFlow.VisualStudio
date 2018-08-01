using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;

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

            var specFlowElement = jsonConfig.SpecFlow;
            if (specFlowElement.Language != null)
            {
                if (!String.IsNullOrWhiteSpace(specFlowElement.Language.Feature))
                {
                    featureLanguage = CultureInfo.GetCultureInfo(specFlowElement.Language.Feature);
                }
            }

            if (specFlowElement.BindingCulture != null)
            {
                if (!String.IsNullOrWhiteSpace(specFlowElement.BindingCulture.Name))
                {
                    featureLanguage = CultureInfo.GetCultureInfo(specFlowElement.BindingCulture.Name);
                }
            }

            if (specFlowElement.StepAssemblies != null)
            {
                foreach (var stepAssemblyEntry in specFlowElement.StepAssemblies)
                {
                    additionalStepAssemblies.Add(stepAssemblyEntry.Assembly);
                }
            }

            if (specFlowElement.Trace != null)
            {
                stepDefinitionSkeletonStyle = specFlowElement.Trace.StepDefinitionSkeletonStyle;
            }

            if (specFlowElement.Generator != null)
            {
                generatorPath = specFlowElement.Generator.GeneratorPath;
                if (specFlowElement.Generator.Dependencies != null)
                    usesPlugins = true;
            }

            if(specFlowElement.UnitTestProvider != null && !string.IsNullOrEmpty(specFlowElement.UnitTestProvider.GeneratorProvider))
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
