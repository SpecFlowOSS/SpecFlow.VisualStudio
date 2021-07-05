using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class ShiftedGherkinFileBlock<T> : IGherkinFileBlock where T : IGherkinFileBlock
    {
        protected T baseBlock;
        public int LineShift { get; set; }

        public T BaseBlock
        {
            get { return baseBlock; }
        }

        public string Keyword
        {
            get { return baseBlock.Keyword; }
        }

        public string Title
        {
            get { return baseBlock.Title; }
        }

        public int KeywordLine
        {
            get { return baseBlock.KeywordLine + LineShift; }
        }

        public int BlockRelativeStartLine
        {
            get { return baseBlock.BlockRelativeStartLine; }
        }

        public int BlockRelativeEndLine
        {
            get { return baseBlock.BlockRelativeEndLine; }
        }

        public int BlockRelativeContentEndLine
        {
            get { return baseBlock.BlockRelativeContentEndLine; }
        }

        public IEnumerable<ClassificationSpan> ClassificationSpans
        {
            get { return baseBlock.ClassificationSpans; }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions
        {
            get { return baseBlock.OutliningRegions; }
        }

        public IEnumerable<ErrorInfo> Errors
        {
            get { return baseBlock.Errors; }
        }

        public ShiftedGherkinFileBlock(T baseBlock, int lineShift)
        {
            this.baseBlock = baseBlock;
            LineShift = lineShift;
        }
    }
}