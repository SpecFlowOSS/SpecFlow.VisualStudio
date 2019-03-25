namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IScenarioOutlineExampleSet : IKeywordLine
    {
        ScenarioOutlineExamplesTable ExamplesTable { get; set; }
    }
}