using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("gherkin")]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        internal IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        [Import]
        internal IGherkinBufferServiceManager GherkinBufferServiceManager = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (!IntegrationOptionsProvider.GetOptions().EnableOutlining)
                return null;

            return (ITagger<T>)GherkinBufferServiceManager.GetOrCreate(buffer, () =>
                new GherkinFileOutliningTagger(GherkinLanguageServiceFactory.GetLanguageService(buffer)));
        }
    }
}