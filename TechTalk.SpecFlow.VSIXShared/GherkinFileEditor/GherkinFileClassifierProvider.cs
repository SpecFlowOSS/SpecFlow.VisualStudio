using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        [Import]
        internal IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        internal IGherkinBufferServiceManager GherkinBufferServiceManager = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (!IntegrationOptionsProvider.GetOptions().EnableSyntaxColoring)
                return null;

            return GherkinBufferServiceManager.GetOrCreate(buffer, () => 
                new GherkinFileClassifier(GherkinLanguageServiceFactory.GetLanguageService(buffer)));
        }
    }
}