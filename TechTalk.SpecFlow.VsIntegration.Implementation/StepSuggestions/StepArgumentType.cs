using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public class StepArgumentType
    {
        public IBindingType Type { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
