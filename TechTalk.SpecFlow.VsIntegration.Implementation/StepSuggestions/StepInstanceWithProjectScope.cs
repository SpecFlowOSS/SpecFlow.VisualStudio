using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    internal class StepInstanceWithProjectScope
    {
        public StepInstance StepInstance { get; private set; }
        public VsProjectScope ProjectScope { get; private set; }

        public StepInstanceWithProjectScope(StepInstance stepInstance, VsProjectScope projectScope)
        {
            StepInstance = stepInstance;
            ProjectScope = projectScope;
        }
    }
}
