using System;
using System.Configuration;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.AppConfig
{
    public partial class GeneratorConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("dependencies", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(ContainerRegistrationCollection), AddItemName = "register")]
        public ContainerRegistrationCollection Dependencies
        {
            get { return (ContainerRegistrationCollection)this["dependencies"]; }
            set { this["dependencies"] = value; }
        }

        [ConfigurationProperty("allowDebugGeneratedFiles", DefaultValue = ConfigDefaults.AllowDebugGeneratedFiles, IsRequired = false)]
        public bool AllowDebugGeneratedFiles
        {
            get { return (bool)this["allowDebugGeneratedFiles"]; }
            set { this["allowDebugGeneratedFiles"] = value; }
        }

        [ConfigurationProperty("allowRowTests", DefaultValue = ConfigDefaults.AllowRowTests, IsRequired = false)]
        public bool AllowRowTests
        {
            get { return (bool)this["allowRowTests"]; }
            set { this["allowRowTests"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = ConfigDefaults.GeneratorPath, IsRequired = false)]
        public string GeneratorPath
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}