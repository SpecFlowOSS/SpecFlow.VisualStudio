using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.Utils
{
    public static class GherkinDialectExtensions
    {
        public static CultureInfo GetCultureInfo(this GherkinDialect gherkinDialect)
        {
            return CultureInfo.GetCultureInfo(gherkinDialect.Language);
        }

        public static IEnumerable<string> GetStepKeywords(this GherkinDialect gherkinDialect)
        {
            var stepKeywords = gherkinDialect.StepKeywords;
            
            return stepKeywords.Distinct().OrderBy(k => k);
        }
        public static IEnumerable<string> GetKeywords(this GherkinDialect gherkinDialect)
        {
            //originally it was concatenated with the BloskKeywords, but there no similar like that
            var stepKeywords = gherkinDialect.StepKeywords;
            
            return stepKeywords.Distinct().OrderBy(k => k);
        }

        public static bool IsStepKeyword(this GherkinDialect gherkinDialect, string keyword)
        {
            return TryParseStepKeyword(gherkinDialect, keyword) != null;
        }

        public static StepKeyword? TryParseStepKeyword(this GherkinDialect dialect, string stepKeyword)
        {
            if (dialect.AndStepKeywords.Contains(stepKeyword)) // we need to check "And" first, as the '*' is also part of the Given, When and Then keywords
                return StepKeyword.And;
            if (dialect.GivenStepKeywords.Contains(stepKeyword))
                return StepKeyword.Given;
            if (dialect.WhenStepKeywords.Contains(stepKeyword))
                return StepKeyword.When;
            if (dialect.ThenStepKeywords.Contains(stepKeyword))
                return StepKeyword.Then;
            if (dialect.ButStepKeywords.Contains(stepKeyword))
                return StepKeyword.But;

            return null;
        }
    }
}
