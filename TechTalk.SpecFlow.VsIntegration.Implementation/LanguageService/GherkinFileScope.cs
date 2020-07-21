using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class GherkinFileScope : IGherkinFileScope
    {
        public GherkinDialectAdapter GherkinDialectAdapter { get; private set; }
        public ITextSnapshot TextSnapshot { get; private set; }

        public IInvalidFileBlock InvalidFileEndingBlock { get; set; }
        public IHeaderBlock HeaderBlock { get; set; }
        public IBackgroundBlock BackgroundBlock { get; set; }
        public List<IScenarioBlock> ScenarioBlocks { get; private set; }

        IEnumerable<IScenarioBlock> IGherkinFileScope.ScenarioBlocks { get { return ScenarioBlocks; } }

        public GherkinFileScope(GherkinDialectAdapter gherkinDialectAdapter, ITextSnapshot textSnapshot)
        {
            GherkinDialectAdapter = gherkinDialectAdapter;
            TextSnapshot = textSnapshot;
            ScenarioBlocks = new List<IScenarioBlock>();
        }
    }
}