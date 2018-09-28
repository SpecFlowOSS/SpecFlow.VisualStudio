using System.Configuration;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.AppConfig
{
    public class TraceConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("stepDefinitionSkeletonStyle", IsRequired = false, DefaultValue = StepDefinitionSkeletonStyle.RegexAttribute)]
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle
        {
            get { return (StepDefinitionSkeletonStyle)this["stepDefinitionSkeletonStyle"]; }
            set { this["stepDefinitionSkeletonStyle"] = value; }
        }
    }
}