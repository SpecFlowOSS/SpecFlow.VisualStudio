using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public class StepInstanceTemplate<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        private readonly StepSuggestionList<TNativeSuggestionItem> instances;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Instances { get { return instances; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }

        public StepDefinitionType StepDefinitionType { get; private set; }
        internal string StepPrefix { get; private set; }

        public CultureInfo Language { get; private set; }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            if (binding.StepDefinitionType != StepDefinitionType)
                return false;

            if (instances.Count == 0)
                return false;

            return instances.Any(i => i.Match(binding, bindingCulture, true, stepDefinitionMatchService));
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");

        public StepInstanceTemplate(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            StepDefinitionType = (StepDefinitionType)scenarioStep.ScenarioBlock;
            Language = stepContext.Language;

            NativeSuggestionItem = nativeSuggestionItemFactory.Create(scenarioStep.Text, StepInstance<TNativeSuggestionItem>.GetInsertionText(scenarioStep), 1, StepDefinitionType.ToString().Substring(0, 1) + "-t", this);
            instances = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
            AddInstances(scenarioStep, scenarioOutline, feature, stepContext, nativeSuggestionItemFactory);

            var match = paramRe.Match(scenarioStep.Text);
            StepPrefix = match.Success ? scenarioStep.Text.Substring(0, match.Index) : scenarioStep.Text;
        }

        private void AddInstances(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
            {
                foreach (var row in exampleSet.Table.Body)
                {
                    var replacedText = paramRe.Replace(scenarioStep.Text,
                        match =>
                        {
                            string param = match.Groups["param"].Value;
                            int headerIndex = Array.FindIndex(exampleSet.Table.Header.Cells, c => c.Value.Equals(param));
                            if (headerIndex < 0)
                                return match.Value;
                            return row.Cells[headerIndex].Value;
                        });

                    var newStep = scenarioStep.Clone();
                    newStep.Text = replacedText;
                    instances.Add(new StepInstance<TNativeSuggestionItem>(newStep, feature, stepContext, nativeSuggestionItemFactory, 2) { ParentTemplate = this });
                }
            }
        }

        static public bool IsTemplate(ScenarioStep scenarioStep)
        {
            return paramRe.Match(scenarioStep.Text).Success;
        }
    }
}