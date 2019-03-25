using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.AutoComplete
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("gherkin")]
    [Name("gherkinStepCompletion")]
    internal class GherkinStepCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;


        [Import]
        IVisualStudioTracer Tracer = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            var options = IntegrationOptionsProvider.GetOptions();
            if (!options.EnableIntelliSense)
                return null;

            bool limitStepInstancesSuggestions = options.LimitStepInstancesSuggestions;
            int maxStepSuggestions = options.MaxStepInstancesSuggestions;
            return new GherkinStepCompletionSource(textBuffer, GherkinLanguageServiceFactory.GetLanguageService(textBuffer), Tracer, limitStepInstancesSuggestions, maxStepSuggestions);
        }
    }
}