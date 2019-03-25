namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface ITableWithRowPositions
    {
        void SetBlockRelativePosition(int rowIndex, int blockRelativeLine);
    }
}