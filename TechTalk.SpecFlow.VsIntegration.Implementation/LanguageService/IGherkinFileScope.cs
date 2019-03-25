using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IGherkinFileScope
    {
        GherkinDialect GherkinDialect { get; }
        IInvalidFileBlock InvalidFileEndingBlock { get; }
        IHeaderBlock HeaderBlock { get; }
        IBackgroundBlock BackgroundBlock { get; }
        IEnumerable<IScenarioBlock> ScenarioBlocks { get; }

        ITextSnapshot TextSnapshot { get; }
    }
}