using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public class GherkinStep : StepInstance, IKeywordLine
    {
        public int BlockRelativeLine { get; set; }
        public BindingStatus BindingStatus { get; set; }

        public GherkinStep(StepDefinitionType stepDefinitionType, StepDefinitionKeyword stepDefinitionKeyword, string stepText, StepContext stepContext, string keyword, int blockRelativeLine)
            : base(stepDefinitionType, stepDefinitionKeyword, keyword, stepText, stepContext)
        {
            BlockRelativeLine = blockRelativeLine;
            BindingStatus = BindingStatus.UnknownBindingStatus;
        }
    }
}