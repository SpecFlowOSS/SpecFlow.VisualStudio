using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.VsIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.AutoComplete
{
    internal class GherkinCompletionCommandFilter : CompletionCommandFilter
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
            if (GherkinStepCompletionSource.IsStepLine(caret, languageService))
            {
                return ch == ' ' && GherkinStepCompletionSource.IsKeywordPrefix(caret - 1, languageService); 
            }

            return false;
        }
    }
}