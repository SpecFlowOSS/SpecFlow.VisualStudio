namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class ScenarioOutlineExampleSet : KeywordLine, IScenarioOutlineExampleSet
    {
        public ScenarioOutlineExamplesTable ExamplesTable { get; set; }

        public ScenarioOutlineExampleSet(string keyword, string text, int blockRelativeLine) : base(keyword, text, blockRelativeLine)
        {
        }
    }
}