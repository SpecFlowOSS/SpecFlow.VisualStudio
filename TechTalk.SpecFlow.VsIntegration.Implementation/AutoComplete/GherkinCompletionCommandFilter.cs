using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.AutoComplete
{
    public class GherkinCompletionCommandFilter : CompletionCommandFilter
    {
        private readonly GherkinLanguageService languageService;

        public GherkinCompletionCommandFilter(IWpfTextView textView, ICompletionBroker broker, GherkinLanguageService languageService, IIdeTracer tracer) : base(textView, broker, tracer)
        {
            this.languageService = languageService;
        }

        /// <summary>
        /// Displays completion after typing a space after a step keyword
        /// </summary>
        protected override bool ShouldCompletionBeDiplayed(SnapshotPoint caret, char? ch)
        {
            if (ch == null)
                return true;

            if (GherkinStepCompletionSource.IsKeywordCompletion(caret))
            {
                return GherkinStepCompletionSource.IsKeywordPrefix(caret, languageService);
            }
            if (ch == ' ' && GherkinStepCompletionSource.IsStepLine(caret, languageService))
            {
                return GherkinStepCompletionSource.IsKeywordPrefix(caret - 1, languageService) // step completion
                    || GherkinStepCompletionSource.IsStepArgument(caret, languageService); // step argument completion
            }

            return false;
        }
    }
}