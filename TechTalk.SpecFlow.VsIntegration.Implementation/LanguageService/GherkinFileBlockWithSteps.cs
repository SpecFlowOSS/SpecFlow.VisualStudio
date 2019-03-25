using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal abstract class GherkinFileBlockWithSteps : GherkinFileBlock
    {
        public IEnumerable<GherkinStep> Steps { get; private set; }

        protected GherkinFileBlockWithSteps(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
            Steps = steps;
        }
    }
}