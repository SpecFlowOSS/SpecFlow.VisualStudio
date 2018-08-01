using System.Runtime.Serialization;
using BoDi;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class GeneratorElement
    {
        [DataMember(Name = "dependencies")]
        public ContainerRegistrationCollection Dependencies { get; set; }

        [DataMember(Name = "path")]
        public string GeneratorPath { get; set; }
    }
}