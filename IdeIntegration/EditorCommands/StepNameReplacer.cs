using System;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.IdeIntegration.EditorCommands
{
    public class StepNameReplacer : IStepNameReplacer
    {
        public string BuildStepNameWithNewRegex(string stepName, string newStepRegex, IStepDefinitionBinding binding)
        {
            var originalMatch = Regex.Match(stepName, FormatRegexForDisplay(binding.Regex));

            var newRegexMatch = Regex.Match(newStepRegex, newStepRegex);
            // Regex pattern "the number is (\d+)" will not match the input "the number is (\d+)"
            // replace (\d+) to (.*) in the pattern so it will match
            if (!newRegexMatch.Success)
            {
                var newStepRegexForMatching = Regex.Replace(newStepRegex, @"\(.*?\)", "(.*)");
                newRegexMatch = Regex.Match(newStepRegex, newStepRegexForMatching);
            }

            // we cannot support the parameter number change,
            // because we will not know where to put the values
            if (originalMatch.Groups.Count != newRegexMatch.Groups.Count)
            {
                throw new NotSupportedException("Changing the number of parameters is not supported!");
            }

            var builder = new StringBuilder(newStepRegex);
            for (var i = newRegexMatch.Groups.Count - 1; i > 0; i--)
            {
                builder.Replace(newRegexMatch.Groups[i].Value, originalMatch.Groups[i].Value, newRegexMatch.Groups[i].Index, newRegexMatch.Groups[i].Length);
            }

            return RemoveDoubleQuotes(builder.ToString());
        }

        private static string FormatRegexForDisplay(Regex regex)
        {
            return RemoveDoubleQuotes(TrimLast(TrimFirst(regex.ToString())));
        }

        private static string RemoveDoubleQuotes(string value)
        {
            return value.Replace("\"\"", "\""); ;
        }

        private static string TrimFirst(string value)
        {
            return value.Remove(0, 1);
        }

        private static string TrimLast(string value)
        {
            return value.Remove(value.Length - 1, 1);
        }
    }
}
