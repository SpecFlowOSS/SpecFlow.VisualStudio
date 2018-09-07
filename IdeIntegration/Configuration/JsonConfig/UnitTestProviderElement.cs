using System.ComponentModel;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class UnitTestProviderElement
    {
        //[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("NUnit")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "generatorProvider")]
        public string GeneratorProvider { get; set; }
    }
}