using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IHeaderBlock : IGherkinFileBlock
    {
        IEnumerable<string> Tags { get; }
    }
}