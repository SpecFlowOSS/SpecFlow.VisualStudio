using System.Collections.Generic;
using Gherkin;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
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