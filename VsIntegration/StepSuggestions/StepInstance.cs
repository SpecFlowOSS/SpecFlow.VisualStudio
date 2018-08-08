using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gherkin.Ast;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public interface ISourceFilePosition
    {
        string SourceFile { get; }
        Location Location { get; }
    }

    public class StepInstance<TNativeSuggestionItem> : StepInstance, IBoundStepSuggestion<TNativeSuggestionItem>, ISourceFilePosition
    {
        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        public CultureInfo Language
        {
            get { return StepContext.Language; }
        }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }
        public StepInstanceTemplate<TNativeSuggestionItem> ParentTemplate { get; internal set; }

        public string SourceFile { get; private set; }

        public Location Location { get; private set; }


        public StepInstance(SpecFlowStep step, SpecFlowDocument specFlowDocument, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, int level = 1)
            : base((StepDefinitionType)step.ScenarioBlock, (StepDefinitionKeyword)step.StepKeyword, step.Keyword, step.Text, stepContext)
        {
            this.NativeSuggestionItem = nativeSuggestionItemFactory.Create(step.Text, GetInsertionText(step), level, StepDefinitionType.ToString().Substring(0, 1), this);
            this.Location = specFlowDocument.SpecFlowFeature.Location;
            this.SourceFile = specFlowDocument.SourceFilePath;
        }

        private const string stepParamIndent = "         ";

        internal static string GetInsertionText(SpecFlowStep step)
        {
            if (step.Argument == null)
                return step.Text;

            StringBuilder result = new StringBuilder(step.Text);
            var multiLineArg = step.Argument as DocString;
            if (multiLineArg != null)
            {
                result.AppendLine();
                result.Append(stepParamIndent);
                result.AppendLine("\"\"\"");
                result.AppendLine(stepParamIndent);
                result.Append(stepParamIndent);
                result.Append("\"\"\"");
            }

            var tableArg = step.Argument as DataTable;
            if (tableArg != null)
            {
                var header = tableArg.Rows.First();
                result.AppendLine();
                result.Append(stepParamIndent);
                result.Append("|");
                foreach (var cell in header.Cells)
                {
                    result.Append(" ");
                    result.Append(cell.Value);
                    result.Append(" |");
                }
                result.AppendLine();
                result.Append(stepParamIndent);
                result.Append("|");
                foreach (var cell in header.Cells)
                {
                    result.Append(" ");
                    result.Append(' ', cell.Value.Length);
                    result.Append(" |");
                }
            }
            return result.ToString();
        }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            return stepDefinitionMatchService.Match(binding, this, bindingCulture, useRegexMatching: includeRegexCheck, useParamMatching: false).Success;
        }
    }
}