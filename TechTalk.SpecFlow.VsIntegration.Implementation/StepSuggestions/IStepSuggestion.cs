using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public interface IStepSuggestion<out TNativeSuggestionItem>
    {
        TNativeSuggestionItem NativeSuggestionItem { get; }
        StepDefinitionType StepDefinitionType { get; }
    }
}