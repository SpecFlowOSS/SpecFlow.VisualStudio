using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.AutoComplete
{
    public class GherkinStepCompletionSource : ICompletionSource
    {
        private bool disposed = false;
        private readonly ITextBuffer textBuffer;
        private readonly GherkinLanguageService languageService;
        private readonly IIdeTracer tracer;
        private readonly bool limitStepInstancesSuggestions;
        private readonly int maxStepInstancesSuggestions;

        public GherkinStepCompletionSource(ITextBuffer textBuffer, GherkinLanguageService languageService, IIdeTracer tracer, bool limitStepInstancesSuggestions, int maxStepInstancesSuggestions)
        {
            this.textBuffer = textBuffer;
            this.languageService = languageService;
            this.tracer = tracer;
            this.limitStepInstancesSuggestions = limitStepInstancesSuggestions;
            this.maxStepInstancesSuggestions = maxStepInstancesSuggestions;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (disposed)
                throw new ObjectDisposedException("GherkinStepCompletionSource");

            ITextSnapshot snapshot = textBuffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(snapshot);
            if (triggerPoint == null)
                return;

            string statusText = null;
            CustomCompletionSet completionSet = null;

            if (IsKeywordCompletion(triggerPoint.Value))
            {
                IEnumerable<Completion> completions = GetKeywordCompletions();
                ITrackingSpan applicableTo = GetApplicableToForKeyword(snapshot, triggerPoint.Value);

                completionSet = new CustomCompletionSet(
                    "Keywords",
                    "Keywords",
                    applicableTo,
                    completions,
                    null);
            }

            GherkinStep stepAtTriggerPoint = null;
            if (completionSet == null)
            {
                // check for step argument completion
                SnapshotSpan? stepArgumentSpan;
                IBindingType stepArgumentType = GetCurrentStepArgumentType(triggerPoint.Value, languageService, out stepArgumentSpan, out stepAtTriggerPoint);
                if (stepArgumentType != null && stepArgumentSpan != null)
                {
                    IEnumerable<Completion> completions;
                    GetCompletionsForStepArgumentType(stepArgumentType.FullName, out completions, out statusText);

                    ITrackingSpan applicableTo = snapshot.CreateTrackingSpan(
                        stepArgumentSpan.Value, SpanTrackingMode.EdgeInclusive);

                    string displayName = string.Format("All {0} Values", stepArgumentType.Name);
                    completionSet = new CustomCompletionSet(
                        displayName,
                        displayName,
                        applicableTo,
                        completions,
                        null);
                }
            }

            if (completionSet == null)
            {
                // check for step completion
                string parsedKeyword;
                var bindingType = GetCurrentBindingType(triggerPoint.Value, stepAtTriggerPoint, out parsedKeyword);
                if (bindingType != null)
                {
                    IEnumerable<Completion> completions;
                    GetCompletionsForBindingType(bindingType.Value, out completions, out statusText);

                    ITrackingSpan applicableTo = GetApplicableToForStep(snapshot, triggerPoint.Value, parsedKeyword);

                    string displayName = string.Format("All {0} Steps", bindingType.Value);
                    completionSet = new HierarchicalCompletionSet(
                        displayName,
                        displayName,
                        applicableTo,
                        completions,
                        null,
                        limitStepInstancesSuggestions,
                        maxStepInstancesSuggestions);
                }
            }

            if (completionSet != null)
            {
                if (!string.IsNullOrEmpty(statusText))
                    completionSet.StatusText = statusText;
                completionSets.Add(completionSet);
            }
        }

        private IEnumerable<Completion> GetKeywordCompletions()
        {
            GherkinDialect dialect = GetDialect(languageService);
            return dialect.GetStepKeywords().Select(k => new Completion(k.Trim(), k.Trim(), null, null, null)).Concat(
                dialect.GetBlockKeywords().Select(k => new Completion(k.Trim(), k.Trim() + ": ", null, null, null)));
        }

        static private GherkinDialect GetDialect(GherkinLanguageService languageService)
        {
            var fileScope = languageService.GetFileScope(waitForResult: false);
            return fileScope != null ? fileScope.GherkinDialect : languageService.ProjectScope.GherkinDialectServices.GetDefaultDialect();
        }

        static internal bool IsKeywordCompletion(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref start, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            return start == triggerPoint;
        }

        static private string GetFirstWord(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return null;

            return triggerPoint.Snapshot.GetText(start, end.Position - start);
        }

        //HACK: this is a hotfix to support "Gegeben sei" 'Given' keyword in German
        static private string GetFirstTwoWords(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref end, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return null;

            return triggerPoint.Snapshot.GetText(start, end.Position - start);
        }

        static internal bool IsStepLine(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            var keywordCandidate = GetFirstWord(triggerPoint);
            if (keywordCandidate == null)
                return false;
            GherkinDialect dialect = GetDialect(languageService);
            if (dialect == null)
                return false;

            if (dialect.IsStepKeyword(keywordCandidate))
                return true;

            keywordCandidate = GetFirstTwoWords(triggerPoint);
            if (keywordCandidate == null)
                return false;
            return dialect.IsStepKeyword(keywordCandidate);
        }

        static internal bool IsKeywordPrefix(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return true; // returns true for empty word

            end = triggerPoint;
//            if (end < triggerPoint)
//                return false;

            var firstWord = triggerPoint.Snapshot.GetText(start, end.Position - start);
            GherkinDialect dialect = GetDialect(languageService);
            return dialect.GetKeywords().Any(k => k.StartsWith(firstWord, StringComparison.CurrentCultureIgnoreCase));
        }

        static internal bool IsStepArgument(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            SnapshotSpan? stepArgumentSpan;
            GherkinStep stepAtTriggerPoint;
            return GetCurrentStepArgumentType(triggerPoint, languageService, out stepArgumentSpan, out stepAtTriggerPoint) != null;
        }


        private ITrackingSpan GetApplicableToForKeyword(ITextSnapshot snapshot, SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private ITrackingSpan GetApplicableToForStep(ITextSnapshot snapshot, SnapshotPoint triggerPoint, string parsedKeyword)
        {
            var line = triggerPoint.GetContainingLine();

            SnapshotPoint keywordEnd = line.Start;
            ForwardWhile(ref keywordEnd, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            if (parsedKeyword != null)
                keywordEnd += parsedKeyword.Length;
            else
                ForwardWhile(ref keywordEnd, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));

            var start = keywordEnd;
            if (start < triggerPoint)
                ForwardWhile(ref start, start + 1, p => char.IsWhiteSpace(p.GetChar()));

            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private static void ForwardWhile(ref SnapshotPoint point, SnapshotPoint triggerPoint, Predicate<SnapshotPoint> predicate)
        {
            while (point < triggerPoint && predicate(point))
                point += 1;
        }

        private StepDefinitionType? GetCurrentBindingType(SnapshotPoint triggerPoint, GherkinStep stepAtTriggerPoint, out string parsedKeyword)
        {
            parsedKeyword = null;
            var fileScope = languageService.GetFileScope(waitForParsingSnapshot: triggerPoint.Snapshot);
            if (fileScope == null)
                return null;

            if (stepAtTriggerPoint != null)
            {
                parsedKeyword = stepAtTriggerPoint.Keyword.TrimEnd();
                return stepAtTriggerPoint.StepDefinitionType;
            }

            if (!IsStepLine(triggerPoint, languageService))
                return null;

            // this is a step line that just started. we need to calculate the binding type from
            // the keyword and the context
            var keywordCandidate = GetFirstWord(triggerPoint);
            if (keywordCandidate == null)
                return null;

            GherkinDialect dialect = GetDialect(languageService);
            var stepKeyword = dialect.TryParseStepKeyword(keywordCandidate);
            if (stepKeyword == null)
            {
                keywordCandidate = GetFirstTwoWords(triggerPoint);
                if (keywordCandidate != null)
                {
                    stepKeyword = dialect.TryParseStepKeyword(keywordCandidate);
                }

                if (stepKeyword == null)
                    return null;
            }

            parsedKeyword = keywordCandidate;

            if (stepKeyword == StepKeyword.Given)
                return StepDefinitionType.Given;
            if (stepKeyword == StepKeyword.When)
                return StepDefinitionType.When;
            if (stepKeyword == StepKeyword.Then)
                return StepDefinitionType.Then;

            parsedKeyword = null;
            // now we need the context
            var triggerLineNumber = triggerPoint.Snapshot.GetLineNumberFromPosition(triggerPoint.Position);
            var stepBlock = fileScope.GetStepBlockFromStepPosition(triggerLineNumber);
            var lastStep = stepBlock.Steps.LastOrDefault(s => s.BlockRelativeLine + stepBlock.KeywordLine < triggerLineNumber);
            if (lastStep == null)
                return StepDefinitionType.Given;
            return lastStep.StepDefinitionType;
        }

        private void GetCompletionsForBindingType(StepDefinitionType stepDefinitionType, out IEnumerable<Completion> completions, out string statusText)
        {
            statusText = null;

            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
            {
                completions = Enumerable.Empty<Completion>();
                return;
            }

            if (!suggestionProvider.Populated)
            {
                string percentText = string.Format("({0}% completed)", suggestionProvider.GetPopulationPercent());
                statusText = (!suggestionProvider.BindingsPopulated ? "step suggestion list is being populated... " : "step suggestion list from existing feature files is being populated... ") + percentText;
            }

            try
            {
                completions = suggestionProvider.GetNativeSuggestionItems(stepDefinitionType);
            }
            catch(Exception)
            {
                //fallback case
                completions = Enumerable.Empty<Completion>();
            }
        }

        static private IBindingType GetCurrentStepArgumentType(SnapshotPoint triggerPoint, GherkinLanguageService languageService,  out SnapshotSpan? stepArgumentSpan, out GherkinStep stepAtTriggerPoint)
        {
            stepArgumentSpan = null;
            stepAtTriggerPoint = null;

            var fileScope = languageService.GetFileScope(waitForParsingSnapshot: triggerPoint.Snapshot);
            if (fileScope == null)
                return null;

            var triggerLineNumber = triggerPoint.Snapshot.GetLineNumberFromPosition(triggerPoint.Position);
            stepAtTriggerPoint = fileScope.GetStepAtPosition(triggerLineNumber);
            if (stepAtTriggerPoint == null)
                return null;

            // todo: this will not find a match if the step argument is at the end of the binding definition,
            //       because trailing whitespace is already removed from step.Text
            Infrastructure.StepDefinitionAmbiguityReason ambuguitiyReason;
            List<BindingMatch> matches;
            BindingMatch bindingMatch = languageService.ProjectScope.BindingMatchService.GetBestMatch(
                stepAtTriggerPoint,
                fileScope.GherkinDialect.CultureInfo,
                out ambuguitiyReason,
                out matches);

            if (!bindingMatch.Success)
                return null;

            // todo: this regex will only match with the current step definition, if it also captures
            //       the default step argument insertion (as "{argName}" ).
            //       This is e.g. the case when the step argument is caputed as (.*), but not when using (Value1|Value2).
            Regex regex = bindingMatch.StepBinding.Regex;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint stepTextStart = line.Start;
            ForwardWhile(ref stepTextStart, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            stepTextStart += stepAtTriggerPoint.Keyword.Length;
            string stepText = line.Snapshot.GetText(stepTextStart, line.End - stepTextStart);

            int triggerPointIndexInStepText = triggerPoint.Position - stepTextStart;
            var m = regex.Match(stepText);
            if (!m.Success || m.Groups.Count <= 1)
                return null;

            // group at index 0 matches the entire regular expression pattern
            // so start searching for step parameters at index 1
            for (int g = 1; g < m.Groups.Count; g++)
            {
                Group group = m.Groups[g];
                if (group.Captures.Count > 0)
                {
                    Capture capture = group.Captures[0];
                    if (triggerPointIndexInStepText >= capture.Index &&
                        triggerPointIndexInStepText <= capture.Index + capture.Length &&
                        bindingMatch.StepBinding.Method.Parameters.Count() >= g)
                    {
                        stepArgumentSpan = new SnapshotSpan(stepTextStart + capture.Index, capture.Length);
                        return bindingMatch.StepBinding.Method.Parameters.ElementAt(g-1).Type;
                    }
                }
            }

            return null;
        }

        private void GetCompletionsForStepArgumentType(string fullTypeName, out IEnumerable<Completion> completions, out string statusText)
        {
            statusText = null;

            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
            {
                completions = Enumerable.Empty<Completion>();
                return;
            }

            if (!suggestionProvider.BindingsPopulated)
            {
                string percentText = string.Format("({0}% completed)", suggestionProvider.GetPopulationPercent());
                statusText = "step argument suggestion list is being populated... " + percentText;
            }

            try
            {
                completions = suggestionProvider.GetNativeStepArgumentSuggestionItems(fullTypeName);
            }
            catch (Exception)
            {
                //fallback case
                completions = Enumerable.Empty<Completion>();
            }
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}

