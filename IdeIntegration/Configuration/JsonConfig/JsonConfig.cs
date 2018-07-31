using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration.JsonConfig
{
    public class JsonConfig
    {
        //[JsonProperty(PropertyName = "specflow", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "specflow")]
        public SpecFlowElement SpecFlow { get; set; }
    }
}
