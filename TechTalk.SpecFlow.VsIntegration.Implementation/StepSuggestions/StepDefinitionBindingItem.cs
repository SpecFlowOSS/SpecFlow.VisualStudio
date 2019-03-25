using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public class StepDefinitionBindingItem
    {
        public IBindingMethod Method { get; set; }
        public StepDefinitionType StepDefinitionType { get; set; }
        public Regex Regex { get; set; }
        public BindingScope BindingScope { get; set; }

        static public StepDefinitionBindingItem FromStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            return new StepDefinitionBindingItem()
            {
                Method = stepDefinitionBinding.Method,
                StepDefinitionType =  stepDefinitionBinding.StepDefinitionType,
                Regex = stepDefinitionBinding.Regex,
                BindingScope = stepDefinitionBinding.BindingScope
            };
        }

        public StepDefinitionBinding ToStepDefinitionBinding()
        {
            return new StepDefinitionBinding(StepDefinitionType, Regex, Method, BindingScope);
        }
    }
}