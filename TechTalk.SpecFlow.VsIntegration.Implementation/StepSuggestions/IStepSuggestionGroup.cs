using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public interface IStepSuggestionGroup<TNativeSuggestionItem>
    {
        IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get; }
    }
}