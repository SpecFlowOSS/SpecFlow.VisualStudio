using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ShiftedBackgroundBlock : ShiftedGherkinFileBlock<IBackgroundBlock>, IBackgroundBlock
    {
        public ShiftedBackgroundBlock(IBackgroundBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<GherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }
    }
}