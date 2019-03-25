using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.VsIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    #region Provider definition

    #endregion

    internal class GherkinFileOutliningTagger : ITagger<IOutliningRegionTag>, IDisposable
    {
        private readonly GherkinLanguageService gherkinLanguageService;

        public GherkinFileOutliningTagger(GherkinLanguageService gherkinLanguageService)
        {
            this.gherkinLanguageService = gherkinLanguageService;

            gherkinLanguageService.FileScopeChanged += GherkinLanguageServiceOnFileScopeChanged;
        }

        private void GherkinLanguageServiceOnFileScopeChanged(object sender, GherkinFileScopeChange gherkinFileScopeChange)
        {
            if (TagsChanged != null)
            {
                SnapshotSpanEventArgs args = new SnapshotSpanEventArgs(gherkinFileScopeChange.CreateChangeSpan());
                TagsChanged(this, args);
            }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var fileScope = gherkinLanguageService.GetFileScope(waitForResult: false);
            if (fileScope == null)
                return new ITagSpan<IOutliningRegionTag>[0];
            return fileScope.GetTags(spans);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose()
        {
            gherkinLanguageService.FileScopeChanged -= GherkinLanguageServiceOnFileScopeChanged;
        }
    }
}
