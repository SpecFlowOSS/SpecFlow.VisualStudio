using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IStepBlock : IGherkinFileBlock
    {
        IEnumerable<GherkinStep> Steps { get; }
    }
}