using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.VsIntegration.StepSuggestions;

namespace TechTalk.SpecFlow.VsIntegration.AutoComplete
{
    internal class HierarchicalCompletionSet : CustomCompletionSet
    {
        public HierarchicalCompletionSet(int maxStepInstances)
        {
            PrefixMatch = false;
            _maxStepInstances = maxStepInstances;
        }

        public HierarchicalCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders, int maxStepInstances)
            : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
            PrefixMatch = false;
            _maxStepInstances = maxStepInstances;
        }

        private readonly int _maxStepInstances;

        protected override bool DoesCompletionMatchApplicabilityText(Completion completion, string filterText, CompletionMatchType matchType, bool caseSensitive)
        {
            if (base.DoesCompletionMatchApplicabilityText(completion, filterText, matchType, caseSensitive))
                return true;

            object parentObject;
            completion.Properties.TryGetProperty("parentObject", out parentObject);
            IStepSuggestionGroup<Completion> parentObjectAsGroup = parentObject as IStepSuggestionGroup<Completion>;
            return 
                parentObjectAsGroup != null && 
                parentObjectAsGroup.Suggestions
                    .Any(stepSuggestion => stepSuggestion.NativeSuggestionItem != null && DoesCompletionMatchApplicabilityText(stepSuggestion.NativeSuggestionItem, filterText, matchType, caseSensitive));
        }

        protected override Predicate<Completion> GetFilterPredicate(string filterText, CompletionMatchType matchType, bool caseSensitive)
        {
            var basePredicate = base.GetFilterPredicate(filterText, matchType, caseSensitive);
            return LimitStepInstances(_maxStepInstances, basePredicate);
        }

        /// <summary>
        /// Returns predicate to limit quantity of step instances suggestions for each step template
        /// </summary>
        private Predicate<Completion> LimitStepInstances(int stepInstancesCount, Predicate<Completion> basePredicate)
        {
            int stepInstancesCounter = 0;

            return completion =>
            {
                if (!basePredicate.Invoke(completion))
                {
                    return false;
                }

                //If completion is step instance - increase counter, else - reset.
                object parentObject;
                completion.Properties.TryGetProperty("parentObject", out parentObject);
                bool isStepInstance = parentObject is StepInstance;

                if (isStepInstance)
                {
                    stepInstancesCounter++;
                }
                else
                {
                    stepInstancesCounter = 0;
                }

                return stepInstancesCounter <= stepInstancesCount;
            };
        }
    }
}