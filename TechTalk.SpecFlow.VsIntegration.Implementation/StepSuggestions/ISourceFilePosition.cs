using Gherkin.Ast;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public interface ISourceFilePosition
    {
        string SourceFile { get; }
        Location Location { get; }
    }
}