using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Configuration.AppConfig;
using TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        private readonly AppConfigConfigurationLoader _appConfigConfigurationLoader;
        
        private readonly JsonConfigurationLoader _jsonConfigurationLoader;

        public ConfigurationLoader()
        {
            _jsonConfigurationLoader = new JsonConfigurationLoader();
            _appConfigConfigurationLoader = new AppConfigConfigurationLoader();
        }

        private static CultureInfo DefaultFeatureLanguage
        {
            get { return CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage); }
        }

        private static CultureInfo DefaultBindingCulture
        {
            get { return null; }
        }

        private static List<string> DefaultAdditionalStepAssemblies
        {
            get { return new List<string>(); }
        }

        private static StepDefinitionSkeletonStyle DefaultStepDefinitionSkeletonStyle
        {
            get { return StepDefinitionSkeletonStyle.RegexAttribute; }
        }

        private static bool DefaultUsesPlugins
        {
            get { return false; }
        }
        private static string DefaultGeneratorPath
        {
            get { return null; }
        }

        public SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration, IConfigurationHolder configurationHolder)
        {
            switch (configurationHolder.ConfigSource)
            {
                case ConfigSource.AppConfig:
                    return LoadAppConfig(specFlowConfiguration,
                        ConfigurationSectionHandler.CreateFromXml(configurationHolder.Content));
                case ConfigSource.Json:
                    return LoadJson(specFlowConfiguration, configurationHolder.Content);
                case ConfigSource.Default:
                    return GetDefault();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            return _jsonConfigurationLoader.LoadJson(specFlowConfiguration, jsonContent);
        }

        public static SpecFlowConfiguration GetDefault()
        {
            return new SpecFlowConfiguration(ConfigSource.Default, 
                DefaultFeatureLanguage, 
                DefaultBindingCulture,
                DefaultAdditionalStepAssemblies,
                DefaultStepDefinitionSkeletonStyle,
                DefaultUsesPlugins,
                DefaultGeneratorPath);
        }

        private SpecFlowConfiguration LoadAppConfig(SpecFlowConfiguration specFlowConfiguration,
            ConfigurationSectionHandler specFlowConfigSection)
        {
            return _appConfigConfigurationLoader.LoadAppConfig(specFlowConfiguration, specFlowConfigSection);
        }
    }

}
