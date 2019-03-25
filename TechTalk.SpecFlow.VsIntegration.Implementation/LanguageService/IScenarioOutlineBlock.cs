using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IScenarioOutlineBlock : IScenarioBlock
    {
        IEnumerable<IScenarioOutlineExampleSet> ExampleSets { get; }
    }
}