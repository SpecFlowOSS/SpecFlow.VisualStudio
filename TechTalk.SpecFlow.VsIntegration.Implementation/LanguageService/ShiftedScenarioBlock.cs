using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ShiftedScenarioBlock : ShiftedGherkinFileBlock<IScenarioBlock>, IScenarioBlock
    {
        public ShiftedScenarioBlock(IScenarioBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<GherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }
    }
}