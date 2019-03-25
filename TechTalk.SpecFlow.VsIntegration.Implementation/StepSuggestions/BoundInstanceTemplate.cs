using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public class BoundInstanceTemplate<TNativeSuggestionItem> : IBoundStepSuggestion<TNativeSuggestionItem>, IStepSuggestionGroup<TNativeSuggestionItem>
    {
        public StepInstanceTemplate<TNativeSuggestionItem> Template { get; private set; }

        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        private readonly StepSuggestionList<TNativeSuggestionItem> suggestions;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get { return suggestions; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }
        public StepDefinitionType StepDefinitionType { get { return Template.StepDefinitionType; } }

        public BoundInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> template, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> suggestions)
        {
            Template = template;
            this.suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory, suggestions);
            NativeSuggestionItem = nativeSuggestionItemFactory.CloneTo(template.NativeSuggestionItem, this);
        }

        public CultureInfo Language
        {
            get { return Template.Language; } 
        }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            if (binding.StepDefinitionType != StepDefinitionType)
                return false;

            if (suggestions.Count == 0)
                return false;

            return suggestions.Any(i => i.Match(binding, bindingCulture, true, stepDefinitionMatchService));
        }
    }
}