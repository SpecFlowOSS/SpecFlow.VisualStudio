using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IStepBlock : IGherkinFileBlock
    {
        IEnumerable<GherkinStep> Steps { get; }
    }
}