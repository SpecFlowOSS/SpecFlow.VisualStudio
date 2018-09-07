using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class JsonConfig
    {
        //[JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "language")]
        public LanguageElement Language { get; set; }

        [DataMember(Name = "bindingCulture")]
        public BindingCultureElement BindingCulture { get; set; }

        //[JsonProperty("stepAssemblies", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "stepAssemblies")]
        public List<StepAssemblyElement> StepAssemblies { get; set; }

        [DataMember(Name = "trace")]
        public TraceElement Trace { get; set; }

        //[JsonProperty("generator", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "generator")]
        public GeneratorElement Generator { get; set; }

        [DataMember(Name = "unitTestProvider")]
        public UnitTestProviderElement UnitTestProvider { get; set; }
    }
}
