using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IHeaderBlock : IGherkinFileBlock
    {
        IEnumerable<string> Tags { get; }
    }
}