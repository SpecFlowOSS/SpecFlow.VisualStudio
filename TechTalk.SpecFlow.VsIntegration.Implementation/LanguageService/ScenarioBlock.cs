using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ScenarioBlock : GherkinFileBlockWithSteps, IScenarioBlock
    {
        public ScenarioBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors, steps)
        {
        }
    }
}