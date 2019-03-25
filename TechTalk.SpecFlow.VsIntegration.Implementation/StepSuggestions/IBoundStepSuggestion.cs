using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public interface IBoundStepSuggestion<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        CultureInfo Language { get; }

        bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService);
        ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get; }
    }
}