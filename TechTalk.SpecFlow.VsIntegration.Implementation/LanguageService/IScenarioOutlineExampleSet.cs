namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IScenarioOutlineExampleSet : IKeywordLine
    {
        ScenarioOutlineExamplesTable ExamplesTable { get; set; }
    }
}