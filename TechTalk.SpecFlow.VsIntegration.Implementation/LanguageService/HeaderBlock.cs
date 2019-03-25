using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class HeaderBlock : GherkinFileBlock, IHeaderBlock
    {
        public IEnumerable<string> Tags { get; private set; }

        public HeaderBlock(string keyword, string title, int keywordLine, IEnumerable<string> tags, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
            Tags = tags;
        }
    }
}