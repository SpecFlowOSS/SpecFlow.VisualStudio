using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public interface IStepSuggestion<out TNativeSuggestionItem>
    {
        TNativeSuggestionItem NativeSuggestionItem { get; }
        StepDefinitionType StepDefinitionType { get; }
    }
}