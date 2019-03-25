using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public interface IStepSuggestionGroup<TNativeSuggestionItem>
    {
        IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get; }
    }
}