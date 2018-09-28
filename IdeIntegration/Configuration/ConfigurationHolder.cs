using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public class ConfigurationHolder : IConfigurationHolder
    {
        private readonly string xmlString;

        public ConfigSource ConfigSource { get; private set; }

        public string Content
        {
            get { return xmlString; }
        }

        public bool HasConfiguration
        {
            get { return !string.IsNullOrEmpty(xmlString); }
        }

        public ConfigurationHolder()
        {
            ConfigSource = ConfigSource.Default;
            xmlString = null;
        }

        public ConfigurationHolder(ConfigSource configSource, string content)
        {
            ConfigSource = configSource;
            xmlString = content;
        }

        public ConfigurationHolder(XmlNode configXmlNode)
        {
            xmlString = configXmlNode != null ? configXmlNode.OuterXml : null;
            ConfigSource = ConfigSource.AppConfig;
        }

        public SpecFlowConfigurationHolder TransformConfigurationToOldHolder()
        {
            return new SpecFlowConfigurationHolder(Content);
        }
    }
}
