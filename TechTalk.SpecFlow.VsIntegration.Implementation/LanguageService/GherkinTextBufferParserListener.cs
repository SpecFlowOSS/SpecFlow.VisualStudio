using Gherkin;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class GherkinTextBufferParserListener : GherkinTextBufferParserListenerBase
    {
        public GherkinTextBufferParserListener(GherkinDialect gherkinDialect, ITextSnapshot textSnapshot, IProjectScope projectScope)
            : base(gherkinDialect, textSnapshot, projectScope)
        {
        }
    }
}
