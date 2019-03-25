namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ShiftedInvalidFileBlock : ShiftedGherkinFileBlock<IInvalidFileBlock>, IInvalidFileBlock
    {
        public ShiftedInvalidFileBlock(IInvalidFileBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }
    }
}