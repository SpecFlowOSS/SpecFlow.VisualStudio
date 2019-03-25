using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal abstract class GherkinFileBlock : IGherkinFileBlock
    {
        public string Keyword { get; set; }
        public string Title { get; private set; }
        public int KeywordLine { get; private set; }
        public int BlockRelativeStartLine { get; private set; }
        public int BlockRelativeEndLine { get; private set; }
        public int BlockRelativeContentEndLine { get; private set; }
        public IEnumerable<ClassificationSpan> ClassificationSpans { get; private set; }
        public IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; private set; }
        public IEnumerable<ErrorInfo> Errors { get; private set; }

        protected GherkinFileBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors)
        {
            Keyword = keyword;
            Title = title;
            KeywordLine = keywordLine;
            BlockRelativeStartLine = blockRelativeStartLine;
            BlockRelativeEndLine = blockRelativeEndLine;
            BlockRelativeContentEndLine = blockRelativeContentEndLine;
            ClassificationSpans = classificationSpans;
            OutliningRegions = outliningRegions;
            Errors = errors;
        }
    }
}