using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ShiftedScenarioOutlineBlock : ShiftedGherkinFileBlock<IScenarioOutlineBlock>, IScenarioOutlineBlock
    {
        public ShiftedScenarioOutlineBlock(IScenarioOutlineBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<GherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }

        public IEnumerable<IScenarioOutlineExampleSet> ExampleSets
        {
            get { return baseBlock.ExampleSets; }
        }
    }
}