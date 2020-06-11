using System;
using System.IO;
using System.Linq;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{

    public abstract class FileBasedSpecFlowConfigurationReader : IConfigurationReader
    {
        protected readonly IIdeTracer tracer;

        protected FileBasedSpecFlowConfigurationReader(IIdeTracer tracer)
        {
            this.tracer = tracer;
        }

        public ConfigurationHolder ReadConfiguration()
        {
            string configFileContent = GetConfigFileContent();
            if (configFileContent == null)
                return new ConfigurationHolder();
            return GetConfigurationHolderFromFileContent(configFileContent);
        }


        protected virtual string GetConfigFileContent()
        {
            var configFilePath = GetConfigFilePath();
            if (configFilePath == null)
                return null;
            try
            {
                return File.ReadAllText(configFilePath);
            }
            catch (Exception ex)
            {
                tracer.Trace("Config file load error: " + ex, GetType().Name);
                return null;
            }
        }

        protected virtual string GetConfigFilePath()
        {
            throw new NotImplementedException();
        }

        private ConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
        {
            if (IsConfigXml(configFileContent))
            {
                try
                {
                    XmlDocument configDocument = new XmlDocument();
                    configDocument.LoadXml(configFileContent);

                    return new ConfigurationHolder(configDocument.SelectSingleNode("/configuration/specFlow"));
                }
                catch (Exception ex)
                {
                    tracer.Trace("Config load error: " + ex, GetType().Name);
                    return new ConfigurationHolder();
                }
            }

            if (IsConfigJson(configFileContent))
                return new ConfigurationHolder(ConfigSource.Json, configFileContent);

            throw new Exception("Config file is not recognized as json nor app.config!");
        }

        private bool IsConfigJson(string configContent)
        {
            return configContent.StartsWith("{") || configContent.StartsWith("[");
        }

        private bool IsConfigXml(string configContent)
        {
            return configContent.StartsWith("<");
        }

    }
}
