using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using gherkin;
using Gherkin;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinDialectAdapter
    {
        private GherkinDialect GherkinDialect { get; }
        private string LangName { get; }

        public CultureInfo CultureInfo => new CultureInfo(LangName);

        //TODO: why did we use a different culture here? there was some magic mapping between e.g. de => de-de
        public CultureInfo CultureInfoForConversions => CultureInfo;

        internal GherkinDialectAdapter(string langName)
        {
            LangName = langName;
            GherkinDialect = new SpecFlowGherkinDialectProvider(langName).GetDialect(langName, new Location());
        }

        public override bool Equals(object obj)
        {
            GherkinDialectAdapter other = obj as GherkinDialectAdapter;

            return other != null && other.LangName.Equals(LangName);
        }

        public override int GetHashCode()
        {
            return LangName.GetHashCode();
        }

        public bool IsStepKeyword(string keyword)
        {
            return TryParseStepKeyword(keyword) != null;
        }

        public StepKeyword? TryParseStepKeyword(string keyword)
        {
            if (GherkinDialect.AndStepKeywords.Contains(keyword)) return StepKeyword.And;

            if (GherkinDialect.GivenStepKeywords.Contains(keyword)) return StepKeyword.Given;

            if (GherkinDialect.WhenStepKeywords.Contains(keyword)) return StepKeyword.When;
            
            if (GherkinDialect.ThenStepKeywords.Contains(keyword)) return StepKeyword.Then;
            
            if (GherkinDialect.ButStepKeywords.Contains(keyword)) return StepKeyword.But;

            //if (NativeLanguageService.keywords("and").contains(keyword))
            //    return StepKeyword.And;
            //// this is checked at the first place to interpret "*" as "and"

            //if (NativeLanguageService.keywords("given").contains(keyword))
            //    return StepKeyword.Given;

            //if (NativeLanguageService.keywords("when").contains(keyword))
            //    return StepKeyword.When;

            //if (NativeLanguageService.keywords("then").contains(keyword))
            //    return StepKeyword.Then;

            //if (NativeLanguageService.keywords("but").contains(keyword))
            //    return StepKeyword.But;

            // In Gherkin, the space at the end is also part of the keyword, becase in some 
            // languages, there is no space between the step keyword and the step text.
            // To support the keywords without leading space as well, we retry the matching with 
            // an additional space too.
            if (!keyword.EndsWith(" "))
                return TryParseStepKeyword(keyword + " ");

            return null;
        }

        public IEnumerable<string> GetKeywords()
        {
            return GetStepKeywords().Concat(GetBlockKeywords()).OrderBy(k => k);
        }

        public IEnumerable<string> GetBlockKeywords()
        {
            var keywords = Enum.GetValues(typeof(GherkinBlockKeyword)).Cast<GherkinBlockKeyword>().Aggregate(Enumerable.Empty<string>(),
                (current, stepKeyword) => current.Concat(GetBlockKeywords(stepKeyword)));
            return keywords.Distinct().OrderBy(k => k);
        }

        public IEnumerable<string> GetStepKeywords()
        {
            var keywords = Enum.GetValues(typeof(StepKeyword)).Cast<StepKeyword>().Aggregate(Enumerable.Empty<string>(),
                (current, stepKeyword) => current.Concat(GetStepKeywords(stepKeyword)));
            return keywords.Distinct().OrderBy(k => k);
        }

        public IEnumerable<string> GetStepKeywords(StepKeyword stepKeyword)
        {
            //return NativeLanguageService.keywords(stepKeyword.ToString().ToLowerInvariant()).toArray().Cast<string>();
            switch (stepKeyword)
            {
                case StepKeyword.Given: return GherkinDialect.GivenStepKeywords;
                case StepKeyword.When: return GherkinDialect.WhenStepKeywords;
                case StepKeyword.Then: return GherkinDialect.ThenStepKeywords;
                case StepKeyword.And: return GherkinDialect.AndStepKeywords;
                case StepKeyword.But: return GherkinDialect.ButStepKeywords;

                default: throw new InvalidOperationException($"Invalid StepKeyword {stepKeyword}");
            }
        }

        public IEnumerable<string> GetBlockKeywords(GherkinBlockKeyword blockKeyword)
        {
            //string key = blockKeyword.ToString().ToLowerInvariant();
            //if (blockKeyword == GherkinBlockKeyword.ScenarioOutline)
            //    key = "scenario_outline";

            //return NativeLanguageService.keywords(key).toArray().Cast<string>();

            switch (blockKeyword)
            {
                case GherkinBlockKeyword.Background: return GherkinDialect.BackgroundKeywords;
                case GherkinBlockKeyword.Feature: return GherkinDialect.FeatureKeywords;
                case GherkinBlockKeyword.Scenario: return GherkinDialect.ScenarioKeywords;
                case GherkinBlockKeyword.ScenarioOutline: return GherkinDialect.ScenarioOutlineKeywords;
                case GherkinBlockKeyword.Examples: return GherkinDialect.ExamplesKeywords;

                default: throw new InvalidOperationException($"Invalid GherkinBlockKeyword {blockKeyword}");
            }
        }
    }
}