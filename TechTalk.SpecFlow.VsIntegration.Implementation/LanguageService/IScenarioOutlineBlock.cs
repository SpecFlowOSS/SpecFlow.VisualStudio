using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IScenarioOutlineBlock : IScenarioBlock
    {
        IEnumerable<IScenarioOutlineExampleSet> ExampleSets { get; }
    }
}