using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;

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
