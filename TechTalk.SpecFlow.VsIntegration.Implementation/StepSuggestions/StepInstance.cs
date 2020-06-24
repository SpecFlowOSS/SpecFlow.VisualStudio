using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Gherkin.Ast;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
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

        public StepInstance(Step step, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, int level = 1)
            : base(GetStepDefType(step), GetStepDefKeyword(step), step.Keyword, step.Text, stepContext)
        {
            this.NativeSuggestionItem = nativeSuggestionItemFactory.Create(step.Text, GetInsertionText(step), level, StepDefinitionType.ToString().Substring(0, 1), this);
            this.Location = feature.Location;
            this.SourceFile = "TODO: find source file";
        }

        private static StepDefinitionType GetStepDefType(Step step)
        {
            switch (step.Keyword)
            {
                case "Given": return StepDefinitionType.Given;
                case "When": return StepDefinitionType.When;
                case "Then": return StepDefinitionType.Then;
                default: throw new Exception("Cannot convert to StepDefinitionType.");
            }
        }

        private static StepDefinitionKeyword GetStepDefKeyword(Step step)
        {
            switch (step.Keyword)
            {
                case "Given": return StepDefinitionKeyword.Given;
                case "When": return StepDefinitionKeyword.When;
                case "Then": return StepDefinitionKeyword.Then;
                case "And": return StepDefinitionKeyword.And;
                case "But": return StepDefinitionKeyword.But;
                default: throw new Exception("Cannot convert to StepDefinitionType.");
            }
        }

        private const string stepParamIndent = "         ";

        static internal string GetInsertionText(Step step)
        {
            //tablearg == gherkintable, which holds the header and the body (gherkintablerow)
            //if no table or not multiline, return the Text
            // "Scenario steps are defined using text and can have additional table (called DataTable) or multi-line text (called DocString) arguments."
            if (step.Argument == null)
                return step.Text;

            StringBuilder result = new StringBuilder(step.Text);

            switch (step.Argument)
            {
                case DocString docString:
                    result.AppendLine();
                    result.Append(stepParamIndent);
                    result.AppendLine("\"\"\"");
                    result.AppendLine(stepParamIndent);
                    result.Append(stepParamIndent);
                    result.Append("\"\"\"");
                    break;
                case DataTable dataTable:
                    //todo: headers?
                    result.AppendLine();
                    result.Append(stepParamIndent);
                    result.Append("|");
                    foreach (var dataTableRow in dataTable.Rows)
                    {
                        foreach (var cell in dataTableRow.Cells)
                        {
                            result.Append(" ");
                            result.Append(cell.Value);
                            result.Append(" |");
                        }
                    }
                    result.AppendLine();
                    result.Append(stepParamIndent);
                    result.Append("|");
                    foreach (var dataTableRow in dataTable.Rows)
                    {
                        foreach (var cell in dataTableRow.Cells)
                        {
                            result.Append(" ");
                            result.Append(' ', cell.Value.Length);
                            result.Append(" |");
                        }
                    }
                    break;
            }
            return result.ToString();
        }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            return stepDefinitionMatchService.Match(binding, this, bindingCulture, useRegexMatching: includeRegexCheck, useParamMatching: false).Success;
        }
    }
}