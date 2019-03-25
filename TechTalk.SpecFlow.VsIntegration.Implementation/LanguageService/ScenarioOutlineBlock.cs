using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ScenarioOutlineBlock : ScenarioBlock, IScenarioOutlineBlock
    {
        public IEnumerable<IScenarioOutlineExampleSet> ExampleSets { get; private set; }

        public ScenarioOutlineBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps, IEnumerable<IScenarioOutlineExampleSet> exampleSets) 
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors, steps)
        {
            ExampleSets = exampleSets;
        }
    }
}