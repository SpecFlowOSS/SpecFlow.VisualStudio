using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public interface ISourceFilePosition
    {
        string SourceFile { get; }
        FilePosition FilePosition { get; }
    }
}