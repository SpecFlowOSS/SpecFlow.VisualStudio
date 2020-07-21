using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class GherkinTextBufferParserListener : GherkinTextBufferParserListenerBase
    {
        public GherkinTextBufferParserListener(GherkinDialectAdapter gherkinDialectAdapter, ITextSnapshot textSnapshot, IProjectScope projectScope)
            : base(gherkinDialectAdapter, textSnapshot, projectScope)
        {
        }
    }
}
