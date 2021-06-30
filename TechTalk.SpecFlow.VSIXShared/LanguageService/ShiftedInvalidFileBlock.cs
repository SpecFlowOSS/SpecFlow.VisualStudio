namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class ShiftedInvalidFileBlock : ShiftedGherkinFileBlock<IInvalidFileBlock>, IInvalidFileBlock
    {
        public ShiftedInvalidFileBlock(IInvalidFileBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }
    }
}