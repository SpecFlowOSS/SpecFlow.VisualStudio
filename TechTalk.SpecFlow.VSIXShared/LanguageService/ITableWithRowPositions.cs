namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface ITableWithRowPositions
    {
        void SetBlockRelativePosition(int rowIndex, int blockRelativeLine);
    }
}